using CustomHomeConstructionProjects.Data;
using CustomHomeConstructionProjects.DTOs;
using CustomHomeConstructionProjects.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomHomeConstructionProjects.Services
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationDbContext _context;

        public AddressService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Address?> GetAddressAsync(AddressDto addressDto)
        {
            return await _context.Addresses.FirstOrDefaultAsync(a =>
                a.Street == addressDto.Street &&
                a.City == addressDto.City &&
                a.State == addressDto.State &&
                a.ZipCode == addressDto.ZipCode);
        }
        public async Task<Address> CreateAddressAsync(AddressDto addressDto)
        {
            try
            {
                var existingAddress = await GetAddressAsync(addressDto);

                if (existingAddress != null)
                {
                    return existingAddress;
                }

                Address newAddress = new()
                {
                    Street = addressDto.Street,
                    City = addressDto.City,
                    State = addressDto.State,
                    ZipCode = addressDto.ZipCode
                };

                _context.Addresses.Add(newAddress);
                await _context.SaveChangesAsync();
                return newAddress;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }
}
