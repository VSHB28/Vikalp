using System.Collections.Generic;
using System.Threading.Tasks;
using Vikalp.Models;
using Vikalp.Models.DTO;

namespace Vikalp.Service.Interfaces
{
    public interface IAshaOrientationService
    {
        Task<List<AshaOrientationDto>> GetAllAsync();
        Task CreateAsync(AshaOrientationDto model);
        Task<List<DropdownDto>> SearchFacilitiesAsync(string term);
        Task<List<AshaListDto>> GetAshasByFacilityAsync(int facilityId);
        Task<object?> GetAshaDetailsAsync(int ashaId);
        Task<MstFacilityDto?> GetFacilityByIdAsync(int facilityId);

        Task SaveVenueAsync(OrientationVenueDetailsDto dto);
        Task SaveAshaOrientationAsync(AshaOrientationDto dto);
        Task<bool> SaveOrientationJsonAsync(AshaOrientationCreateDto model, int userId);

        Task<AshaOrientationCreateDto?> GetOrientationForEditAsync(string venueGuid);
        Task<bool> UpdateOrientationAsync(int userId, AshaOrientationCreateDto model);
        void Delete(int userId);
    }
}