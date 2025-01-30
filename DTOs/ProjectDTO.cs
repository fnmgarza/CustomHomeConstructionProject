using CustomHomeConstructionProjects.Models;
using System.ComponentModel.DataAnnotations;

namespace CustomHomeConstructionProjects.DTOs
{
    public class ProjectDto
    {
        public required ProjectDetailsDto ProjectDetails { get; set; }
        public required ClientDto Client { get; set; }
    }
    public class ProjectDetailsDto
    {
        public int Id { get; set; }
        [Required, MinLength(3), MaxLength(100)]
        public required string ProjectName { get; set; }
        public ProjectStatusDto? Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EstimatedCompletionDate { get; set; }
        [Required, Range(1, double.MaxValue)]
        public decimal Budget { get; set; }
        public List<ProjectNoteDto>? Notes { get; set; }
        public string? PhotoUrl { get; set; }
    }
    public class ProjectStatusDto
    {
        public int Id { get; set; }
        public required string StatusName { get; set; }
        public required string Color { get; set; }
    }
    public class ProjectNoteDto
    {
        public int Id { get; set; }
        public string? NoteText { get; set; }
    }
    public class ClientDto
    {
        public int Id { get; set; }
        [Required, MinLength(3), MaxLength(100)]
        public required string ClientName { get; set; }
        public required AddressDto Address { get; set; }
        public List<ClientContactDto> ClientContacts { get; set; } = new List<ClientContactDto>();
    }

    public class AddressDto
    {
        public int Id { get; set; }
        [Required, MinLength(5), MaxLength(200)]
        public required string Street { get; set; }

        [Required, MinLength(2), MaxLength(50)]
        public required string City { get; set; }

        [Required, MinLength(2), MaxLength(50)]
        public required string State { get; set; }

        [Required, RegularExpression(@"^\d{5}$", ErrorMessage = "Invalid Zip Code.")]
        public required string ZipCode { get; set; }
    }

    public class ClientContactDto
    {
        public int Id { get; set; }
        [Required, MinLength(2), MaxLength(50)]
        public required string Name { get; set; }
        [Required, EmailAddress]
        public required string Email { get; set; }
        [Required, RegularExpression(@"^\d{10,15}$", ErrorMessage = "Invalid Phone Number.")]
        public required string Phone { get; set; }
    }
}
