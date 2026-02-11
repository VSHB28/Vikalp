using System.Collections.Generic;
using System.Threading.Tasks;
using Vikalp.Models.DTO;

namespace Vikalp.Service.Interfaces
{
    public interface IHomeVisitService
    {
        Task<List<HomeVisitDTO>> GetAllAsync();
        Task<HomeVisitDTO?> GetByIdAsync(Guid linelistguid, int userId);
        Task<bool> InsertAsync(HomeVisitDTO model);
        Task<bool> UpdateAsync(HomeVisitDTO model);
        Task<bool> DeleteAsync(int visitId);

        Task<List<dynamic>> GetSubCentersAsync();
    }


}
