using Microsoft.AspNetCore.Mvc.Rendering;
using Vikalp.Models.DTO;

public interface IDropdownService
{
    List<DropdownDto> GetRoles();
    List<DropdownDto> GetLanguages();
    List<DropdownDto> GetGenders();

    List<DropdownDto> GetStates();
    List<DropdownDto> GetDistricts(int stateId);
    List<DropdownDto> GetBlocks(int districtId);
    List<DropdownDto> GetFacilities(int blockId);
    List<DropdownDto> GetEventActivities();

    List<DropdownDto> GetSubCentre(int blockId, int UserId);
    List<DropdownDto> GetFacilityTypes();
    List<DropdownDto> GetAshas();
    AshaDetailDto GetAshaDetails(int ashaId);

    List<DropdownDto> GetAshaDetailsbySubcentre(int subcentreId, int UserId);
    List<DropdownDto> GetTopicsCovered(int userId, int flagId);

    Task<Dictionary<string, List<SelectListItem>>> GetCommonDropdownsAsync(int userId, int languageId);

    Task<Dictionary<string, List<SelectListItem>>> GetCommonDropdownsfamilyplanningAsync(int userId, int languageId);
}
