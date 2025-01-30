namespace CustomHomeConstructionProjects.Models
{
    public class ProjectNote
    {
        public int Id { get; set; }
        public int ProjectId { get; set; } // Foreign Key
        public required Project Project { get; set; }
        public string? NoteText { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
