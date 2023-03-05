﻿using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace ITMO.Dev.ASAP.Identity.Extensions;

public static class UserManagerExtensions
{
    public static async Task CreateOrElseThrowAsync(
        this UserManager<AsapIdentityUser> userManager,
        AsapIdentityUser user,
        string password)
    {
        IdentityResult result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            throw new RegistrationFailedException(string.Join(' ', result.Errors.Select(x => x.Description)));
    }
}
