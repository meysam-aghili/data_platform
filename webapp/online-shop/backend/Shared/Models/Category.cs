using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models;

public class Category: Base
{
    [Required]
    public int Level { get; set; }
    
    public long? ParentId { get; set; }

    [ForeignKey(nameof(ParentId))]
    public Category? ParentCategory { get; set; }
}
