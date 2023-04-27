using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Domain.Models;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.Submissions.States;
using ITMO.Dev.ASAP.Domain.SubmissionStateWorkflows;
using ITMO.Dev.ASAP.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Tests.Extensions;

public static class DatabaseContextTestExtensions
{
    public static Task<Submission> GetSubmissionAsync(
        this IDatabaseContext context,
        params ISubmissionState[] states)
    {
        return context.Submissions
            .Where(submission => states.Any(x => x.Equals(submission.State)))
            .Where(x =>
                x.GroupAssignment.Assignment.SubjectCourse.WorkflowType ==
                SubmissionStateWorkflowType.ReviewWithDefense)
            .FirstAsync();
    }

    public static Task<User> GetNotMentorUser(
        this IDatabaseContext context,
        GroupAssignment assignment,
        User excludedUser)
    {
        IEnumerable<Guid> mentors = assignment.Assignment.SubjectCourse.Mentors
            .Select(x => x.UserId);

        return context.Users
            .Where(x => x.Equals(excludedUser) == false)
            .Where(user => mentors.Contains(user.Id) == false)
            .FirstAsync();
    }

    public static Task<GroupAssignment> GetGroupAssignmentWithUnSubmittedStudents(this IDatabaseContext context)
    {
        return context.GroupAssignments
            .Where(x => x.Assignment.SubjectCourse.WorkflowType == SubmissionStateWorkflowType.ReviewWithDefense)
            .Where(assignment => assignment.Group.Students.Any(student =>
                assignment.Submissions.All(submission => submission.Student.Equals(student) == false)))
            .FirstAsync();
    }

    public static Task<GroupAssignment> GetGroupAssignmentWithSubmittedStudents(this IDatabaseContext context)
    {
        GroupAssignment groupAssignment = context.GroupAssignments
            .Where(x => x.Assignment.SubjectCourse.WorkflowType == SubmissionStateWorkflowType.ReviewWithDefense)
            .AsEnumerable()
            .First(assignment => assignment.Submissions.GroupBy(x => x.Student)
                .Select(x => x.OrderByDescending(xx => xx.Code).First())
                .Any(x => x.State.Kind is SubmissionStateKind.Active));

        return Task.FromResult(groupAssignment);
    }
}