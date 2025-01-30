using CustomHomeConstructionProjects.DTOs;
using CustomHomeConstructionProjects.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace CustomHomeConstructionProjects.Services
{
    public interface IProjectService
    {
        Task<Project> CreateProjectAsync(ProjectDto projectDto, string userId, IDbContextTransaction transaction);
        Task<ProjectDto?> UpdateProjectAsync(int id, ProjectDto projectDto, string userId);
        Task<bool> DeleteProjectAsync(int id, string userId);
    }
}
