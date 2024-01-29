using Infrastructure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Function.Endpoints;

public class CustomersDelete
{
    private readonly Context _context;
    private readonly ILogger _logger;

    public CustomersDelete(Context context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger<CustomersDelete>();
    }

    [Function(nameof(CustomersDelete))]
    [OpenApiOperation(operationId: "CustomersDelete", tags: new[] { "Customer" }, Summary = "Delete a Customer by Id.", Description = "Operation delete a customer from database.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter("id", Description = "Id for the customer to be fetched.", Type = typeof(Guid), Required = true, Visibility = OpenApiVisibilityType.Important)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Summary = "Job list.", Description = "List of all the jobs.")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "customers/{id}")] HttpRequestData req,
            Guid id)
    {
        var response = req.CreateResponse(HttpStatusCode.NoContent);

        response.Headers.Add("Content-Type", "application/json; charset=utf-8");

        var customer = await _context.Customers.FirstOrDefaultAsync(p => p.CustomerId.Equals(id));

        if (customer is null) return req.CreateResponse(HttpStatusCode.Gone);

        _context.Remove(customer);

        var numberOfStateEntries = await _context.SaveChangesAsync();

        return response;
    }
}
