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

    List<DropdownDto> GetAshas();
    AshaDetailDto GetAshaDetails(int ashaId);

}
