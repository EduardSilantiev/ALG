using ALG.Core.Users;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ALG.EntityFrameworkCore.EntityFrameworkCore.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AlgDbContext _context;

        public UsersRepository(AlgDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmailAsync(string email) => 
            await _context.Users.SingleOrDefaultAsync(x => x.Email == email);
    }
}
