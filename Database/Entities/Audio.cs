using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test.Database.Entities
{
    [Table("audio")]
    public class Audio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("title")]
        public required string Title { get; set; }
        [Column("album")]
        public required string Album { get; set; }        
        [Column("lirycs")]
        public string? Lirycs { get; set; }
        [Column("year")]
        public int Year { get; set; }
        [Column("audio_url")]
        public string AudioURL { get; set; } = "";
        [Column("image_url")]
        public string? ImageURL { get; set; }
        public virtual List<Singer> Singers { get; set; } = new List<Singer>();
    }
}
