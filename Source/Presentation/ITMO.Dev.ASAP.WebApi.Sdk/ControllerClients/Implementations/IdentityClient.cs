using ITMO.Dev.ASAP.Application.Dto.Identity;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Identity;
using ITMO.Dev.ASAP.WebApi.Sdk.Extensions;
using ITMO.Dev.ASAP.WebApi.Sdk.Tools;
using Newtonsoft.Json;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients.Implementations;

internal class IdentityClient : IIdentityClient
{
    private readonly ClientRequestHandler _handler;
    private readonly JsonSerializerSettings _serializerSettings;

    public IdentityClient(HttpClient client, JsonSerializerSettings serializerSettings)
    {
        _serializerSettings = serializerSettings;
        _handler = new ClientRequestHandler(client, serializerSettings);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/identity/login")
        {
            Content = request.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<LoginResponse>(message, cancellationToken);
    }

    public async Task ChangeUserRoleAsync(string username, string roleName, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Put, $"api/identity/users/{username}/role")
        {
            Content = roleName.ToContent(_serializerSettings),
        };

        await _handler.SendAsync(message, cancellationToken);
    }

    public async Task CreateUserAccountAsync(Guid id, CreateUserAccountRequest request, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, $"api/identity/user/{id}/create")
        {
            Content = request.ToContent(_serializerSettings),
        };

        await _handler.SendAsync(message, cancellationToken);
    }

    public async Task UpdateUsernameAsync(UpdateUsernameRequest request, CancellationToken cancellationToken = default)
    {
<<<<<<< HEAD
        using var message = new HttpRequestMessage(HttpMethod.Put, "api/identity/username")
=======
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/identity/update/username")
>>>>>>> 002b91de2d9686aedd4dcc24b963fbc1f57dd13d
        {
            Content = request.ToContent(_serializerSettings),
        };

        await _handler.SendAsync(message, cancellationToken);
    }

    public async Task<PasswordOptionsDto> GetPasswordOptionsAsync(CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, "api/identity/password/options");
        return await _handler.SendAsync<PasswordOptionsDto>(message, cancellationToken);
    }

    public async Task UpdatePasswordAsync(UpdatePasswordRequest request, CancellationToken cancellationToken = default)
    {
<<<<<<< HEAD
        using var message = new HttpRequestMessage(HttpMethod.Put, "api/identity/password")
=======
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/identity/update/password")
>>>>>>> 002b91de2d9686aedd4dcc24b963fbc1f57dd13d
        {
            Content = request.ToContent(_serializerSettings),
        };

        await _handler.SendAsync(message, cancellationToken);
    }
}