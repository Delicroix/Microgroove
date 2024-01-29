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

public class CustomersGetById
{
    private readonly ILogger _logger;
    private readonly Context _context;

    public CustomersGetById(Context context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger<CustomersGetById>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="req"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [Function(nameof(CustomersGetById))]
    [OpenApiOperation(operationId: "CustomersGetById", tags: new[] { "Customer" }, Summary = "Get a Customer by Id.", Description = "Operation get a customer in database.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter("id", Description = "Id for the customer to be fetched.", Type = typeof(Guid), Required = true, Visibility = OpenApiVisibilityType.Important)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Summary = "Job list.", Description = "List of all the jobs.")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers/{id}")] HttpRequestData req,
            Guid id)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);

        response.Headers.Add("Content-Type", "application/json; charset=utf-8");

        var customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(p => p.CustomerId.Equals(id));

        if (customer is null) return req.CreateResponse(HttpStatusCode.NotFound);

        await response.WriteStringAsync(JsonSerializer.Serialize(customer));

        return response;

    }
}
