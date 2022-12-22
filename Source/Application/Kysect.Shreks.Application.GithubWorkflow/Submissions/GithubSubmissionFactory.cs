﻿using Kysect.Shreks.Application.Validators;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Extensions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Specifications.Submissions;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Tools;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using Kysect.CommonLib;
using Kysect.Shreks.Application.Abstractions.Permissions;
using Kysect.Shreks.Application.GithubWorkflow.Extensions;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.Specifications.GroupAssignments;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Application.GithubWorkflow.Models;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Application.GithubWorkflow.Submissions;

public class GithubSubmissionFactory : IGithubSubmissionFactory
{
    private readonly IShreksDatabaseContext _context;
    private readonly IPermissionValidator _permissionValidator;

    public GithubSubmissionFactory(IShreksDatabaseContext context, IPermissionValidator permissionValidator)
    {
        _context = context;
        _permissionValidator = permissionValidator;
    }

    public async Task<GithubSubmissionCreationResult> CreateOrUpdateGithubSubmission(
        GithubPullRequestDescriptor pullRequestDescriptor,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pullRequestDescriptor);

        Guid userId = await GetUserId(pullRequestDescriptor, cancellationToken);
        Guid assignmentId = await GetAssignmentId(pullRequestDescriptor, cancellationToken);

        bool triggeredByMentor = await _permissionValidator.IsOrganizationMentor(
            userId, pullRequestDescriptor.Organization);

        bool triggeredByAnotherUser = !PermissionValidator.IsRepositoryOwner(
            pullRequestDescriptor.Sender, pullRequestDescriptor.Repository);

        var submissionSpec = new FindLatestGithubSubmission(
            pullRequestDescriptor.Organization,
            pullRequestDescriptor.Repository,
            pullRequestDescriptor.PrNumber);

        GithubSubmission? submission = await _context.SubmissionAssociations
            .WithSpecification(submissionSpec)
            .Where(x => !x.State.Equals(SubmissionStateKind.Completed))
            .FirstOrDefaultAsync(cancellationToken);

        bool isCreated = false;

        if (submission is null || submission.IsRated)
        {
            if (triggeredByAnotherUser && !triggeredByMentor)
            {
                throw DomainInvalidOperationException.RepositoryAssignedToAnotherUserClosePullRequest(
                    pullRequestDescriptor.Repository, pullRequestDescriptor.Sender);
            }

            submission = await CreateGithubSubmissionAsync(
                userId,
                assignmentId,
                pullRequestDescriptor,
                cancellationToken);
            isCreated = true;

            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else if (!triggeredByMentor)
        {
            submission.UpdateDate(Calendar.CurrentDateTime);

            _context.Submissions.Update(submission);
            await _context.SaveChangesAsync(cancellationToken);

            if (triggeredByAnotherUser)
            {
                throw DomainInvalidOperationException.RepositoryAssignedToAnotherUserSubmissionUpdated(
                    pullRequestDescriptor.Repository, pullRequestDescriptor.Sender);
            }
        }

        return new GithubSubmissionCreationResult(submission, isCreated);
    }

    private async Task<Guid> GetUserId(
        GithubPullRequestDescriptor pullRequestDescriptor,
        CancellationToken cancellationToken)
    {
        User user = await _context.UserAssociations.GetUserByGithubUsername(pullRequestDescriptor.Sender);

        return user.Id;
    }

    private async Task<Guid> GetAssignmentId(
        GithubPullRequestDescriptor pullRequestDescriptor,
        CancellationToken cancellationToken)
    {
        GithubSubjectCourseAssociation? subjectCourseAssociation = await _context.SubjectCourseAssociations
            .OfType<GithubSubjectCourseAssociation>()
            .FirstOrDefaultAsync(gsc => gsc.GithubOrganizationName == pullRequestDescriptor.Organization,
                cancellationToken);

        if (subjectCourseAssociation is null)
            throw EntityNotFoundException.SubjectCourseForOrganizationNotFound(pullRequestDescriptor.Organization);

        Assignment? assignment = subjectCourseAssociation.SubjectCourse.Assignments.FirstOrDefault(
            a => a.ShortName == pullRequestDescriptor.BranchName);

        if (assignment is null)
        {
            string assignments = subjectCourseAssociation.SubjectCourse
                .Assignments
                .OrderBy(a => a.Order)
                .ToSingleString(a => a.ShortName);

            throw EntityNotFoundException.AssignmentWasNotFound(pullRequestDescriptor.BranchName,
                subjectCourseAssociation.SubjectCourse.Title, assignments);
        }

        return assignment.Id;
    }

    private async Task<GithubSubmission> CreateGithubSubmissionAsync(
        Guid userId,
        Guid assignmentId,
        GithubPullRequestDescriptor pullRequestDescriptor,
        CancellationToken cancellationToken)
    {
        // TODO: student must be filtered by subject course
        Student? student = await _context.Students.FindAsync(new object[] { userId }, cancellationToken);

        if (student is null)
        {
            GithubSubjectCourseAssociation? courseAssociation = await _context.SubjectCourseAssociations
                .OfType<GithubSubjectCourseAssociation>()
                .Include(association => association.SubjectCourse)
                .SingleOrDefaultAsync(
                    x => x.GithubOrganizationName == pullRequestDescriptor.Organization,
                    cancellationToken);

            if (courseAssociation is null)
                throw EntityNotFoundException.SubjectCourseForOrganizationNotFound(pullRequestDescriptor.Organization);

            _ = await _context.Mentors.FindAsync(new object[] { userId, courseAssociation.SubjectCourse.Id },
                    cancellationToken)
                ?? throw EntityNotFoundException.UserNotFoundInSubjectCourse(userId,
                    courseAssociation.SubjectCourse.Title);

            student = await FindStudentByRepositoryName(pullRequestDescriptor, cancellationToken);
        }

        var groupAssignmentSpec = new GetStudentGroupAssignment(student.UserId, assignmentId);

        GroupAssignment groupAssignment = await _context.GroupAssignments
            .WithSpecification(groupAssignmentSpec)
            .SingleAsync(cancellationToken);

        var studentAssignmentSubmissionsSpec = new GetStudentAssignmentSubmissions(
            student.UserId, assignmentId);

        int count = await _context.Submissions
            .WithSpecification(studentAssignmentSubmissionsSpec)
            .CountAsync(cancellationToken);

        var submission = new GithubSubmission
        (
            count + 1,
            student,
            groupAssignment,
            Calendar.CurrentDateTime,
            pullRequestDescriptor.Payload,
            pullRequestDescriptor.Organization,
            pullRequestDescriptor.Repository,
            pullRequestDescriptor.PrNumber
        );

        return submission;
    }

    private async Task<Student> FindStudentByRepositoryName(
        GithubPullRequestDescriptor pullRequestDescriptor,
        CancellationToken cancellationToken)
    {
        User user = await _context.UserAssociations.GetUserByGithubUsername(pullRequestDescriptor.Repository);

        return await _context.Students.GetByIdAsync(user.Id, cancellationToken);
    }
}