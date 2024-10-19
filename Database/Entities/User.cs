using System.ComponentModel.DataAnnotations.Schema;

namespace test.Database.Entities
{
    [Table("user")]
    public record User(Guid Id, string Name, string Email, string Password, string[] Roles);
}
