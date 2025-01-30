using CustomHomeConstructionProjects.Data;
using CustomHomeConstructionProjects.DTOs;
using CustomHomeConstructionProjects.Models;
using CustomHomeConstructionProjects.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomHomeConstructionProjects.Controllers
{
    [ApiController]
    [Route("api/contacts")]
    public class ClientContactsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IClientContactService _clientContactService;

        public ClientContactsController(ApplicationDbContext context, IClientContactService clientContactService)
        {
            _context = context;
            _clientContactService = clientContactService;
        }

        [HttpPost("add-contact")]
        public async Task<IActionResult> AddClientContact(int clientId, [FromBody] ClientContactDto contactDto)
        {
            if (contactDto == null)
            {
                return BadRequest("Contact data is required.");
            }

            try
            {
                var newContact = await _clientContactService.AddClientContactAsync(clientId, contactDto);
                return CreatedAtAction(nameof(AddClientContact), new { id = newContact.Id }, newContact);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("get-contacts/{clientId}")]
        public async Task<IActionResult> GetClientContacts(int clientId)
        {
            var contacts = await _clientContactService.GetClientContactsAsync(clientId);

            if (!contacts.Any())
            {
                return NotFound("No contacts found for this client.");
            }

            return Ok(contacts);
        }

        [HttpPost("get-contact/{clientId}")]
        public async Task<IActionResult> GetClientContact(int clientId, [FromBody] ClientContactDto contactDto)
        {
            var contact = await _clientContactService.GetClientContactAsync(clientId, contactDto);
            if (contact == null)
            {
                return NotFound("Contact not found.");
            }
            return Ok(contact);
        }
    }

}
