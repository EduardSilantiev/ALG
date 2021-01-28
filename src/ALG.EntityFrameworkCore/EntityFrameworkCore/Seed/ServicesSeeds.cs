using ALG.Core.Services;
using System.Linq;

namespace ALG.EntityFrameworkCore.EntityFrameworkCore.Seed
{
    public class ServicesSeeds
    {
        private readonly AlgDbContext _context;

        public ServicesSeeds(AlgDbContext context)
        {
            _context = context;
        }
        public void Seed()
        {
            var services = InitDefaults.GetInitialServices();
            foreach (var service in services)
                AddServiceIfNotExists(service);
        }

        private void AddServiceIfNotExists(Service service)
        {
            if (!_context.Services.Any(s => s.Name == service.Name))
            {
                _context.Services.Add(service);
                _context.SaveChanges();
            }
        }
    }
}
