using ITMO.Dev.ASAP.Application.Dto.Submissions;
using ITMO.Dev.ASAP.Common.Resources;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.Octokit.Extensions;
using ITMO.Dev.ASAP.Github.Application.Octokit.Notifications;
using ITMO.Dev.ASAP.Github.Application.Specifications;
using ITMO.Dev.ASAP.Github.Domain.Assignments;
using ITMO.Dev.ASAP.Github.Domain.Submissions;
using ITMO.Dev.ASAP.Github.Domain.Users;
using ITMO.Dev.ASAP.Presentation.Contracts.Services;
using MediatR;
using static ITMO.Dev.ASAP.Github.Application.Contracts.PullRequestEvents.PullRequestUpdated;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.PullRequestEvents;

internal class PullRequestUpdatedHandler : IRequestHandler<Command>
{
    private readonly IDatabaseContext _context;
    private readonly IAsapSubmissionWorkflowService _submissionWorkflowService;
    private readonly INotifierFactory _notifierFactory;

    public PullRequestUpdatedHandler(
        IDatabaseContext context,
        IAsapSubmissionWorkflowService submissionWorkflowService,
        INotifierFactory notifierFactory)
    {
        _context = context;
        _submissionWorkflowService = submissionWorkflowService;
        _notifierFactory = notifierFactory;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        GithubUser issuer = await _context.Users
            .GetForUsernameAsync(request.PullRequest.Sender, cancellationToken);

        GithubUser user = await _context.Users
            .GetForUsernameAsync(request.PullRequest.Repository, cancellationToken);

        GithubAssignment assignment = await _context
            .GetAssignmentForPullRequestAsync(request.PullRequest, cancellationToken);

        SubmissionUpdateResult result = await _submissionWorkflowService.SubmissionUpdatedAsync(
            issuer.Id,
            user.Id,
            assignment.Id,
            cancellationToken);

        IPullRequestEventNotifier notifier = _notifierFactory.ForPullRequest(request.PullRequest);

        if (result.IsCreated)
        {
            var submission = new GithubSubmission(
                result.Submission.Id,
                assignment.Id,
                user.Id,
                result.Submission.SubmissionDate,
                request.PullRequest.Organization,
                request.PullRequest.Repository,
                request.PullRequest.PullRequestNumber);

            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync(default);

            string message = UserCommandProcessingMessage.SubmissionCreated(result.Submission.ToDisplayString());
            await notifier.SendCommentToPullRequest(message);
        }
        else
        {
            await notifier.NotifySubmissionUpdate(result.Submission);
        }

        return Unit.Value;
    }
}