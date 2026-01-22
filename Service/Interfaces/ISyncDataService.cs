using Vikalp.Models.DTO;

namespace Vikalp.Service.Interfaces
{
    public interface ISyncDataService
    {
        Task<ApiResponseDto<object>> SaveAshaOrientationFromJsonAsync(string rawJson, int userId);
    }
}