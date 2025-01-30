using CustomHomeConstructionProjects.DTOs;
using CustomHomeConstructionProjects.Models;

namespace CustomHomeConstructionProjects.Services
{
    public interface IAddressService
    {
        Task<Address?> GetAddressAsync(AddressDto addressDto);
        Task<Address> CreateAddressAsync(AddressDto addressDto);
    }
}
