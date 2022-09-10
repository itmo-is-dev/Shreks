﻿using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Core.Extensions;
using Kysect.Shreks.Core.Specifications.Github;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow;

public class SubjectCourseGithubOrganizationManager : ISubjectCourseGithubOrganizationManager
{
    private readonly ISubjectCourseGithubOrganizationInviteSender _inviteSender;
    private readonly ISubjectCourseGithubOrganizationRepositoryManager _repositoryManager;
    private readonly IShreksDatabaseContext _context;
    private readonly ILogger<SubjectCourseGithubOrganizationManager> _logger;

    public SubjectCourseGithubOrganizationManager(
        ISubjectCourseGithubOrganizationInviteSender inviteSender,
        ISubjectCourseGithubOrganizationRepositoryManager repositoryManager,
        IShreksDatabaseContext context,
        ILogger<SubjectCourseGithubOrganizationManager> logger)
    {
        _inviteSender = inviteSender;
        _repositoryManager = repositoryManager;
        _context = context;
        _logger = logger;
    }

    public async Task UpdateOrganizations(CancellationToken cancellationToken)
    {
        List<GithubSubjectCourseAssociation> githubSubjectCourseAssociations = await _context
            .SubjectCourseAssociations
            .OfType<GithubSubjectCourseAssociation>()
            .ToListAsync(cancellationToken);

        foreach (GithubSubjectCourseAssociation subjectAssociation in githubSubjectCourseAssociations)
        {
            List<string> usernames = await _context
                .SubjectCourseGroups
                .WithSpecification(new GetSubjectCourseGithubUsers(subjectAssociation.SubjectCourse.Id))
                .Select(association => association.GithubUsername)
                .ToListAsync(cancellationToken: cancellationToken);

            await _inviteSender.Invite(subjectAssociation.GithubOrganizationName, usernames);
            await GenerateRepositories(_repositoryManager, usernames, subjectAssociation.GithubOrganizationName, subjectAssociation.TemplateRepositoryName);
        }
    }

    private async Task GenerateRepositories(
        ISubjectCourseGithubOrganizationRepositoryManager repositoryManager,
        IReadOnlyCollection<string> usernames,
        string organizationName,
        string templateName)
    {
        IReadOnlyCollection<string> repositories = await repositoryManager.GetRepositories(organizationName);

        if (!repositories.Contains(templateName))
        {
            string message = $"No template repository found for organization {organizationName}";
            _logger.LogWarning(message);
            return;
        }

        foreach (string username in usernames)
        {
            string newRepositoryName = username;

            if (repositories.Any(r => r.Equals(newRepositoryName, StringComparison.OrdinalIgnoreCase)))
                continue;

            try
            {
                await repositoryManager.CreateRepositoryFromTemplate(organizationName, newRepositoryName, templateName);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create repo for {username}", username);
            }

            try
            {
                await repositoryManager.AddAdminPermission(organizationName, newRepositoryName, username);
            }
            catch (Exception e) when (e.Message == "Invalid Status Code returned. Expected a 204 or a 404")
            {
                _logger.LogWarning($"Octokit return wrong error code for {username}.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to add user {username} as repo admin.");
                continue;
            }

            _logger.LogInformation("Successfully created repository for user {User}", username);
        }
    }
}