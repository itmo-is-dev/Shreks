using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Extensions;
using ITMO.Dev.ASAP.Domain.Study;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries.HasAccessToSubjectCourse;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class HasAccessToSubjectCourseHandler : IRequestHandler<Query, Response>
{
    private readonly ICurrentUser _currentUser;
    private readonly IDatabaseContext _context;

    public HasAccessToSubjectCourseHandler(ICurrentUser currentUser, IDatabaseContext context)
    {
        _currentUser = currentUser;
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses
            .GetByIdAsync(request.SubjectCourseId, cancellationToken);

        bool hasAccess = _currentUser.HasAccessToSubjectCourse(subjectCourse);

        return new Response(hasAccess);
    }
}