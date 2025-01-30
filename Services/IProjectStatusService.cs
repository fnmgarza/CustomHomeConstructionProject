using CustomHomeConstructionProjects.Models;

namespace CustomHomeConstructionProjects.Services
{
    public interface IProjectStatusService
    {
        Task<ProjectStatus?> GetStatusByNameAsync(string statusName);
        Task<ProjectStatus> CreateProjectStatusAsync(string statusName, string colorHex);
        Task<ProjectStatus> GetOrCreateProjectStatus(string statusName, string colorHex);

    }
}
