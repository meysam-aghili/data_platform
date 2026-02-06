using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models;

public class Base
{
    [Key]
    public long Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Slug { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required]
    public long CreatedById { get; set; }

    [Required]
    [Column(TypeName = "timestamp without time zone")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [Required]
    public long UpdatedById { get; set; }

    [ForeignKey(nameof(CreatedById))]
    public PanelUser CreatedByPanelUser {  get; set; } = null!;

    [ForeignKey(nameof(UpdatedById))]
    public PanelUser UpdatedByPanelUser { get; set; } = null!;
}
