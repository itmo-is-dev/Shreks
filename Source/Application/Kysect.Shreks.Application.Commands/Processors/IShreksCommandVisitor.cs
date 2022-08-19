using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Result;

namespace Kysect.Shreks.Application.Commands.Processors;

public interface IShreksCommandVisitor<TResult> where TResult : IShreksCommandResult
{
    Task<TResult> VisitAsync(RateCommand rateCommand);
    Task<TResult> VisitAsync(UpdateCommand updateCommand);
}