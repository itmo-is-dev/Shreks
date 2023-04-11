using ITMO.Dev.ASAP.Application.Abstractions.Google;
using ITMO.Dev.ASAP.Application.Abstractions.Google.Notifications;
using ITMO.Dev.ASAP.Application.Abstractions.Google.Sheets;
using ITMO.Dev.ASAP.Application.Abstractions.Queue;
using ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Notifications;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Application.Handlers.Google;

internal class SubjectCourseGroupQueueUpdateHandler : INotificationHandler<SubjectCourseGroupQueueUpdateNotification>
{
    private readonly ILogger<SubjectCourseGroupQueueUpdateHandler> _logger;
    private readonly IQueueUpdateService _queueUpdateService;
    private readonly ISheet<SubmissionsQueueDto> _sheet;
    private readonly ISubjectCourseTableService _subjectCourseTableService;
    private readonly IServiceScopeFactory _serviceProvider;

    public SubjectCourseGroupQueueUpdateHandler(
        ILogger<SubjectCourseGroupQueueUpdateHandler> logger,
        IQueueUpdateService queueUpdateService,
        ISheet<SubmissionsQueueDto> sheet,
        ISubjectCourseTableService subjectCourseTableService,
        IServiceScopeFactory serviceProvider)
    {
        _logger = logger;
        _queueUpdateService = queueUpdateService;
        _sheet = sheet;
        _subjectCourseTableService = subjectCourseTableService;
        _serviceProvider = serviceProvider;
    }

    public async Task Handle(
        SubjectCourseGroupQueueUpdateNotification notification,
        CancellationToken cancellationToken)
    {
        try
        {
            await ExecuteAsync(notification, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Error while updating queue for subject course {SubjectCourseId} group {GroupId}",
                notification.SubjectCourseId,
                notification.GroupId);
        }
    }

    private async Task ExecuteAsync(
        SubjectCourseGroupQueueUpdateNotification notification,
        CancellationToken cancellationToken)
    {
        using IServiceScope serviceScope = _serviceProvider.CreateScope();
        IPublisher publisher = serviceScope.ServiceProvider.GetRequiredService<IPublisher>();

        SubmissionsQueueDto submissionsQueue = await _queueUpdateService.GetSubmmissionsQueue(
            notification.GroupId,
            notification.SubjectCourseId,
            cancellationToken);

        string spreadsheetId = await _subjectCourseTableService
            .GetSubjectCourseTableId(notification.SubjectCourseId, cancellationToken);

        await _sheet.UpdateAsync(spreadsheetId, submissionsQueue, cancellationToken);

        var updateCacheNotification = new UpdateSubmissionsQueueCache.Notification(
            notification.SubjectCourseId,
            notification.GroupId,
            submissionsQueue);

        await publisher.Publish(updateCacheNotification, cancellationToken);
    }
}