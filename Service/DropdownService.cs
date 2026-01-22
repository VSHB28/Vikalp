using Microsoft.Data.SqlClient;
using Mono.TextTemplating;
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

    public List<DropdownDto> GetStates()
    {
        var param = new SqlParameter[]
        {
        new SqlParameter("@UserID", 1)
        };
        var dt = SqlUtils.ExecuteSP(Conn(), "sp_GetStates", param);

        return dt.AsEnumerable().Select(r => new DropdownDto
        {
            Id = r.Field<int>("StateId"),
            Name = r.Field<string>("StateName")
        }).ToList();
    }


    public List<DropdownDto> GetDistricts(int stateId)
    {
        var param = new SqlParameter[]
        {
        new SqlParameter("@StateId", stateId)
        };

        var dt = SqlUtils.ExecuteSP(Conn(), "sp_GetDistrictsByState", param);

        return dt.AsEnumerable().Select(r => new DropdownDto
        {
            Id = r.Field<int>("DistrictId"),
            Name = r.Field<string>("DistrictName")
        }).ToList();
    }


    public List<DropdownDto> GetBlocks(int districtId)
    {
        var param = new SqlParameter[]
        {
        new SqlParameter("@DistrictId", districtId)
        };

        var dt = SqlUtils.ExecuteSP(Conn(), "sp_GetBlocksByDistrict", param);

        return dt.AsEnumerable().Select(r => new DropdownDto
        {
            Id = r.Field<int>("BlockId"),
            Name = r.Field<string>("BlockName")
        }).ToList();
    }

    public List<DropdownDto> GetFacilities(int blockId)
    {
        var param = new SqlParameter[]
        {
        new SqlParameter("@BlockId", blockId)
        };

        var dt = SqlUtils.ExecuteSP(Conn(), "sp_GetFacilitiesByBlock", param);

        return dt.AsEnumerable().Select(r => new DropdownDto
        {
            Id = r.Field<int>("FacilityId"),
            Name = r.Field<string>("FacilityName")
        }).ToList();
    }

    public List<DropdownDto> GetAshas()
    {
        var param = new SqlParameter[]
        {
        new SqlParameter("@UserID", 1)
        };
        var dt = SqlUtils.ExecuteSP(Conn(), "sp_GetAshas", param);

        return dt.AsEnumerable().Select(r => new DropdownDto
        {
            Id = r.Field<int>("AshaId"),
            Name = r.Field<string>("AshaName")
        }).ToList();
    }

    public AshaDetailDto GetAshaDetails(int ashaId)
    {
        var param = new SqlParameter[]
        {
        new SqlParameter("@AshaId", ashaId)
        };

        var dt = SqlUtils.ExecuteSP(Conn(), "sp_GetAshaById", param);

        return dt.AsEnumerable().Select(r => new AshaDetailDto
        {
            AshaId = r.Field<int>("AshaId"),
            MobileNumber = r.Field<string>("MobileNumber")
        }).FirstOrDefault();
    }

}
