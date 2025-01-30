using CustomHomeConstructionProjects.Data;
using CustomHomeConstructionProjects.DTOs;
using CustomHomeConstructionProjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Runtime.CompilerServices;

namespace CustomHomeConstructionProjects.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IClientService _clientService;
        private readonly IProjectStatusService _projectStatusService;

        public ProjectService(ApplicationDbContext context, IClientService clientService, IProjectStatusService projectStatusService)
        {
            _context = context;
            _clientService = clientService;
            _projectStatusService = projectStatusService;
        }
        public async Task<Project> CreateProjectAsync(ProjectDto projectDto, string userId, IDbContextTransaction transaction)
        {
            var client = await _clientService.GetOrCreateClientAsync(projectDto.Client);

            if (client == null)
            {
                throw new InvalidOperationException("Failed to create or retrieve client.");
            }

            if (projectDto.ProjectDetails.Status == null)
            {
                projectDto.ProjectDetails.Status = new ProjectStatusDto
                {
                    StatusName = "Not Started",
                    Color = "#f39c12"
                };
            }

            if (string.IsNullOrEmpty(projectDto.ProjectDetails.Status.StatusName))
            {
                projectDto.ProjectDetails.Status.StatusName = "Not Started";
                projectDto.ProjectDetails.Status.Color = "#f39c12";
            }

            var projectStatus = await _projectStatusService.GetOrCreateProjectStatus(projectDto.ProjectDetails.Status.StatusName, projectDto.ProjectDetails.Status.Color);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            try
            {
                var newProject = new Project
                {
                    User = user,
                    UserId = userId,
                    ProjectName = projectDto.ProjectDetails.ProjectName,
                    StartDate = projectDto.ProjectDetails.StartDate,
                    EstimatedCompletionDate = projectDto.ProjectDetails.EstimatedCompletionDate,
                    Budget = projectDto.ProjectDetails.Budget,
                    ClientId = client.Id,
                    Client = client,
                    StatusId = projectStatus.Id,
                    Status = projectStatus,
                };

                _context.Projects.Add(newProject);
                await _context.SaveChangesAsync();

                if (projectDto.ProjectDetails.Notes != null && projectDto.ProjectDetails.Notes.Any())
                {
                    await AddNotesToProjectAsync(newProject, projectDto.ProjectDetails.Notes);
                }

                return newProject;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        private async Task AddNotesToProjectAsync(Project project, List<ProjectNoteDto> notes)
        {
            if (notes == null || !notes.Any()) return;

            var noteEntities = notes.Select(noteText => new ProjectNote
            {
                Project = project,
                ProjectId = project.Id,
                NoteText = noteText.NoteText
            }).ToList();

            _context.ProjectNotes.AddRange(noteEntities);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteProjectAsync(int id, string userId)
        {
            try
            {
                var project = await _context.Projects.Where(x => x.Id == id && x.UserId == userId).FirstAsync();

                if (project == null)
                {
                    return false;
                }

                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
        public async Task<ProjectDto?> UpdateProjectAsync(int id, ProjectDto projectDto, string userId)
        {
            var project = await _context.Projects
                .Include(p => p.Notes)
                .Include(p => p.Client)
                .ThenInclude(c => c.Address)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null || project.UserId != userId)
            {
                return null;
            }

            project.ProjectName = projectDto.ProjectDetails.ProjectName;
            project.StartDate = projectDto.ProjectDetails.StartDate;
            project.EstimatedCompletionDate = projectDto.ProjectDetails.EstimatedCompletionDate;
            project.Budget = projectDto.ProjectDetails.Budget;

            if (projectDto.ProjectDetails.Status != null)
            {
                var existingStatus = await _context.ProjectStatuses
                    .FirstOrDefaultAsync(s => s.Id == projectDto.ProjectDetails.Status.Id);

                if (existingStatus != null)
                {
                    project.StatusId = existingStatus.Id;
                }
            }

            if (project.Notes != null && projectDto.ProjectDetails.Notes != null)
            {
                project.Notes.Clear();
                foreach (var noteDto in projectDto.ProjectDetails.Notes)
                {
                    var newNote = new ProjectNote
                    {
                        Id = noteDto.Id,
                        NoteText = noteDto.NoteText,
                        ProjectId = project.Id,
                        Project = project
                    };
                    project.Notes.Add(newNote);
                }
            }
            
            if (projectDto.Client != null)
            {
                project.Client.ClientName = projectDto.Client.ClientName;
                project.Client.Address.Street = projectDto.Client.Address.Street;
                project.Client.Address.City = projectDto.Client.Address.City;
                project.Client.Address.State = projectDto.Client.Address.State;
                project.Client.Address.ZipCode = projectDto.Client.Address.ZipCode;
            }

            await _context.SaveChangesAsync();

            var updatedProjectDto = new ProjectDto
            {
                ProjectDetails = new ProjectDetailsDto
                {
                    Id = project.Id,
                    ProjectName = project.ProjectName,
                    StartDate = project.StartDate,
                    EstimatedCompletionDate = project.EstimatedCompletionDate,
                    Budget = project.Budget,
                    Status = project.Status != null ? new ProjectStatusDto
                    {
                        Id = project.Status.Id,
                        StatusName = project.Status.StatusName,
                        Color = project.Status.ColorHex
                    } : null,
                    Notes = project.Notes?.Select(n => new ProjectNoteDto
                    {
                        Id = n.Id,
                        NoteText = n.NoteText
                    }).ToList()
                },
                Client = new ClientDto
                {
                    ClientName = project.Client.ClientName,
                    Address = new AddressDto
                    {
                        Street = project.Client.Address.Street,
                        City = project.Client.Address.City,
                        State = project.Client.Address.State,
                        ZipCode = project.Client.Address.ZipCode
                    }
                }
            };

            return updatedProjectDto;
        }
    }
}
