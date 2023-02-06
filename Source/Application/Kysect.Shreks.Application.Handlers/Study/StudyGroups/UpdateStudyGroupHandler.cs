﻿using Kysect.Shreks.Application.Contracts.Study.StudyGroups.Notifications;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.StudyGroups.Commands.UpdateStudyGroup;

namespace Kysect.Shreks.Application.Handlers.Study.StudyGroups;

internal class UpdateStudyGroupHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IPublisher _publisher;

    public UpdateStudyGroupHandler(IShreksDatabaseContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        StudentGroup studentGroup = await _context.StudentGroups.GetByIdAsync(request.Id, cancellationToken);
        studentGroup.Name = request.NewName;

        _context.StudentGroups.Update(studentGroup);
        await _context.SaveChangesAsync(cancellationToken);

        StudyGroupDto dto = studentGroup.ToDto();

        var notification = new StudyGroupUpdated.Notification(dto);
        await _publisher.PublishAsync(notification, cancellationToken);

        return new Response(dto);
    }
}