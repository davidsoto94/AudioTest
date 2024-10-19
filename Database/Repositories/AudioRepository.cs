using Microsoft.EntityFrameworkCore;
using test.Database.DbContexts;
using test.Database.Entities;
using test.Database.Repositories.Interfaces;

namespace test.Database.Repositories
{
    public class AudioRepository(AudioDbContext audioDbContext) : IAudioRepository
    {
        public async Task<Audio?> GetAudio(int id)
        {
            var response = await audioDbContext.Audios.Where(a => a.Id == id).Include(a => a.Singers).FirstOrDefaultAsync();            
            return response;
        }

        public async Task SaveAudioData(Audio audio)
        {
            await audioDbContext.Audios.AddAsync(audio);
            await audioDbContext.Audios.AddAsync(audio);
            await audioDbContext.SaveChangesAsync();
        }
    }
}
