using test.Database.Entities;

namespace test.Database.Repositories.Interfaces
{
    public interface IAudioRepository
    {
        public Task<Audio?> GetAudio(int id);
        public Task SaveAudioData(Audio audio);
    }
}
