using ALG.Core.Users;
using System.Linq;

namespace ALG.EntityFrameworkCore.EntityFrameworkCore.Seed
{
    public class UsersSeeds
    {
        private readonly AlgDbContext _context;

        public UsersSeeds(AlgDbContext context)
        {
            _context = context;
        }
        public void Seed()
        {
            var users = InitDefaults.GetInitialUsers();
            foreach (var user in users)
                AddUserIfNotExists(user);
        }

        private void AddUserIfNotExists(User user)
        {
            if (!_context.Users.Any(u => u.Email == user.Email))
            {
                _context.Users.Add(user);
                _context.SaveChanges();
            }
        }

    }
}
