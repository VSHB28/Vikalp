using Vikalp.Models.DTO;

namespace Vikalp.Service.Interfaces
{
    public interface IHealthSystemService
    {
        Task<List<HealthSystemActivityDto>> GetAllAsync();
        //Task<(List<HealthSystemActivityDto> Data, int TotalCount)> GetPagedAsync(int userId, int pageNumber, int pageSize);
        Task<(List<HealthSystemActivityDto> Data, int TotalCount)> GetPagedAsync(int userId, int pageNumber, int pageSize, int? stateId, int? districtId, int? ActivityNameId);

        Task<List<DropdownDto>> SearchFacilitiesAsync(string term);
        Task<bool> SaveHealthSystemActivityJsonAsync(HealthSystemActivityDto model, int userId);
       // Task<List<HealthSystemParticipantDto>> GetAllParticipantAsync();
        Task<(List<HealthSystemParticipantDto> Data, int TotalCount)> GetAllParticipantAsync(int userId, int page, int pageSize, int? stateId, int? districtId, int? blockId, int? facilityId);

        Task<List<ParticipantListDto>> GetparticipantByFacilityAsync(int facilityId);
        Task<MstFacilityDto?> GetFacilityByIdAsync(int facilityId);
        Task<HealthSystemActivityDto?> GetByIdAsync(int id);





     








        Task<bool> SaveParticipantsAsync(DateTime dateOfActivity, int stateId, int? districtId, int? facilityTypeId, string? facilityTypeOther, List<HealthSystemParticipantDto> participants, int createdBy);
        Task<bool> UpdateAsync(HealthSystemActivityDto model, int userId);

        Task<HealthSystemParticipantSaveDto?> GetParticipantsByIdAsync(int id);

        Task<bool> UpdateParticipantsAsync(DateTime dateofActivity, int stateId, int? districtId, int? facilityTypeId, int? activityTypeId, string? facilityTypeOther, int? activityNameId, int? providerTypeId, string? remarks, List<HealthSystemParticipantDto> participants, int userId);

    }
}
