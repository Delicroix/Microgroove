namespace Application.DTOs
{
    public class CustomerDto
    {
        public Guid CustomerId { get; set; }
        public string? FullName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? Avatar { get; set; }

    }
}
