using ITMO.Dev.ASAP.Domain.Submissions;

namespace ITMO.Dev.ASAP.Application.Abstractions.Submissions;

public interface ISubmissionFactory
{
    Task<Submission> CreateAsync(Guid userId, Guid assignmentId, CancellationToken cancellationToken);
}