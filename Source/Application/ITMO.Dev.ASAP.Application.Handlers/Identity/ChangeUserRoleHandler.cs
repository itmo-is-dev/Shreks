using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Entities;
using ITMO.Dev.ASAP.Identity.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.ChangeUserRole;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class ChangeUserRoleHandler : IRequestHandler<Command>
{
    private readonly ICurrentUser _currentUser;
    private readonly RoleManager<AsapIdentityRole> _roleManager;
    private readonly UserManager<AsapIdentityUser> _userManager;

    public ChangeUserRoleHandler(
        ICurrentUser currentUser,
        UserManager<AsapIdentityUser> userManager,
        RoleManager<AsapIdentityRole> roleManager)
    {
        _currentUser = currentUser;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        AsapIdentityUser user = await _userManager.GetByNameAsync(request.Username);
        string? userRoleName = (await _userManager.GetRolesAsync(user)).SingleOrDefault();

        if (!_currentUser.CanChangeRole(userRoleName, request.UserRole))
            throw new RoleChangingException($"Unable to change role of {user.UserName}");

        await _userManager.RemoveFromRolesAsync(user, new[] { userRoleName });

        await _roleManager.CreateRoleIfNotExistsAsync(request.UserRole);
        await _userManager.AddToRoleAsync(user, request.UserRole);

        return Unit.Value;
    }
}