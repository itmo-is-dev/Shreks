﻿using FluentSpreadsheets;
using FluentSpreadsheets.SheetBuilders;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Integration.Google.Segments;
using MediatR;
using static Microsoft.Extensions.DependencyInjection.ActivatorUtilities;

namespace Kysect.Shreks.Integration.Google.Factories;

public class QueueSheetDataFactory : ISheetDataFactory<Queue>
{
    private readonly ISheetBuilder _sheetBuilder;
    private readonly ISheetSegment<Unit, Submission, Unit>[] _segments;

    public QueueSheetDataFactory(
        ISheetBuilder sheetBuilder,
        IServiceProvider serviceProvider)
    {
        _sheetBuilder = sheetBuilder;

        _segments = new ISheetSegment<Unit, Submission, Unit>[]
        {
            CreateInstance<QueueStudentSegment>(serviceProvider),
            CreateInstance<AssignmentDataSegment>(serviceProvider)
        };
    }

    public IComponent Create(Queue queue)
    {
        var sheetData = new SheetData<Unit, Submission, Unit>(Unit.Value, queue.Submissions, Unit.Value);
        return _sheetBuilder.Build(_segments, sheetData);
    }
}