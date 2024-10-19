namespace test.Models.DTOs;

public class AudioPostDTO
{
    public required List<int> SingerId { get; set; }
    public required string Title { get; set; }
    public required string Album { get; set; }
    public string? Lirycs { get; set; }
    public required int Year { get; set; }
    public required IFormFile AudioFile { get; set; }
    public IFormFile? ImageFile { get; set; }
}
