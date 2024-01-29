using Domain.Customers;
using Infrastructure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Function.Endpoints;

public class CustomersCreate
{
    private readonly Context _context;
    private readonly ILogger _logger;
    private readonly HttpClient? _httpClient;

    public CustomersCreate(Context context, ILoggerFactory loggerFactory)//, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger<CustomersCreate>();
        //_httpClient = httpClientFactory.CreateClient();
    }

    [Function(nameof(CustomersCreate))]
    [OpenApiOperation(operationId: "CustomersCreate", tags: new[] { "Customer" }, Summary = "Create a new Customer.", Description = "Operation create a new customer in database.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Customer), Required = true, Description = "create customer in database.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(string), Summary = "Job list.", Description = "List of all the jobs.")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "customers")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.Created);

        response.Headers.Add("Content-Type", "application/json; charset=utf-8");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        var customer = JsonSerializer.Deserialize<Customer>(requestBody);

        customer.CustomerId = Guid.NewGuid();

        _context.Customers.Add(customer);

        var returnValue = await _context.SaveChangesAsync();

        if (false)//returnValue > 0)
        {
            var content = new StringContent(JsonSerializer.Serialize(new { name = $"{customer.FullName}", format = "svg" }));

            //https://ui-avatars.com/api/?name=John+Doe&format=svg
            var avitarResponse = await _httpClient.PostAsync("https://ui-avatars.com/api/", content);

            string responseBody = await avitarResponse.Content.ReadAsStringAsync();

            customer.Avatar = responseBody;

            _context.Customers.Update(customer);

            var numberOfStateEntries = await _context.SaveChangesAsync();
        }

        await response.WriteStringAsync(JsonSerializer.Serialize(customer));

        return response;
    }
}
