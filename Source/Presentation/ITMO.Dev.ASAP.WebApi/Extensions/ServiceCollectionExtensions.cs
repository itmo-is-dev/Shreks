using FluentSerialization.Extensions.NewtonsoftJson;
using ITMO.Dev.ASAP.Application.Dto.Tools;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Application.Handlers.Extensions;
using ITMO.Dev.ASAP.Controllers;
using ITMO.Dev.ASAP.DataAccess.Extensions;
using ITMO.Dev.ASAP.Github.Octokit.Extensions;
using ITMO.Dev.ASAP.Github.Presentation.Webhooks.Extensions;
using ITMO.Dev.ASAP.Identity.Extensions;
using ITMO.Dev.ASAP.WebApi.Configuration;
using Microsoft.EntityFrameworkCore;
using ConfigurationBuilder = FluentSerialization.ConfigurationBuilder;

namespace ITMO.Dev.ASAP.WebApi.Extensions;

#pragma warning disable CA1506

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection ConfigureServiceCollection(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        WebApiConfiguration webApiConfiguration,
        IConfigurationSection identityConfigurationSection,
        bool isDevelopmentEnvironment)
    {
        serviceCollection
            .AddControllers()
            .AddNewtonsoftJson(x => ConfigurationBuilder
                .Build(new DtoSerializationConfiguration())
                .ApplyToSerializationSettings(x.SerializerSettings))
            .AddApplicationPart(typeof(IControllerProjectMarker).Assembly)
            .AddControllersAsServices();

        serviceCollection
            .AddSwagger()
            .AddApplicationConfiguration()
            .AddHandlers()
            .AddGithubPresentation()
            .AddDatabaseContext(o => o
                .UseNpgsql(webApiConfiguration.PostgresConfiguration.ToConnectionString(webApiConfiguration
                    .DbNamesConfiguration.ApplicationDbName))
                .UseLazyLoadingProxies());

        serviceCollection.AddIdentityConfiguration(
            identityConfigurationSection,
            x => x.UseNpgsql(
                webApiConfiguration.PostgresConfiguration.ToConnectionString(webApiConfiguration.DbNamesConfiguration
                    .IdentityDbName)));

        serviceCollection
            .AddGoogleIntegrationServices(webApiConfiguration)
            .AddGithubServices(configuration);

        if (isDevelopmentEnvironment && webApiConfiguration.TestEnvironmentConfiguration is not null)
            serviceCollection.AddEntityGeneratorsAndSeeding(webApiConfiguration.TestEnvironmentConfiguration);

        serviceCollection.AddRazorPages();

        return serviceCollection;
    }
}