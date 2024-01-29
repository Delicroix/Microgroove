using System.Text.Json.Serialization;

namespace Domain.Customers;

public class Customer
{
    [JsonPropertyName("customerId")]
    public Guid CustomerId { get; set; }
    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }
    [JsonPropertyName("dateOfBirth")]
    public DateOnly DateOfBirth { get; set; }
    public string? Avatar { get; set; }
}
