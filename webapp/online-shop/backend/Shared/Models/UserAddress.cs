using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Shared.Models;

public class UserAddress : Base
{
    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    public long CityId { get; set; }

    [Required]
    public bool IsDeleted { get; set; } = false;

    [Required]
    public long UserId { get; set; }

    [ForeignKey(nameof(CityId))]
    public City City { get; set; } = null!;
}
