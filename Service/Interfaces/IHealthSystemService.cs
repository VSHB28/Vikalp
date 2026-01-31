using Vikalp.Models.DTO;

namespace Vikalp.Service.Interfaces
{
    public interface IHealthSystemService
    {
        Task<List<HealthSystemActivityDto>> GetAllAsync();
        Task<List<DropdownDto>> SearchFacilitiesAsync(string term);
        Task<bool> SaveHealthSystemActivityJsonAsync(HealthSystemActivityDto model, int userId);
        Task<List<HealthSystemParticipantDto>> GetAllParticipantAsync();
        Task<bool> SaveHealthSystemParticipantJsonAsync(HealthSystemParticipantDto model, int userId);

        Task<List<ParticipantListDto>> GetparticipantByFacilityAsync(int facilityId);
        Task<MstFacilityDto?> GetFacilityByIdAsync(int facilityId);
    }
}
