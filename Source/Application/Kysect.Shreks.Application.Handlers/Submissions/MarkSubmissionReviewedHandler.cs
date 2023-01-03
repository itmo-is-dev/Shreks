using AutoMapper;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Abstractions.Permissions;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Submissions.Commands.MarkSubmissionReviewed;

namespace Kysect.Shreks.Application.Handlers.Submissions;

internal class MarkSubmissionReviewedHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;
    private readonly IPermissionValidator _permissionValidator;
    private readonly ITableUpdateQueue _tableUpdateQueue;

    public MarkSubmissionReviewedHandler(
        IPermissionValidator permissionValidator,
        IShreksDatabaseContext context,
        IMapper mapper,
        ITableUpdateQueue tableUpdateQueue)
    {
        _permissionValidator = permissionValidator;
        _context = context;
        _mapper = mapper;
        _tableUpdateQueue = tableUpdateQueue;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        await _permissionValidator.EnsureSubmissionMentorAsync(
            request.IssuerId, request.SubmissionId, cancellationToken);

        Submission submission = await _context.Submissions
            .IncludeSubjectCourse()
            .GetByIdAsync(request.SubmissionId, cancellationToken);

        submission.MarkAsReviewed();

        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        _tableUpdateQueue.EnqueueCoursePointsUpdate(submission.GetSubjectCourseId());

        SubmissionDto dto = _mapper.Map<SubmissionDto>(submission);

        return new Response(dto);
    }
}