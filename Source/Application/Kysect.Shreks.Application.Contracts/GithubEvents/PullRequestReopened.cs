using Kysect.Shreks.Application.Dto.Submissions;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.GithubEvents;

internal static class PullRequestReopened
{
    public record Command(Guid IssuerId, Guid SubmissionId) : IRequest<Response>;

    public record Response(SubmissionActionMessageDto Message);
}