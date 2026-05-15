using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoDashboard.Models;

public enum DocumentCategory
{
    [Display(Name = "Project Documents")]
    ProjectDocuments,
    [Display(Name = "Team Resources")]
    TeamResources,
    [Display(Name = "Personal Files")]
    PersonalFiles,
    [Display(Name = "Reports")]
    Reports,
    [Display(Name = "Presentations")]
    Presentations,
    [Display(Name = "Other")]
    Other
}

public class Document
{
    [Key]
    public int DocumentId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public DocumentCategory Category { get; set; }

    public int? ProjectId { get; set; }

    [MaxLength(500)]
    public string? Tags { get; set; }

    [Required]
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    [Required]
    public int UploaderId { get; set; }

    [Required]
    public long FileSize { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileType { get; set; } = string.Empty;

    [Required]
    public string FilePath { get; set; } = string.Empty;

    // Navigation properties
    [ForeignKey("UploaderId")]
    public virtual User? Uploader { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project? Project { get; set; }

    public virtual ICollection<DocumentShare> DocumentShares { get; set; } = new List<DocumentShare>();
}