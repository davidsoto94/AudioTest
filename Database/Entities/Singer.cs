using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace test.Database.Entities
{
    [Table("singer")]
    public class Singer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; } = "";
        [JsonIgnore]
        public virtual List<Audio> Audios { get; set; } = [];
    }
}
