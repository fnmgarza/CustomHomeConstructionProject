using Microsoft.AspNetCore.Identity;

namespace CustomHomeConstructionProjects.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Project>? Projects { get; set; }
    }
}