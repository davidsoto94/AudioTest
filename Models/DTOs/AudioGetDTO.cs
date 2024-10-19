namespace test.Models.DTOs;

public class AudioGetDTO
{
    public required List<int> SingerId { get; set; }
    public required string Title { get; set; }
    public required string Album { get; set; }
    public string? Lirycs { get; set; }
    public required int Year { get; set; }
    public required string AudioURL { get; set; }
    public string? ImageURL { get; set; }
}
