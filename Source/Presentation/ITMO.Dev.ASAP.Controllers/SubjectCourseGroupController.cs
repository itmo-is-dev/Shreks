using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Commands;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.SubjectCourseGroups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = AsapIdentityRoleNames.AdminRoleName)]
public class SubjectCourseGroupController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubjectCourseGroupController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public CancellationToken CancellationToken => HttpContext.RequestAborted;

    [HttpPost]
    public async Task<ActionResult<SubjectCourseGroupDto>> CreateSubjectCourseGroup(
        CreateSubjectCourseGroupRequest request)
    {
        (Guid subjectCourseId, Guid groupId) = request;

        var command = new CreateSubjectCourseGroup.Command(subjectCourseId, groupId);
        CreateSubjectCourseGroup.Response result = await _mediator.Send(command, CancellationToken);

        return Ok(result);
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<IReadOnlyCollection<SubjectCourseGroupDto>>> BulkCreateSubjectCourseGroupsAsync(
        BulkCreateSubjectCourseGroupsRequest request)
    {
        var command = new BulkCreateSubjectCourseGroups.Command(request.SubjectCourseId, request.GroupIds);
        BulkCreateSubjectCourseGroups.Response response = await _mediator.Send(command, CancellationToken);

        return Ok(response.Groups);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteSubjectCourseGroup(DeleteSubjectCourseGroupRequest request)
    {
        (Guid subjectCourseId, Guid groupId) = request;

        var command = new DeleteSubjectCourseGroup.Command(subjectCourseId, groupId);
        await _mediator.Send(command, CancellationToken);

        return NoContent();
    }
}