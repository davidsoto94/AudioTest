using Amazon.S3;
using test.Database.Repositories.Interfaces;
using test.Database.Repositories;
using test.Models.DTOs;
using test.Services.Interfaces;
using test.Database.Entities;
namespace test.Services;

public class S3StorageService (ILogger<S3StorageService> logger,IAmazonS3 amazonS3, IAudioRepository audioRepository, ISingerRepository singerRepository) : IStorageService
{
    private readonly ILogger<S3StorageService> _logger = logger;
    private readonly IAmazonS3 _amazonS3 = amazonS3;
    private readonly IAudioRepository _audioRepository = audioRepository;
    private readonly ISingerRepository _singerRepository = singerRepository;

    public async Task SaveFile(AudioPostDTO audioDTO)
    {
        if (audioDTO.AudioFile.Length == 0) throw new Exception("the file has no length");

        var singers = await _singerRepository.GetSingersList(audioDTO.SingerId);
        if (singers == null || singers.Count == 0) throw new Exception("The singers Id's are not valid");

        var bucket = Environment.GetEnvironmentVariable("AWS_BUCKET");

        if (bucket == null) throw new Exception("No bucket name");

        string audioPath = Guid.NewGuid() + audioDTO.AudioFile.FileName;

        string imagePath = audioDTO.ImageFile == null ? "" : new Guid() + audioDTO.ImageFile.FileName;

        using (var stream = new MemoryStream())
        {
            await audioDTO.AudioFile.CopyToAsync(stream);

            var putRequest = new Amazon.S3.Model.PutObjectRequest
            {
                BucketName = bucket,
                Key = audioPath,
                InputStream = stream
            };

            var response = await _amazonS3.PutObjectAsync(putRequest);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK) {
                _logger.LogError(response.ToString());
                throw new Exception("the file was not able to be uploaded, please try again");
            }
        }

        if (imagePath != "")
        {
            using (var stream = new MemoryStream())
            {
                await audioDTO.ImageFile!.CopyToAsync(stream);

                var putRequest = new Amazon.S3.Model.PutObjectRequest
                {
                    BucketName = bucket,
                    Key = imagePath,
                    InputStream = stream
                };

                var response = await _amazonS3.PutObjectAsync(putRequest);

                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError(response.ToString());
                }
            }
        }
        var audio = new Audio()
        {
            Album = audioDTO.Album,
            AudioURL = audioPath,
            ImageURL = imagePath,
            Lirycs = audioDTO.Lirycs,
            Title = audioDTO.Title,
            Year = audioDTO.Year,
            Singers = singers
        };
        await _audioRepository.SaveAudioData(audio);
    }
}
