using CustomHomeConstructionProjects.Data;
using CustomHomeConstructionProjects.DTOs;
using CustomHomeConstructionProjects.Models;
using CustomHomeConstructionProjects.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomHomeConstructionProjects.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IClientService _clientService;

        public ClientsController(ApplicationDbContext context, IClientService clientService)
        {
            _context = context;
            _clientService = clientService;
        }
        [HttpPost("create-client")]
        public async Task<IActionResult> CreateClient([FromBody] ClientDto clientDto)
        {
            if (clientDto == null)
            {
                return BadRequest("Client data is required.");
            }

            try
            {
                var existingClient = await _clientService.GetClientAsync(clientDto);

                if (existingClient != null)
                {
                    return Conflict("Client already exists with this address.");
                }

                var client = await _clientService.CreateClientAsync(clientDto);
                return CreatedAtAction(nameof(CreateClient), new { id = client.Id }, client);
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("get-client/{clientName}")]
        public async Task<IActionResult> GetClient(string clientName)
        {
            var client = await _context.Clients
                .Include(c => c.Address)
                .Include(c => c.ClientContacts)
                .FirstOrDefaultAsync(c => c.ClientName == clientName);

            if (client == null)
            {
                return NotFound("Client not found.");
            }

            var clientResponse = new
            {
                client.Id,
                client.ClientName,
                Address = new
                {
                    client.Address.Street,
                    client.Address.City,
                    client.Address.State,
                    client.Address.ZipCode
                },
                Contacts = client.ClientContacts?.Select(contact => new
                {
                    contact.Id,
                    contact.ContactName,
                    contact.Email,
                    contact.PhoneNumber
                }).ToList()
            };

            return Ok(clientResponse);
        }
    }

}
