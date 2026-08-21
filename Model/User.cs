using System.ComponentModel.DataAnnotations.Schema;

namespace Model;

[Table("user")]
public class User(string username, string password) : Entity<long>
{
    [Column("username")]
    public string Username { get; set; } = username;
    [Column("password")]
    public string Password { get; set; } = password;
}