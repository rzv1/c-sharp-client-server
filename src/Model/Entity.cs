using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model; 

public class Entity<T>
{
    [Key]
    [Column("id")]
    public T? Id { get; set; }
}