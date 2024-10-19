using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test.Database.Entities;
using test.Database.Repositories.Interfaces;
using test.Models.DTOs;
using test.Services;

namespace TestsTest;

[TestClass]
public class AudioServiceTests
{
    private Mock<IAudioRepository> _audioRepositoryMock;
    private Mock<IAmazonS3> _amazonS3Mock;
    private Mock<ILogger<AudioService>> _loggerMock;
    private AudioService _audioService;

    public AudioServiceTests()
    {
        _audioRepositoryMock = new Mock<IAudioRepository>();
        _amazonS3Mock = new Mock<IAmazonS3>();
        _loggerMock = new Mock<ILogger<AudioService>>();

        _audioService = new AudioService(_loggerMock.Object, _amazonS3Mock.Object, _audioRepositoryMock.Object);
    }

    [TestMethod]
    public async Task GetAudio_ReturnsNull_WhenAudioNotFound()
    {
        // Arrange
        int audioId = 1;
        _audioRepositoryMock.Setup(repo => repo.GetAudio(audioId)).ReturnsAsync((Audio)null);

        // Act
        var result = await _audioService.GetAudio(audioId);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetAudio_ReturnsAudioGetDTO_WhenAudioFound()
    {
        // Arrange
        int audioId = 1;
        var audio = new Audio
        {
            Singers = new List<Singer> { new Singer { Id = 1 } },
            Title = "Test Title",
            Album = "Test Album",
            Lirycs = "Test Lyrics",
            Year = 2022,
            AudioURL = "test-audio-url",
            ImageURL = "test-image-url"
        };

        _audioRepositoryMock.Setup(repo => repo.GetAudio(audioId)).ReturnsAsync(audio);

        var bucketName = "test-bucket";
        Environment.SetEnvironmentVariable("AWS_BUCKET", bucketName);

        _amazonS3Mock.Setup(s3 => s3.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>()))
            .Returns("http://example.com/test-url");

        // Act
        var result = await _audioService.GetAudio(audioId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(audio.Title, result.Title);
        Assert.AreEqual(audio.Album, result.Album);
        Assert.AreEqual(audio.Year, result.Year);
        Assert.AreEqual("http://example.com/test-url", result.AudioURL);
        Assert.AreEqual("http://example.com/test-url", result.ImageURL);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "No bucket name")]
    public async Task GetAudio_ThrowsException_WhenBucketNameNotSet()
    {
        // Arrange
        int audioId = 1;
        var audio = new Audio
        {
            Singers = new List<Singer> { new Singer { Id = 1 } },
            Title = "Test Title",
            Album = "Test Album",
            Lirycs = "Test Lyrics",
            Year = 2022,
            AudioURL = "test-audio-url",
            ImageURL = "test-image-url"
        };

        _audioRepositoryMock.Setup(repo => repo.GetAudio(audioId)).ReturnsAsync(audio);

        // Clear the AWS_BUCKET variable to simulate it not being set
        Environment.SetEnvironmentVariable("AWS_BUCKET", null);

        // Act
        await _audioService.GetAudio(audioId);
    }
}
