using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models;

public class City : Base
{
    public long StateId { get; set; }

    [ForeignKey(nameof(StateId))]
    public State State { get; set; } = null!;
};
