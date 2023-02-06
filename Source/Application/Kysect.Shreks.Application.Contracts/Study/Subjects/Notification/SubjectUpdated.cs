using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Subjects.Notification;

internal static class SubjectUpdated
{
    public record Notification(SubjectDto Subject) : INotification;
}