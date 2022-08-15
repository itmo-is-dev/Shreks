using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.ValueObject;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Submissions.Commands.UpdateSubmissionPoints;
namespace Kysect.Shreks.Application.Handlers.Submissions;

public class UpdateSubmissionPointsHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public UpdateSubmissionPointsHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var submission = await _context.Submissions.GetByIdAsync(request.SubmissionId, cancellationToken);

        submission.Points = new Points(request.NewPoints);
        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(submission.ToDto());
    }
}