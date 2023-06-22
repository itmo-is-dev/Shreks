using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Subjects;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Subjects;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.Subjects;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Tools;
using ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;
using ITMO.Dev.ASAP.WebUI.Abstractions.Extensions;
using ITMO.Dev.ASAP.WebUI.Abstractions.SafeExecution;
using System.Reactive.Linq;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.Subjects;

public class SubjectManager : ISubjectManager, IDisposable
{
    private readonly IDisposable _subscription;
    private readonly ISubjectClient _subjectClient;
    private readonly ISafeExecutor _safeExecutor;
    private readonly IMessageConsumer _consumer;
    private readonly IMessageProducer _producer;

    public SubjectManager(
        ISubjectClient subjectClient,
        ISafeExecutor safeExecutor,
        IMessageConsumer consumer,
        IMessageProducer producer)
    {
        _subjectClient = subjectClient;
        _safeExecutor = safeExecutor;
        _consumer = consumer;
        _producer = producer;

        _subscription = new SubscriptionBuilder()
            .Subscribe(producer.Observe<SubjectSelectedEvent>().Subscribe(OnSubjectSelected))
            .Build();
    }

    public IObservable<SubjectCreatedEvent> SubjectCreated => _producer.Observe<SubjectCreatedEvent>();

    public IObservable<SubjectDto> Subject => _producer
        .Observe<CurrentSubjectLoadedEvent>()
        .Select(x => x.Subject);

    public async ValueTask CreateAsync(string title, CancellationToken cancellationToken)
    {
        await using ISafeExecutionBuilder<SubjectDto> builder = _safeExecutor.Execute(async () =>
        {
            var request = new CreateSubjectRequest(title);
            SubjectDto subject = await _subjectClient.CreateAsync(request, cancellationToken);

            return subject;
        });

        builder.Title = "Failed to create subject";
        builder.OnSuccess(x => _consumer.Send(new SubjectCreatedEvent(x)));
    }

    public ValueTask SelectAsync(Guid subjectId)
    {
        var evt = new SubjectSelectedEvent(subjectId);
        _consumer.Send(evt);

        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }

    private async void OnSubjectSelected(SubjectSelectedEvent evt)
    {
        await using ISafeExecutionBuilder<SubjectDto> builder = _safeExecutor
            .Execute(() => _subjectClient.GetByIdAsync(evt.SubjectId));

        builder.Title = "Failed to load current subject";

        builder.OnSuccess(subject =>
        {
            var loadedEvent = new CurrentSubjectLoadedEvent(subject);
            _consumer.Send(loadedEvent);
        });
    }
}