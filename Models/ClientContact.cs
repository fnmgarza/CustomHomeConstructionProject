
namespace CustomHomeConstructionProjects.Models
{
    public class ClientContact
    {
        public int Id { get; set; }
        public int ClientId { get; set; } // Foreign Key
        public virtual Client? Client { get; set; } // Reference back to Client
        public required string ContactName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
