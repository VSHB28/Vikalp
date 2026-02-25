using Vikalp.Models.DTO;

namespace Vikalp.Service.Interfaces
{
    public interface IEventActivityService
    {
        Task<(IEnumerable<EventActivityDto> Data, int TotalCount)> GetAllAsync(
        int userId,
        int page,
        int pageSize,
        int? StateId,
        int? DistrictId,
        int? BlockId,
        int? FacilityId,
        int? SubCenterId);
        Task<EventActivityDto?> GetByIdAsync(int id);
        Task<bool> CreateAsync(EventActivityDto dto, int userId);
        Task<bool> UpdateAsync(EventActivityDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }

}
