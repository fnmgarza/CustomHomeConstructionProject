using CustomHomeConstructionProjects.Data;
using CustomHomeConstructionProjects.DTOs;
using CustomHomeConstructionProjects.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomHomeConstructionProjects.Services
{
    public class ClientContactService : IClientContactService
    {
        private readonly ApplicationDbContext _context;

        public ClientContactService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<ClientContact>> GetClientContactsAsync(int clientId)
        {
            return await _context.ClientContacts
                .Where(c => c.ClientId == clientId)
                .ToListAsync();
        }

        public async Task<ClientContact?> GetClientContactAsync(int clientId, ClientContactDto contactDto)
        {
            return await _context.ClientContacts.FirstOrDefaultAsync(c =>
                c.ClientId == clientId &&
                c.ContactName == contactDto.Name &&
                c.Email == contactDto.Email &&
                c.PhoneNumber == contactDto.Phone);
        }

        public async Task<ClientContact> AddClientContactAsync(int clientId, ClientContactDto contactDto)
        {
            try
            {
                if (contactDto == null)
                {
                    throw new ArgumentNullException(nameof(contactDto), "Contact data is required.");
                }

                var existingClient = await _context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);

                if (existingClient == null)
                {
                    throw new InvalidOperationException("Client not found.");
                }

                var existingContact = await GetClientContactAsync(clientId, contactDto);
                if (existingContact != null)
                {
                    return existingContact;
                }

                var newContact = new ClientContact
                {
                    Client = existingClient,
                    ContactName = contactDto.Name,
                    Email = contactDto.Email,
                    PhoneNumber = contactDto.Phone,
                    ClientId = existingClient.Id
                };

                _context.ClientContacts.Add(newContact);
                await _context.SaveChangesAsync();
                return newContact;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }

}
