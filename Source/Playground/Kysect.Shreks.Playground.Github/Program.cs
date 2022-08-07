using Kysect.Shreks.Application.Commands.Extensions;
using Kysect.Shreks.Integration.Github.Extensions;
using Kysect.Shreks.Integration.Github.Helpers;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.File("ShreksGithubPlayground.log")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

var shreksConfiguration = builder.Configuration.GetSection(nameof(ShreksConfiguration)).Get<ShreksConfiguration>();
shreksConfiguration.Verify();

builder.Services
    .AddGithubServices(shreksConfiguration)
    .AddWebhookProcessors()
    .AddCommandParser();

builder.Services
    .AddLogging(logBuilder => logBuilder.AddSerilog());

var app = builder.Build();

app.UseGithubIntegration(shreksConfiguration);

app.Run();