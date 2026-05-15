using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoDashboard.Models;

public class DocumentShare
{
    [Key]
    public int DocumentShareId { get; set; }

    [Required]
    public int DocumentId { get; set; }

    [Required]
    public int SharedWithUserId { get; set; }

    [Required]
    public int SharedByUserId { get; set; }

    [Required]
    public DateTime ShareDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("DocumentId")]
    public virtual Document? Document { get; set; }

    [ForeignKey("SharedWithUserId")]
    public virtual User? SharedWithUser { get; set; }

    [ForeignKey("SharedByUserId")]
    public virtual User? SharedByUser { get; set; }
}