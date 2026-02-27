using Vikalp.Models.DTO;

namespace Vikalp.Service.Interfaces
{
    public interface IHealthSystemService
    {
        //Task<List<HealthSystemActivityDto>> GetAllAsync();
        Task<(List<HealthSystemActivityDto> Data, int TotalCount)> GetPagedAsync(int userId, int pageNumber, int pageSize, int? stateId, int? districtId, int? ActivityNameId);

        Task<HealthSystemActivityDto> GetByIdAsync(int userId, int ActivityId);
        Task<List<DropdownDto>> SearchFacilitiesAsync(string term);
        Task<bool> SaveHealthSystemActivityJsonAsync(HealthSystemActivityDto model, int userId);
        Task<List<HealthSystemParticipantDto>> GetAllParticipantAsync();
        Task<(List<HealthSystemParticipantDto> Data, int TotalCount)> GetAllParticipantAsync(int userId, int page, int pageSize, int? stateId, int? districtId, int? blockId, int? facilityId);
        Task<List<ParticipantListDto>> GetparticipantByFacilityAsync(int facilityId);
        Task<MstFacilityDto?> GetFacilityByIdAsync(int facilityId);

        Task<bool> SaveParticipantsAsync(DateTime dateOfActivity, int stateId, int? districtId, int? facilityTypeId, string? facilityTypeOther, List<HealthSystemParticipantDto> participants, int createdBy);
    }
}
