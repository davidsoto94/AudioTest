using test.Database.Entities;

namespace test.Database.Repositories.Interfaces
{
    public interface ISingerRepository
    {
        public Task AddSingerWithNoTranckingAsync(Singer singer);

        public Task<List<Singer>> GetAllSingersWithNotTrackingAsync();
        public Task<List<Singer>> GetSingersList(List<int> Ids);
    }
}
