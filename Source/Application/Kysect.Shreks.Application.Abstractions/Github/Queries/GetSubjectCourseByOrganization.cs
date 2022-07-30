using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Queries;

public static class GetSubjectCourseByOrganization
{
    public record Query(string OrganizationName) : IRequest<Response>;

    public record Response(Guid SubjectCourseId);
}