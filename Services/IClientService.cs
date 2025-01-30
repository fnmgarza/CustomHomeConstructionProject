using CustomHomeConstructionProjects.DTOs;
using CustomHomeConstructionProjects.Models;

namespace CustomHomeConstructionProjects.Services
{
    public interface IClientService
    {
        Task<Client?> GetClientAsync(ClientDto clientDto);
        Task<Client> CreateClientAsync(ClientDto clientDto);
        Task<Client?> GetOrCreateClientAsync(ClientDto clientDto);
    }
}
