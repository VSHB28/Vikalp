using System.Data;
using Vikalp.Models.DTO;
using Vikalp.Utilities;

public class DropdownService : IDropdownService
{
    private readonly IConfiguration _configuration;

    public DropdownService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private string Conn()
    {
        return _configuration.GetConnectionString("DefaultConnection");
    }

    public List<DropdownDto> GetRoles()
    {
        var dt = SqlUtils.ExecuteSP(Conn(), "sp_GetRoles", null);

        return dt.AsEnumerable().Select(r => new DropdownDto
        {
            Id = r.Field<int>("RoleId"),
            Name = r.Field<string>("RoleName")
        }).ToList();
    }

    public List<DropdownDto> GetLanguages()
    {
        var dt = SqlUtils.ExecuteSP(Conn(), "sp_GetLanguages", null);

        return dt.AsEnumerable().Select(r => new DropdownDto
        {
            Id = r.Field<int>("LanguageId"),
            Name = r.Field<string>("LanguageName")
        }).ToList();
    }

    public List<DropdownDto> GetGenders()
    {
        var dt = SqlUtils.ExecuteSP(Conn(), "sp_GetGenders", null);

        return dt.AsEnumerable().Select(r => new DropdownDto
        {
            Id = r.Field<int>("GenderId"),
            Name = r.Field<string>("GenderName")
        }).ToList();
    }
}
