using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using test.Database.Repositories.Interfaces;
using test.Models.DTOs;
using test.Database.Entities;
using test.Services;

namespace TestsTest;

[TestClass]
public class S3StorageServiceTests
{
    private Mock<IAmazonS3> _amazonS3Mock;
    private Mock<IAudioRepository> _audioRepositoryMock;
    private Mock<ISingerRepository> _singerRepositoryMock;
    private Mock<ILogger<S3StorageService>> _loggerMock;
    private S3StorageService _s3StorageService;

    public S3StorageServiceTests()
    {
        _amazonS3Mock = new Mock<IAmazonS3>();
        _audioRepositoryMock = new Mock<IAudioRepository>();
        _singerRepositoryMock = new Mock<ISingerRepository>();
        _loggerMock = new Mock<ILogger<S3StorageService>>();

        _s3StorageService = new S3StorageService(
            _loggerMock.Object,
            _amazonS3Mock.Object,
            _audioRepositoryMock.Object,
            _singerRepositoryMock.Object
        );
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "the file has no length")]
    public async Task SaveFile_ThrowsException_WhenAudioFileIsEmpty()
    {
        // Arrange
        var audioDTO = new AudioPostDTO
        {
            AudioFile = new Mock<IFormFile>().Object, // Simulate an empty IFormFile
            SingerId = new List<int> { 1 },
            Title = "Test Title",
            Album = "Test Album",
            Year = 2022
        };

        // Act
        await _s3StorageService.SaveFile(audioDTO);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "The singers Id's are not valid")]
    public async Task SaveFile_ThrowsException_WhenSingersAreInvalid()
    {
        // Arrange
        var audioFileMock = new Mock<IFormFile>();
        audioFileMock.Setup(f => f.Length).Returns(1);
        audioFileMock.Setup(f => f.FileName).Returns("audio.mp3");

        var audioDTO = new AudioPostDTO
        {
            AudioFile = audioFileMock.Object,
            SingerId = new List<int> { 1 },
            Title = "Test Title",
            Album = "Test Album",
            Year = 2022
        };

        _singerRepositoryMock.Setup(repo => repo.GetSingersList(audioDTO.SingerId)).ReturnsAsync(new List<Singer>());

        // Act
        await _s3StorageService.SaveFile(audioDTO);
    }

    [TestMethod]
    public async Task SaveFile_SavesAudioAndImage_WhenValidInput()
    {
        // Arrange
        var audioFileMock = new Mock<IFormFile>();
        var imageFileMock = new Mock<IFormFile>();

        audioFileMock.Setup(f => f.Length).Returns(1);
        audioFileMock.Setup(f => f.FileName).Returns("audio.mp3");
        imageFileMock.Setup(f => f.Length).Returns(1);
        imageFileMock.Setup(f => f.FileName).Returns("image.jpg");

        var audioDTO = new AudioPostDTO
        {
            AudioFile = audioFileMock.Object,
            ImageFile = imageFileMock.Object,
            SingerId = new List<int> { 1 },
            Album = "Test Album",
            Lirycs = "Test Lyrics",
            Title = "Test Title",
            Year = 2022
        };

        _singerRepositoryMock.Setup(repo => repo.GetSingersList(audioDTO.SingerId)).ReturnsAsync(new List<Singer> { new Singer() });
        Environment.SetEnvironmentVariable("AWS_BUCKET", "test-bucket");

        _amazonS3Mock.Setup(s3 => s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), default))
            .ReturnsAsync(new PutObjectResponse { HttpStatusCode = System.Net.HttpStatusCode.OK });

        // Act
        await _s3StorageService.SaveFile(audioDTO);

        // Assert
        _audioRepositoryMock.Verify(repo => repo.SaveAudioData(It.IsAny<Audio>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "the file was not able to be uploaded, please try again")]
    public async Task SaveFile_ThrowsException_WhenAudioUploadFails()
    {
        // Arrange
        var audioFileMock = new Mock<IFormFile>();
        audioFileMock.Setup(f => f.Length).Returns(1);
        audioFileMock.Setup(f => f.FileName).Returns("audio.mp3");

        var audioDTO = new AudioPostDTO
        {
            AudioFile = audioFileMock.Object,
            SingerId = new List<int> { 1 },
            Title = "Test Title",
            Album = "Test Album",
            Year = 2022
        };

        _singerRepositoryMock.Setup(repo => repo.GetSingersList(audioDTO.SingerId)).ReturnsAsync(new List<Singer> { new Singer() });
        Environment.SetEnvironmentVariable("AWS_BUCKET", "test-bucket");

        _amazonS3Mock.Setup(s3 => s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), default))
            .ReturnsAsync(new PutObjectResponse { HttpStatusCode = System.Net.HttpStatusCode.BadRequest });

        // Act
        await _s3StorageService.SaveFile(audioDTO);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "the file has no length")]
    public async Task SaveFile_ThrowsException_WhenTitleIsEmpty()
    {
        // Arrange
        var audioFileMock = new Mock<IFormFile>();
        audioFileMock.Setup(f => f.Length).Returns(1);
        audioFileMock.Setup(f => f.FileName).Returns("audio.mp3");

        var audioDTO = new AudioPostDTO
        {
            Title = "testTitle",
            AudioFile = audioFileMock.Object,
            SingerId = new List<int> { 1 },
            Album = "Test Album",
            Year = 2022
            // Title is missing
        };

        // Act
        await _s3StorageService.SaveFile(audioDTO);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "The singers Id's are not valid")]
    public async Task SaveFile_ThrowsException_WhenAlbumIsEmpty()
    {
        // Arrange
        var audioFileMock = new Mock<IFormFile>();
        audioFileMock.Setup(f => f.Length).Returns(1);
        audioFileMock.Setup(f => f.FileName).Returns("audio.mp3");

        var audioDTO = new AudioPostDTO
        {
            AudioFile = audioFileMock.Object,
            SingerId = new List<int> { 1 },
            Title = "Test Title",
            Year = 2022,
            Album = "testAlbum"
        };

        // Act
        await _s3StorageService.SaveFile(audioDTO);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "the file has no length")]
    public async Task SaveFile_ThrowsException_WhenYearIsZero()
    {
        // Arrange
        var audioFileMock = new Mock<IFormFile>();
        audioFileMock.Setup(f => f.Length).Returns(0);
        audioFileMock.Setup(f => f.FileName).Returns("audio.mp3");

        var audioDTO = new AudioPostDTO
        {
            AudioFile = audioFileMock.Object,
            SingerId = new List<int> { 1 },
            Title = "Test Title",
            Album = "Test Album",
            Year = 0 // Invalid year
        };

        // Act
        await _s3StorageService.SaveFile(audioDTO);
    }
}
