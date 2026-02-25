using Microsoft.AspNetCore.Mvc.Rendering;
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

    public List<DropdownDto> GetSubCentre(int blockId, int UserId)
    {
        var param = new SqlParameter[]
        {
        new SqlParameter("@BlockId", blockId),
        new SqlParameter("@UserId", UserId)
        };

        var dt = SqlUtils.ExecuteSP(Conn(), "sp_GetsubcentresByBlock", param);

        return dt.AsEnumerable().Select(r => new DropdownDto
        {
            Id = r.Field<int>("SubCentreId"),
            Name = r.Field<string>("SubCentre")
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

    public List<DropdownDto> GetAshaDetailsbySubcentre(int subcentreId, int UserId)
    {
        var param = new SqlParameter[]
        {
        new SqlParameter("@Subcentreid", subcentreId),
        new SqlParameter("@UserID", UserId)
        };

        var dt = SqlUtils.ExecuteSP(Conn(), "sp_GetAshaBySubcentre", param);

        return dt.AsEnumerable().Select(r => new DropdownDto
        {
            Id = r.Field<int>("AshaId"),
            Name = r.Field<string>("AshaName")
        }).ToList();
    }

    public List<DropdownDto> GetTopicsCovered(int userId, int flagId)
    {
        var param = new SqlParameter[]
        {
        new SqlParameter("@UserId", 1),
        new SqlParameter("@FlagId", 1)
        };

        var dt = SqlUtils.ExecuteSP(Conn(), "sp_getTopicsCovered", param);

        return dt.AsEnumerable().Select(r => new DropdownDto
        {
            Id = r.Field<int>("Id"),
            Name = r.Field<string>("Value")
        }).ToList();
    }

    public List<DropdownDto> GetFacilityTypes()
    {
        var dt = SqlUtils.ExecuteSP(
            Conn(),
            "sp_GetFacilityTypes",
            null   // no parameters
        );

        return dt.AsEnumerable()
                 .Select(r => new DropdownDto
                 {
                     Id = r.Field<int>("Id"),
                     Name = r.Field<string>("Name")
                 })
                 .ToList();
    }
    public async Task<Dictionary<string, List<SelectListItem>>> GetCommonDropdownsAsync(int userId, int languageId)
    {
        var result = new Dictionary<string, List<SelectListItem>>();

        using (var conn = new SqlConnection(Conn()))
        using (var cmd = new SqlCommand("dbo.sp_getCommonDatafordropdown", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@LanguageId", languageId);

            await conn.OpenAsync();
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                // ActivityTypeName
                var activityTypeName = new List<SelectListItem>();
                while (await reader.ReadAsync())
                {
                    activityTypeName.Add(new SelectListItem
                    {
                        Value = reader["Id"].ToString(),
                        Text = reader["Value"].ToString()
                    });
                }
                result["ActivityTypeName"] = activityTypeName;

                await reader.NextResultAsync();

                var activityType = new List<SelectListItem>();
                while (await reader.ReadAsync())
                {
                    activityType.Add(new SelectListItem
                    {
                        Value = reader["Id"].ToString(),
                        Text = reader["Value"].ToString()
                    });
                }
                result["ActivityType"] = activityType;

                await reader.NextResultAsync();

                var activityFormat = new List<SelectListItem>();
                while (await reader.ReadAsync())
                {
                    activityFormat.Add(new SelectListItem
                    {
                        Value = reader["Id"].ToString(),
                        Text = reader["Value"].ToString()
                    });
                }
                result["ActivityFormat"] = activityFormat;

                await reader.NextResultAsync();

                var clinical = new List<SelectListItem>();
                while (await reader.ReadAsync())
                {
                    clinical.Add(new SelectListItem
                    {
                        Value = reader["Id"].ToString(),
                        Text = reader["Value"].ToString()
                    });
                }
                result["Clinical"] = clinical;

                await reader.NextResultAsync();

                var nonClinical = new List<SelectListItem>();
                while (await reader.ReadAsync())
                {
                    nonClinical.Add(new SelectListItem
                    {
                        Value = reader["Id"].ToString(),
                        Text = reader["Value"].ToString()
                    });
                }
                result["NonClinical"] = nonClinical;

                await reader.NextResultAsync();

                var providerType = new List<SelectListItem>();
                while (await reader.ReadAsync())
                {
                    providerType.Add(new SelectListItem
                    {
                        Value = reader["Id"].ToString(),
                        Text = reader["Value"].ToString()
                    });
                }
                result["ProviderType"] = providerType;

                await reader.NextResultAsync();

                var mstfacilityType = new List<SelectListItem>();
                while (await reader.ReadAsync())
                {
                    mstfacilityType.Add(new SelectListItem
                    {
                        Value = reader["FacilityTypeID"].ToString(),
                        Text = reader["FacilityType"].ToString()
                    });
                }
                result["FacilityType"] = mstfacilityType;

                await reader.NextResultAsync();

                var yesNo = new List<SelectListItem>();
                while (await reader.ReadAsync())
                {
                    yesNo.Add(new SelectListItem
                    {
                        Value = reader["Id"].ToString(),
                        Text = reader["Value"].ToString()
                    });
                }
                result["YesNo"] = yesNo;
            }
        }

        return result;
    }

    public async Task<Dictionary<string, List<SelectListItem>>> GetCommonDropdownsfamilyplanningAsync(int userId, int languageId)
    {
        var result = new Dictionary<string, List<SelectListItem>>();

        using (var conn = new SqlConnection(Conn()))
        using (var cmd = new SqlCommand("dbo.sp_getCommonDataforLineListdropdown", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@LanguageId", languageId);

            await conn.OpenAsync();
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                // Familyplanning
                var familyplanning = new List<SelectListItem>();
                while (await reader.ReadAsync())
                {
                    familyplanning.Add(new SelectListItem
                    {
                        Value = reader["Id"].ToString(),
                        Text = reader["Value"].ToString()
                    });
                }
                result["Familyplanning"] = familyplanning;

                await reader.NextResultAsync();

                var yesNo = new List<SelectListItem>();
                while (await reader.ReadAsync())
                {
                    yesNo.Add(new SelectListItem
                    {
                        Value = reader["Id"].ToString(),
                        Text = reader["Value"].ToString()
                    });
                }
                result["YesNo"] = yesNo;

                await reader.NextResultAsync();

                var whoattandcall = new List<SelectListItem>();
                while (await reader.ReadAsync())
                {
                    whoattandcall.Add(new SelectListItem
                    {
                        Value = reader["Id"].ToString(),
                        Text = reader["Value"].ToString()
                    });
                }
                result["WhoAttandcall"] = whoattandcall;

                await reader.NextResultAsync();

                var socialbenifit = new List<SelectListItem>();
                while (await reader.ReadAsync())
                {
                    socialbenifit.Add(new SelectListItem
                    {
                        Value = reader["Id"].ToString(),
                        Text = reader["Value"].ToString()
                    });
                }
                result["SocialBenifit"] = socialbenifit;

                await reader.NextResultAsync();

                var callstatus = new List<SelectListItem>();
                while (await reader.ReadAsync())
                {
                    callstatus.Add(new SelectListItem
                    {
                        Value = reader["Id"].ToString(),
                        Text = reader["Value"].ToString()
                    });
                }
                result["CallStatus"] = callstatus;
                await reader.NextResultAsync();

                var mstfacilityType = new List<SelectListItem>();
                while (await reader.ReadAsync())
                {
                    mstfacilityType.Add(new SelectListItem
                    {
                        Value = reader["FacilityTypeID"].ToString(),
                        Text = reader["FacilityType"].ToString()
                    });
                }
                result["FacilityType"] = mstfacilityType;
                await reader.NextResultAsync();

                var designation = new List<SelectListItem>();
                while (await reader.ReadAsync())
                {
                    designation.Add(new SelectListItem
                    {
                        Value = reader["Id"].ToString(),
                        Text = reader["Value"].ToString()
                    });
                }
                result["Designation"] = designation;
                await reader.NextResultAsync();

                var gender = new List<SelectListItem>();
                while (await reader.ReadAsync())
                {
                    gender.Add(new SelectListItem
                    {
                        Value = reader["Id"].ToString(),
                        Text = reader["Value"].ToString()
                    });
                }
                result["Gender"] = gender;
            }
        }

        return result;
    }

    //public List<SelectListItem>? GetDropdown(string v)
    //{
    //    switch (v)
    //    {
    //        case "YesNo":
    //            return new List<SelectListItem>
    //            {
    //                new SelectListItem { Text = "Yes", Value = "1" },
    //                new SelectListItem { Text = "No", Value = "0" }
    //            };

    //        case "Gender":
    //            return new List<SelectListItem>
    //            {
    //                new SelectListItem { Text = "Male", Value = "1" },
    //                new SelectListItem { Text = "Female", Value = "2" }
    //            };

    //        default:
    //            return null;
    //    }
    //}

    public List<SelectListItem>? GetDropdown(string key)
    {
        switch (key)
        {
            case "YesNo":
                return new List<SelectListItem>
            {
                new SelectListItem { Text = "Yes", Value = "1" },
                new SelectListItem { Text = "No", Value = "0" }
            };

            case "Gender":
                return new List<SelectListItem>
            {
                new SelectListItem { Text = "Male", Value = "1" },
                new SelectListItem { Text = "Female", Value = "2" },
                new SelectListItem { Text = "Other", Value = "3" }   // optional
            };

            case "Designation":
                return new List<SelectListItem>
            {
                new SelectListItem { Text = "State Director", Value = "1" },
                new SelectListItem { Text = "Senior Coordinator", Value = "2" },
                new SelectListItem { Text = "Specialist Program", Value = "3" },
                new SelectListItem { Text = "Officer", Value = "4" },
                new SelectListItem { Text = "Nursing Trainer", Value = "5" }
            };

            default:
                return new List<SelectListItem>();   // better than null
        }
    }
}
