
namespace CustomHomeConstructionProjects.Models
{
    public class Client
    {
        public int Id { get; set; }
        public required string ClientName { get; set; }
        public int AddressId { get; set; }  // Foreign Key
        public required Address Address { get; set; }

        // Navigation for related contacts
        public virtual ICollection<ClientContact> ClientContacts { get; set; } = new List<ClientContact>();
    }
}
