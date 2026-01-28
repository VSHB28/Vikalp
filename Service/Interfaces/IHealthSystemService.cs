using Vikalp.Models.DTO;

namespace Vikalp.Service.Interfaces
{
    public interface IHealthSystemService
    {
        Task<List<HealthSystemActivityDto>> GetAllAsync();
    }
}
