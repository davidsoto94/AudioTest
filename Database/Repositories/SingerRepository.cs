using Microsoft.EntityFrameworkCore;
using test.Database.DbContexts;
using test.Database.Entities;
using test.Database.Repositories.Interfaces;

namespace test.Database.Repositories
{
    public class SingerRepository (AudioDbContext audioDbContext): ISingerRepository
    {
        public async Task AddSingerWithNoTranckingAsync(Singer singer)
        {
            await audioDbContext.Singers.AddAsync(singer);
            await audioDbContext.SaveChangesAsync();
        }

        public async Task<List<Singer>> GetAllSingersWithNotTrackingAsync()
        {
            return await audioDbContext.Singers.ToListAsync();
        }

        public async Task<List<Singer>> GetSingersList(List<int> Ids)
        {
            return await audioDbContext.Singers.Where(s => Ids.Contains(s.Id)).Include(s => s.Audios).ToListAsync();
        }
    }
}
