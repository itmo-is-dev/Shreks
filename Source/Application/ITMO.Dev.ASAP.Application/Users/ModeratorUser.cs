﻿using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Identity.Entities;

namespace ITMO.Dev.ASAP.Application.Users;

internal class ModeratorUser : ICurrentUser
{
    private static readonly HashSet<string?> AvailableRolesToChange = new()
    {
        AsapIdentityRole.MentorRoleName,
    };

    public ModeratorUser(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }

    public bool CanUpdateAllDeadlines => true;

    public bool CanCreateUserWithRole(string roleName)
    {
        return AvailableRolesToChange.Contains(roleName);
    }

    public bool CanChangeUserRole(string currentRoleName, string newRoleName)
    {
        return AvailableRolesToChange.Contains(currentRoleName)
               && AvailableRolesToChange.Contains(newRoleName);
    }
}