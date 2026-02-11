using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;

public class HomeVisitService : IHomeVisitService
{
    private readonly IConfiguration _configuration;

    public HomeVisitService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private SqlConnection GetConnection()
        => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

    public async Task<List<HomeVisitDTO>> GetAllAsync()
    {
        var list = new List<HomeVisitDTO>();

        using var con = GetConnection();
        using var cmd = new SqlCommand("USP_HomeVisit_CRUD", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Action", "GETALL");

        await con.OpenAsync();
        using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
        {
            list.Add(MapReaderToHomeVisitDto(dr));
        }

        return list;
    }

    private HomeVisitDTO MapReaderToHomeVisitDto(SqlDataReader dr)
    {
        return new HomeVisitDTO
        {
            // Line Listing fields
            LineListId = dr.GetInt32(dr.GetOrdinal("LineListId")),
            LineListGuid = dr["LineListGuid"]?.ToString(),
            StateId = dr["StateId"] != DBNull.Value ? Convert.ToInt32(dr["StateId"]) : (int?)null,
            StateName = dr["StateName"]?.ToString(),
            DistrictId = dr["DistrictId"] != DBNull.Value ? Convert.ToInt32(dr["DistrictId"]) : (int?)null,
            DistrictName = dr["DistrictName"]?.ToString(),
            BlockId = dr["BlockId"] != DBNull.Value ? Convert.ToInt32(dr["BlockId"]) : (int?)null,
            BlockName = dr["BlockName"]?.ToString(),
            VillageName = dr["VillageName"]?.ToString(),
            FacilityId = dr["FacilityId"] != DBNull.Value ? Convert.ToInt32(dr["FacilityId"]) : (int?)null,
            FacilityName = dr["FacilityName"]?.ToString(),
            SubCenterId = dr["SubCenterId"] != DBNull.Value ? Convert.ToInt32(dr["SubCenterId"]) : (int?)null,
            SubCentre = dr["SubCentre"]?.ToString(),
            ASHAId = dr["ASHAId"] != DBNull.Value ? Convert.ToInt32(dr["ASHAId"]) : (int?)null,
            AnganwadiWorkerName = dr["AnganwadiWorkerName"]?.ToString(),
            WomanName = dr["WomanName"]?.ToString(),
            HusbandName = dr["HusbandName"]?.ToString(),
            MobileNumber = dr["MobileNumber"]?.ToString(),

            // Home Visit fields
            VisitId = dr["VisitId"] != DBNull.Value ? Convert.ToInt32(dr["VisitId"]) : (int?)null,
            VisitGuid = dr["VisitGuid"]?.ToString(),
            ASHAName = dr["ASHAName"]?.ToString(),
            ClientParity = dr["ClientParity"] != DBNull.Value ? Convert.ToInt32(dr["ClientParity"]) : (int?)null,
            IsCurrentlyPregnant = dr["IsCurrentlyPregnant"] != DBNull.Value ? Convert.ToInt32(dr["IsCurrentlyPregnant"]) : (int?)null,
            IsReceivingSocialBenefit = dr["IsReceivingSocialBenefit"] != DBNull.Value ? Convert.ToInt32(dr["IsReceivingSocialBenefit"]) : (int?)null,
            SocialBenifits = dr["SocialBenifits"] != DBNull.Value ? dr["SocialBenifits"].ToString()!.Split(',').Select(int.Parse).ToList() : new List<int>(),
            IsUsingFamilyPlanning = dr["IsUsingFamilyPlanning"] != DBNull.Value ? Convert.ToInt32(dr["IsUsingFamilyPlanning"]) : (int?)null,
            FamilyPlanningMethod = dr["FamilyPlanningMethod"] != DBNull.Value
                    ? dr["FamilyPlanningMethod"].ToString()!
                        .Split(',')
                        .Select(int.Parse)
                        .ToList()
                    : new List<int>(),
            IsCounsellingDone = dr["IsCounsellingDone"] != DBNull.Value ? Convert.ToInt32(dr["IsCounsellingDone"]) : (int?)null,
            IsReferredForServices = dr["IsReferredForServices"] != DBNull.Value ? Convert.ToInt32(dr["IsReferredForServices"]) : (int?)null,
            ReferredHealthCenterType = dr["ReferredHealthCenterType"]?.ToString(),
            ReferredHealthCenterName = dr["ReferredHealthCenterName"]?.ToString(),
            IsConsentTaken = dr["IsConsentTaken"] != DBNull.Value ? Convert.ToInt32(dr["IsConsentTaken"]) : (int?)null,
            IsCallInitiated = dr["IsCallInitiated"] != DBNull.Value ? Convert.ToInt32(dr["IsCallInitiated"]) : (int?)null,

            // Audit fields
            CreatedOn = dr["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(dr["CreatedOn"]) : (DateTime?)null,
            CreatedBy = dr["CreatedBy"] != DBNull.Value ? Convert.ToInt32(dr["CreatedBy"]) : (int?)null,
            UpdatedBy = dr["UpdatedBy"] != DBNull.Value ? Convert.ToInt32(dr["UpdatedBy"]) : (int?)null,
            UpdatedOn = dr["UpdatedOn"] != DBNull.Value ? Convert.ToDateTime(dr["UpdatedOn"]) : (DateTime?)null,

            // Extra naming consistency
            SubCenterName = dr["SubCenterName"]?.ToString()
        };
    }
    public async Task<HomeVisitDTO?> GetByIdAsync(Guid linelistguid, int userId)
    {
        using var con = GetConnection();
        using var cmd = new SqlCommand("sp_getHomeVisitByguid", con);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@linelistguid", linelistguid);

        await con.OpenAsync();
        using var dr = await cmd.ExecuteReaderAsync();

        if (await dr.ReadAsync())
        {
            return MapReaderToHomeVisitDto(dr);
        }

        return null;
    }

    public async Task<bool> InsertAsync(HomeVisitDTO model)
    {
        using var con = GetConnection();
        using var cmd = new SqlCommand("USP_HomeVisit_CRUD", con);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Action", "INSERT");
        cmd.Parameters.AddWithValue("@ASHAName", model.ASHAName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@VillageName", model.VillageName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@SubCenterId", model.SubCenterId);
        cmd.Parameters.AddWithValue("@UserId", model.UserId);

        await con.OpenAsync();

        int result = await cmd.ExecuteNonQueryAsync();

        return result > 0 || result == -1;
    }

    public async Task<bool> UpdateAsync(HomeVisitDTO model)
    {
        using var con = GetConnection();
        using var cmd = new SqlCommand("USP_HomeVisit_CRUD", con);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Action", "UPDATE");
        cmd.Parameters.AddWithValue("@VisitId", model.VisitId);
        cmd.Parameters.AddWithValue("@ASHAId", model.ASHAId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@ASHAName", model.ASHAName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@VillageName", model.VillageName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@ClientParity", model.ClientParity ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@IsCurrentlyPregnant", model.IsCurrentlyPregnant ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@SubCenterId", model.SubCenterId);
        cmd.Parameters.AddWithValue("@UserId", model.UserId);

        await con.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }


    public async Task<bool> DeleteAsync(int visitId, int userId)
    {
        using var con = GetConnection();
        using var cmd = new SqlCommand("USP_HomeVisit_CRUD", con);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Action", "DELETE");
        cmd.Parameters.AddWithValue("@VisitId", visitId);
        cmd.Parameters.AddWithValue("@UserId", userId);

        await con.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int visitId)
    {
        using var con = GetConnection();
        using var cmd = new SqlCommand("USP_HomeVisit_CRUD", con);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Action", "DELETE");
        cmd.Parameters.AddWithValue("@VisitId", visitId);

        await con.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<List<dynamic>> GetSubCentersAsync()
    {
        var list = new List<dynamic>();

        using var con = GetConnection();
        using var cmd = new SqlCommand(
            "SELECT SubCentreId, SubCentre FROM MstSubCentre", con);

        await con.OpenAsync();
        using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
        {
            list.Add(new
            {
                SubCentreId = dr["SubCentreId"],
                SubCentre = dr["SubCentre"].ToString()
            });
        }

        return list;
    }

   
}
