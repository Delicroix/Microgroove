using Domain.Customers;
using Infrastructure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Function.Endpoints;

public class CustomersGetAll
{
    private readonly Context _context;
    private readonly ILogger _logger;

    public CustomersGetAll(Context context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger<CustomersGetAll>();
    }

    [Function(nameof(CustomersGetAll))]
    [OpenApiOperation(operationId: "CustomersGetAll", tags: new[] { "Customer" }, Summary = "Get a list of all Customers.", Description = "Operation get customers in database.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Customer>), Summary = "Job list.", Description = "List of all the jobs.")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);

        response.Headers.Add("Content-Type", "application/json; charset=utf-8");

        var customers = await _context.Customers.AsNoTracking().ToArrayAsync();

        await response.WriteStringAsync(JsonSerializer.Serialize(customers));

        return response;
    }
}
