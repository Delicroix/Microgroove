using Domain.Customers;
using Infrastructure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Function.Endpoints;

public class CustomersGetByIdUpdateDelete
{
    private readonly ILogger _logger;
    private readonly Context _context;

    public CustomersGetByIdUpdateDelete(Context context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger<CustomersGetByIdUpdateDelete>();
    }

    [Function("CustomersGetByIdUpdateDelete")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            Guid id)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        if (req.Method == "GET")
        {
            var customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(p => p.CustomerId.Equals(id));
            if (customer is null) return req.CreateResponse(HttpStatusCode.NotFound);

            await response.WriteStringAsync(JsonSerializer.Serialize(customer));
            return response;
        }

        else if (req.Method == "PUT")
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var customer = JsonSerializer.Deserialize<Customer>(requestBody);

            if (customer is null) return req.CreateResponse(HttpStatusCode.BadRequest);

            customer.CustomerId = id;
            _context.Customers.Update(customer);
            var numberOfStateEntries = await _context.SaveChangesAsync();
            await response.WriteStringAsync(JsonSerializer.Serialize(customer));
            return response;
        }

        else if (req.Method == "DELETE")
        {
            response = req.CreateResponse(HttpStatusCode.NoContent);

            var customer = await _context.Customers.FirstOrDefaultAsync(p => p.CustomerId.Equals(id));

            if (customer is null) return req.CreateResponse(HttpStatusCode.Gone);

            _context.Remove(customer);
            var numberOfStateEntries = await _context.SaveChangesAsync();

            await response.WriteStringAsync(JsonSerializer.Serialize(customer));
            return response;
        }

        return response;

    }
}
