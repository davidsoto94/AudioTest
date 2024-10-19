using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.IO;
using test.Database.Repositories.Interfaces;
using test.Models.DTOs;

namespace test.Services;

public class AudioService (ILogger<AudioService> logger, IAmazonS3 amazonS3, IAudioRepository audioRepository)
{
    private readonly IAudioRepository _audioRepository = audioRepository;
    private readonly ILogger<AudioService> _logger = logger;
    private readonly IAmazonS3 _amazonS3 = amazonS3;

    public async Task<AudioGetDTO?> GetAudio(int id)
    {
        var audio = await _audioRepository.GetAudio(id);
        if (audio == null) return null;

        var bucket = Environment.GetEnvironmentVariable("AWS_BUCKET");
        if (bucket == null) throw new Exception("No bucket name");

        var audioGetDto = new AudioGetDTO
        {
            SingerId = audio.Singers.Select(s => s.Id).ToList(),
            Title = audio.Title,
            Album = audio.Album,
            Lirycs = audio.Lirycs,
            Year = audio.Year,
            AudioURL = GeneratePreSignedUrl(bucket, audio.AudioURL),
            ImageURL = audio.ImageURL == "" || audio.ImageURL == null ? null : GeneratePreSignedUrl(bucket, audio.ImageURL!)
        };

        return audioGetDto;

    }

    private string GeneratePreSignedUrl(string bucketName, string objectKey)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = objectKey,
            Expires = DateTime.UtcNow.AddHours(1) // Set expiration time as needed
        };

        return _amazonS3.GetPreSignedURL(request);
    }

}
