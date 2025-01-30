using CustomHomeConstructionProjects.Data;
using CustomHomeConstructionProjects.DTOs;
using CustomHomeConstructionProjects.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomHomeConstructionProjects.Services
{
    public class ProjectStatusService : IProjectStatusService
    {
        private readonly ApplicationDbContext _context;

        public ProjectStatusService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ProjectStatus> GetOrCreateProjectStatus(string statusName, string colorHex)
        {
            var projectStatus = await GetStatusByNameAsync(statusName);

            if (projectStatus == null)
            {
                projectStatus = await CreateProjectStatusAsync(statusName, colorHex);

            }

            return projectStatus;
        }
        public async Task<ProjectStatus?> GetStatusByNameAsync(string statusName)
        {
            return await _context.ProjectStatuses
                .FirstOrDefaultAsync(s => s.StatusName == statusName);
        }
        public async Task<ProjectStatus> CreateProjectStatusAsync(string statusName, string colorHex)
        {
            try
            {
                var existingStatus = await GetStatusByNameAsync(statusName);

                if (existingStatus != null)
                {
                    return existingStatus;
                }

                var newStatus = new ProjectStatus 
                { 
                    StatusName = statusName,
                    ColorHex = colorHex
                };

                _context.ProjectStatuses.Add(newStatus);
                await _context.SaveChangesAsync();

                return newStatus;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }

}
