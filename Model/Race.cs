using System.ComponentModel.DataAnnotations.Schema;

namespace Model; 

[Table("race")]
public class Race(string distance, string style) : Entity<long>
{
    [Column("distance")]
    public string Distance { get; set; } = distance;
    [Column("style")]
    public string Style { get; set; } = style;
    [NotMapped]
    public List<long> Participants { get; set; }
    public int ParticipantCount => Participants.Count;
    [NotMapped]
    public bool IsSelected { get; set; } = false;
    public override string ToString()
    {
        return Distance + " " + Style;
    }
}