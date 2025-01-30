using CustomHomeConstructionProjects.Data;
using CustomHomeConstructionProjects.DTOs;
using CustomHomeConstructionProjects.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomHomeConstructionProjects.Services
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAddressService _addressService;

        public ClientService(ApplicationDbContext context, IAddressService addressService, IClientContactService clientContactService)
        {
            _context = context;
            _addressService = addressService;
        }

        public async Task<Client?> GetOrCreateClientAsync(ClientDto clientDto)
        {
            var client = await GetClientAsync(clientDto);

            if (client == null)
            {
                client = await CreateClientAsync(clientDto);
            }

            return client;
        }

        public async Task<Client?> GetClientAsync(ClientDto clientDto)
        {
            var address = await _addressService.GetAddressAsync(clientDto.Address);

            if (address == null)
            {
                return null;
            }

            return await _context.Clients
                .FirstOrDefaultAsync(c => c.ClientName == clientDto.ClientName && c.AddressId == address.Id);
        }

        public async Task<Client> CreateClientAsync(ClientDto clientDto)
        {
            try
            {
                var address = await _addressService.GetAddressAsync(clientDto.Address);

                if (address == null)
                {
                    address = await _addressService.CreateAddressAsync(clientDto.Address);
                }

                var existingClient = await GetClientAsync(clientDto);

                if (existingClient != null)
                {
                    return existingClient;
                }

                var newClient = new Client
                {
                    ClientName = clientDto.ClientName,
                    AddressId = address.Id,
                    Address = address
                };

                _context.Clients.Add(newClient);
                await _context.SaveChangesAsync();

                return newClient;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }

}
