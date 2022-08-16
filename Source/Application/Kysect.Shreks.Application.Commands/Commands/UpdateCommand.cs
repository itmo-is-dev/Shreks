using CommandLine;
using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Study;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/update")]
public class UpdateCommand : IShreksCommand
{
    public UpdateCommand(string submissionId, double? ratingPercent, double? extraPoints)
    {
        SubmissionId = submissionId;
        RatingPercent = ratingPercent;
        ExtraPoints = extraPoints;
    }

    [Value(0, Required = true, MetaName = "SubmissionId")]
    public string SubmissionId { get; }
    
    [Option(shortName:'r', longName: "rating", Required = false)]
    public double? RatingPercent { get; }
    
    [Option(shortName:'e', longName:"extra", Required = false)]
    public double? ExtraPoints { get; }
    
    public Task<TResult> Accept<TResult>(IShreksCommandVisitor<TResult> visitor, ICommandContextFactory contextFactory,
        CancellationToken cancellationToken) 
        where TResult : IShreksCommandResult
    {
        return visitor.Visit(this, contextFactory, cancellationToken);
    }

    public async Task<SubmissionDto> ExecuteAsync(BaseContext context)
    {
        Guid submissionId = Guid.Parse(SubmissionId);
        SubmissionDto submissionDto = null!;
        if (RatingPercent.HasValue)
        {
             var command = new UpdateSubmissionPoints.Command(submissionId, RatingPercent.Value);
             var response = await context.Mediator.Send(command, context.CancellationToken);
             submissionDto = response.Submission;
        }

        if (ExtraPoints.HasValue)
        {
            //TODO: add update extra balls command (or add them to previous command)
        }

        return submissionDto;
    }
}