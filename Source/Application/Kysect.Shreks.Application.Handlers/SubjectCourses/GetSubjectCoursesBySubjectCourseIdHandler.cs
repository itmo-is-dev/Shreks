using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Queries.GetSubjectCoursesBySubjectCourseId;

namespace Kysect.Shreks.Application.Handlers.SubjectCourses;

internal class GetSubjectCoursesBySubjectCourseIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetSubjectCoursesBySubjectCourseIdHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<SubjectCourse> courses = await _context.SubjectCourses
            .Where(x => x.Subject.Id.Equals(request.SubjectCourseId))
            .ToListAsync(cancellationToken);

        SubjectCourseDto[] dto = courses
            .Select(x => x.ToDto())
            .ToArray();

        return new Response(dto);
    }
}