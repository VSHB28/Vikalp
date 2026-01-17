using Vikalp.Models.DTO;

public interface IDropdownService
{
    List<DropdownDto> GetRoles();
    List<DropdownDto> GetLanguages();
    List<DropdownDto> GetGenders();
}
