using CustomHomeConstructionProjects.DTOs;
using CustomHomeConstructionProjects.Models;

namespace CustomHomeConstructionProjects.Services
{
    public interface IClientContactService
    {
        Task<ClientContact> AddClientContactAsync(int clientId, ClientContactDto contactDto);
        Task<List<ClientContact>> GetClientContactsAsync(int clientId);
        Task<ClientContact?> GetClientContactAsync(int clientId, ClientContactDto contactDto);
    }
}
