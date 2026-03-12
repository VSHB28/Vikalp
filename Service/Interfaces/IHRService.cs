using Vikalp.Models.DTO;
using System.Threading.Tasks;

namespace Vikalp.Service.Interfaces
{
    public interface IHRService
    {
        Task<HrStatusDto> GetHrStatusAsync(int hrId);
        Task<List<HrStatusDto>> GetHrListAsync();
        Task SaveHrStatusAsync(HrStatusDto model);
    }
}