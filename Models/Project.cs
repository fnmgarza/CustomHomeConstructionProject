using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace CustomHomeConstructionProjects.Models
{
    public class Project
    {
        public int Id { get; set; }
        public required string UserId { get; set; } // Foreign key
        public required ApplicationUser User { get; set; }
        public required string ProjectName { get; set; }

        public int ClientId { get; set; }  // Foreign Key
        public required Client Client { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EstimatedCompletionDate { get; set; }

        public int StatusId { get; set; }  // Foreign Key
        public required ProjectStatus Status { get; set; }

        [Precision(18, 2)]
        public decimal Budget { get; set; }

        public List<ProjectNote>? Notes { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
