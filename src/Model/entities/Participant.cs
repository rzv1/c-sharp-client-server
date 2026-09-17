using System.ComponentModel.DataAnnotations.Schema;

namespace Model; 

[Table("participant")]
public class Participant(string name, int age) : Entity<long>
{
    [Column("name")]
    public string Name { get; set; } = name;
    [Column("age")]
    public int Age { get; set; } = age;
    [NotMapped]
    public List<long>? Races { get; set; }
    public int RaceCount => Races!.Count;
    public override string ToString()
    {
        return Name + " " + Age + " Races: " + Races;
    }
}