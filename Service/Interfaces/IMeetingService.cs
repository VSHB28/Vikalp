using Vikalp.Models.DTO;

namespace Vikalp.Service.Interfaces
{
    public interface IMeetingService
    {
        Task<(IEnumerable<MeetingDto> Data, int TotalCount)> GetAllAsync(
        int userId,
        int page,
        int pageSize,
        int? StateId,
        int? DistrictId,
        int? BlockId,
        int? FacilityId);
        Task<MeetingDto?> GetByIdAsync(int id);
        Task<bool> CreateAsync(MeetingDto dto, int userId);
        Task<bool> UpdateAsync(MeetingDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }

}
