using System.Collections.Generic;
using System.Threading.Tasks;
using Vikalp.Models.DTO;

namespace Vikalp.Service.Interfaces
{
    public interface IHomeVisitService
    {
        Task<(List<HomeVisitDTO> Data, int TotalCount)> GetAllAsync(int userId, int page, int pageSize, int? stateId, int? districtId, int? blockId, int? facilityId, int? subcentreId);
        Task<HomeVisitDTO?> GetByIdAsync(Guid linelistguid, int userId);
        Task<bool> DeleteAsync(int visitId);

        Task<List<dynamic>> GetSubCentersAsync();
        Task<List<HomevisitFollowUpDto>> GetFollowUpHistoryAsync(Guid linelistguid, int userId);
        Task<bool> InsertFollowUpAsync(HomevisitFollowUpDto model);
        Task<bool> SaveHomeVisitAsync(HomeVisitDTO model, int userId);
        Task<bool> UpdateHomeVisitAsync(HomeVisitDTO model, int userId);

    }

}
