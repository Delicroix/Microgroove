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

public class CustomersUpdate
{
    private readonly ILogger _logger;
    private readonly Context _context;

    public CustomersUpdate(Context context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger<CustomersUpdate>();
    }

    [Function(nameof(CustomersUpdate))]
    [OpenApiOperation(operationId: "CustomersGetByIdUpdateDelete", tags: new[] { "Customer" }, Summary = "Update an existing Customer.", Description = "Operation Update an existing Customer in database.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Customer), Required = true, Description = "Update an existing Customer in database.")]
    [OpenApiParameter("id", Description = "Id for the customer to be updated.", Type = typeof(Guid), Required = true, Visibility = OpenApiVisibilityType.Important)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NoContent, contentType: "application/json", bodyType: typeof(string), Summary = "Customer Updated.", Description = "Customer Updated.")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "customers/{id}")] HttpRequestData req,
            Guid id)
    {
        var response = req.CreateResponse(HttpStatusCode.NoContent);

        response.Headers.Add("Content-Type", "application/json; charset=utf-8");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        var customer = JsonSerializer.Deserialize<Customer>(requestBody);

        if (customer is null) return req.CreateResponse(HttpStatusCode.BadRequest);

        customer.CustomerId = id;

        _ = _context.Customers.Update(customer);

        _ = await _context.SaveChangesAsync();

        return response;
    }
}
