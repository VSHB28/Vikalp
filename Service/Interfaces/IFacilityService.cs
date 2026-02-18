using Vikalp.Models.DTO;

namespace Vikalp.Service.Interfaces
{
    public interface IFacilityService
    {
        Task<IEnumerable<FacilityDto>> GetByBlockAsync(int blockId);
        Task<FacilityDto?> GetByIdAsync(int id);
        Task AddAsync(FacilityDto dto);
        Task UpdateAsync(FacilityDto dto);
        Task DeleteAsync(int id);
        Task SaveFacilityProfileAsync(FacilityProfileDto model);
        Task<FacilityProfileDto> GetFacilityProfileAsync(int profileId);
        Task SaveHrStatusAsync(HrStatusDto model);
        Task<HrStatusDto> GetHrStatusAsync(int hrId);

    }
}
