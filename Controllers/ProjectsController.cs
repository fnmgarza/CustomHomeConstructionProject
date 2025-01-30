using CustomHomeConstructionProjects.Data;
using CustomHomeConstructionProjects.DTOs;
using CustomHomeConstructionProjects.Models;
using CustomHomeConstructionProjects.Services;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomHomeConstructionProjects.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IProjectService _projectService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IClientContactService _clientContactService;

        public ProjectsController(ApplicationDbContext context, IProjectService projectService, IAuthorizationService authorizationService, IClientContactService clientContactService)
        {
            _context = context;
            _projectService = projectService;
            _authorizationService = authorizationService;
            _clientContactService = clientContactService;
        }
        [HttpGet("statuses")]
        public async Task<ActionResult<IEnumerable<ProjectStatusDto>>> GetProjectStatuses()
        {
            var statuses = await _context.ProjectStatuses
                .Select(s => new ProjectStatusDto
                {
                    Id = s.Id,
                    StatusName = s.StatusName,
                    Color = s.ColorHex
                })
                .ToListAsync();

            return Ok(statuses);
        }
        [HttpGet("get-projects")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            if (User == null)
            {
                throw new UnauthorizedAccessException();
            }

            string? userId = User.Identity.GetSubjectId();

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException();
            }

            var projects = await _context.Projects
                .Where(x => x.UserId == userId)
                .Include(p => p.Client)
                    .ThenInclude(c => c.ClientContacts)
                .Include(p => p.Status)
                .Include(p => p.Notes)
                .Select(p => new ProjectDto
                {
                    ProjectDetails = new ProjectDetailsDto
                    {
                        Id = p.Id,
                        ProjectName = p.ProjectName,
                        Status = new ProjectStatusDto
                        {
                            Id = p.Status.Id,
                            StatusName = p.Status.StatusName,
                            Color = p.Status.ColorHex
                        },
                        StartDate = p.StartDate,
                        EstimatedCompletionDate = p.EstimatedCompletionDate,
                        Budget = p.Budget,
                        PhotoUrl = p.PhotoUrl,
                        Notes = p.Notes != null ? 
                            p.Notes.Select(n => new ProjectNoteDto 
                            {
                                NoteText = n.NoteText 
                            }).ToList() 
                            : new List<ProjectNoteDto>()
                    },
                    Client = new ClientDto
                    {
                        Id = p.ClientId,
                        ClientName = p.Client.ClientName,
                        Address = new AddressDto
                        {
                            Id = p.Client.AddressId,
                            Street = p.Client.Address.Street,
                            City = p.Client.Address.City,
                            State = p.Client.Address.State,
                            ZipCode = p.Client.Address.ZipCode,
                        }
                    },
                })
                .ToListAsync();

            if (projects == null || projects.Count == 0)
            {
                return Ok(new List<ProjectDto>());
            }

            return Ok(projects);
        }


        // Search projects by name or client
        [HttpGet("search")]
        public async Task<IActionResult> SearchProjects([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
                return BadRequest("Query parameter is required.");

            var projects = await _context.Projects
                .Where(p => p.ProjectName.Contains(query))
                .ToListAsync();

            return Ok(projects);
        }

        // Update a project
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectDto projectDto)
        {
            string userId = _authorizationService.GetUserId();

            if (userId == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            if (id != projectDto.ProjectDetails.Id)
            {
                return BadRequest("Project ID mismatch.");
            }

            var result = await _projectService.UpdateProjectAsync(id, projectDto, userId);

            if (result == null)
            {
                return NotFound("Project not found or unauthorized.");
            }

            return Ok(result);
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            string userId = _authorizationService.GetUserId();

            if (userId == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var result = await _projectService.DeleteProjectAsync(id, userId);

            if (!result)
            {
                return NotFound("Project not found or unauthorized.");
            }

            return NoContent();
        }

        [HttpPost("create-project")]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDto projectDto)
        {
            if (projectDto == null)
            {
                return BadRequest("Project data is required.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                string userId = _authorizationService.GetUserId();

                if (userId == null)
                {
                    return BadRequest();
                }

                var project = await _projectService.CreateProjectAsync(projectDto, userId, transaction);
                await transaction.CommitAsync();

                return Ok(project);
            }
            catch (ArgumentException ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
