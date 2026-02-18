using Vikalp.Models.DTO;

namespace Vikalp.Service.Interfaces
{
    public interface IHealthSystemService
    {
        //Task<List<HealthSystemActivityDto>> GetAllAsync();
        Task<(List<HealthSystemActivityDto> Data, int TotalCount)> GetPagedAsync(int userId, int pageNumber, int pageSize);
        Task<List<DropdownDto>> SearchFacilitiesAsync(string term);
        Task<bool> SaveHealthSystemActivityJsonAsync(HealthSystemActivityDto model, int userId);
        Task<List<HealthSystemParticipantDto>> GetAllParticipantAsync();
        Task<List<ParticipantListDto>> GetparticipantByFacilityAsync(int facilityId);
        Task<MstFacilityDto?> GetFacilityByIdAsync(int facilityId);

        Task<bool> SaveParticipantsAsync(DateTime dateOfActivity, int stateId, int? districtId, int? facilityTypeId, string? facilityTypeOther, List<HealthSystemParticipantDto> participants, int createdBy
    );
    }
}
