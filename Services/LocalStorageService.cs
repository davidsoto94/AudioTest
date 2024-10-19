using Microsoft.AspNetCore.Http;
using test.Database.Entities;
using test.Database.Repositories.Interfaces;
using test.Models.DTOs;
using test.Services.Interfaces;

namespace test.Services;

public class LocalStorageService (IAudioRepository audioRepository, ISingerRepository singerRepository): IStorageService
{
    private readonly IAudioRepository _audioRepository = audioRepository;
    private readonly ISingerRepository _singerRepository = singerRepository;
    public async Task SaveFile(AudioPostDTO audioDTO)
    {
        if (audioDTO.AudioFile.Length == 0) throw new Exception("the file has no length");
        var singers = await _singerRepository.GetSingersList(audioDTO.SingerId);

        if (singers.Count == 0) throw new Exception("The singers Id's are not valid");

        var filePath = Path.GetTempFileName();

        using (var stream = File.Create(filePath))
        {
            await audioDTO.AudioFile.CopyToAsync(stream);
        }

        var audio = new Audio()
        {
            Album = audioDTO.Album,
            AudioURL = filePath,
            ImageURL = "",
            Lirycs = audioDTO.Lirycs,
            Title = audioDTO.Title,
            Year = audioDTO.Year,
            Singers = singers
        };
        await _audioRepository.SaveAudioData(audio);
    }
}
