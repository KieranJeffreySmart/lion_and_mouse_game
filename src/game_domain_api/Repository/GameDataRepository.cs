using game_domain_api.GameContext;
using Microsoft.EntityFrameworkCore;

namespace game_domain_api.Repository
{
    public class GameDataRepository : IGameDataRepository
    {
        private readonly GameDbContext _context;

        public GameDataRepository(GameDbContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        public async Task<GameData?> GetByIdAsync(Guid id)
        {
            return await _context.GameData.FindAsync(id);
        }

        public async Task<IEnumerable<GameData>> GetAllAsync()
        {
            return await _context.GameData.ToListAsync();
        }

        public async Task<GameData> AddAsync(GameData gameData)
        {
            _context.GameData.Add(gameData);
            await _context.SaveChangesAsync();
            return gameData;
        }

        public async Task<GameData> UpdateAsync(GameData gameData)
        {
            _context.GameData.Update(gameData);
            await _context.SaveChangesAsync();
            return gameData;
        }

        public async Task DeleteAsync(Guid id)
        {
            var gameData = await _context.GameData.FindAsync(id);
            if (gameData != null)
            {
                _context.GameData.Remove(gameData);
                await _context.SaveChangesAsync();
            }
        }
    }

    public class GameDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<GameData>()
                .HasKey(i => i.Id);
        }

        public DbSet<GameData> GameData { get; set; }

        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
        }
    }

    public interface IGameDataRepository
    {
        Task<GameData?> GetByIdAsync(Guid id);
        Task<IEnumerable<GameData>> GetAllAsync();
        Task<GameData> AddAsync(GameData gameData);
        Task<GameData> UpdateAsync(GameData gameData);
        Task DeleteAsync(Guid id);
    }
}