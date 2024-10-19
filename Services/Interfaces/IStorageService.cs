using test.Models.DTOs;

namespace test.Services.Interfaces
{
    public interface IStorageService
    {
        public Task SaveFile(AudioPostDTO audioDTO);
    }
}
