using ALG.EntityFrameworkCore.EntityFrameworkCore.Seed;
using Microsoft.EntityFrameworkCore;

namespace ALG.EntityFrameworkCore
{
    public class DbInitializer
    {
        private readonly AlgDbContext _context;

        public DbInitializer(AlgDbContext context)
        {
            _context = context;
        }

        public void InitDatabase()
        {
            MigrateDatabase();
            SeedDatabase();
        }

        private void MigrateDatabase()
        {
            _context.Database.EnsureCreated();
            _context.Database.Migrate();
        }

        private void SeedDatabase()
        {
            new UsersSeeds(_context).Seed();
            new ServicesSeeds(_context).Seed();
        }
    }
}
