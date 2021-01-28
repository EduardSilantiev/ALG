using System;
using System.Linq;
using System.Threading.Tasks;

namespace ALG.Core.Services
{
    public interface IServicesRepository
    {
        IQueryable<Service> GetAllServicesAsync(Guid userId, string filter);
        Task<Service> GetServiceAsync(Guid userId, Guid serviceId);
        Task ActivateBonusAsync(ActivatedBonus activatedBonus);
    }
}
