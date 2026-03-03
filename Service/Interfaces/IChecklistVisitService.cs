using Vikalp.Models.DTO;

namespace Vikalp.Service.Interfaces
{
    public interface IChecklistVisitService
    {
        Task<(IEnumerable<ChecklistVisitDTO> Data, int TotalCount)> GetAllAsync(
        int userId,
        int page,
        int pageSize,
        int? StateId,
        int? DistrictId,
        int? BlockId,
        int? FacilityId,
        int? SubCenterId);
        Task<ChecklistVisitDTO?> GetByIdAsync(int id);
        Task<bool> CreateAsync(ChecklistVisitDTO dto, int userId);
        Task<bool> UpdateAsync(ChecklistVisitDTO dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
