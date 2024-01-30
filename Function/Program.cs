using Infrastructure;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureOpenApi()
    .ConfigureServices(s =>
    {

        s.AddHttpClient();
        s.AddDbContext<Context>(options => options.UseInMemoryDatabase("InMemDb"));
        s.AddSingleton<IOpenApiConfigurationOptions>(_ =>
        {
            OpenApiConfigurationOptions options = new OpenApiConfigurationOptions
            {
                Info = new OpenApiInfo
                {
                    Version = "2.0",
                    Title = "Microgroove API",
                    Description = "API for Microgroove endpoints."
                },
                OpenApiVersion = OpenApiVersionType.V3,
                IncludeRequestingHostName = true,
                ForceHttps = false,
                ForceHttp = false,
            };
            return options;
        });
    })
    .Build();

host.Run();
