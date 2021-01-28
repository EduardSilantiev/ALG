using ALG.Core.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ALG.EntityFrameworkCore.EntityFrameworkCore.Repositories
{
    public class ServicesRepository : IServicesRepository
    {
        private readonly AlgDbContext _context;

        public ServicesRepository(AlgDbContext context)
        {
            _context = context;
        }

        public IQueryable<Service> GetAllServicesAsync(Guid userId, string filter)
        {
            IQueryable<Service> services = _context.Services;

            if (filter != null && filter.Length > 0)
                services = services.Where(s => s.Name.Contains(filter));

            services = services.Include(service => service.ActivatedBonuses.Where(bonus => bonus.UserId == userId));
            return services;
        }

        public async Task<Service> GetServiceAsync(Guid userId, Guid serviceId) =>
            await _context.Services.Where(s => s.Id == serviceId)
                            .Include(service => service.ActivatedBonuses.Where(bonus => bonus.UserId == userId))
                            .FirstOrDefaultAsync();

        public async Task ActivateBonusAsync(ActivatedBonus activatedBonus)
        {
            _context.Add(activatedBonus);
            await _context.SaveChangesAsync();
        }
    }
}
