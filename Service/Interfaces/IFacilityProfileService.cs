using Vikalp.Models.DTO;

namespace Vikalp.Service.Interfaces
{
    public interface IFacilityProfileService
    {
        Task<List<HealthSystemActivityDto>> GetAllAsync();
    }
}
