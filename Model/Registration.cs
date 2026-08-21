using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Model;

[Table("registration")]
public class Registration
{
    [Column("race")]
    public long RaceId { get; set; }
    [Column("participant")]
    public long ParticipantId { get; set; }
}