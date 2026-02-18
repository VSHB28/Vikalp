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

    public async Task<List<HomeVisitDTO>> GetAllAsync(int userId)
    {
        var list = new List<HomeVisitDTO>();

        using var con = GetConnection();
        using var cmd = new SqlCommand("sp_getHomeVisitalldata", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@UserId", userId);

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
            FamilyPlanningMethod = dr["FamilyPlanningMethod"] != DBNull.Value ? Convert.ToInt32(dr["FamilyPlanningMethod"]) : (int?)null,
            IsCounsellingDone = dr["IsCounsellingDone"] != DBNull.Value ? Convert.ToInt32(dr["IsCounsellingDone"]) : (int?)null,
            IsReferredForServices = dr["IsReferredForServices"] != DBNull.Value ? Convert.ToInt32(dr["IsReferredForServices"]) : (int?)null,
            ReferredHealthCenterType = dr["ReferredHealthCenterType"] != DBNull.Value ? Convert.ToInt32(dr["ReferredHealthCenterType"]) : (int?)null,
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

    public async Task<List<HomevisitFollowUpDto>> GetFollowUpHistoryAsync(Guid linelistguid, int userId)
    {
        try
        {
            var list = new List<HomevisitFollowUpDto>();

            using var con = GetConnection();
            using var cmd = new SqlCommand("sp_getfollowupHistory", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@linelistguid", linelistguid);

            await con.OpenAsync();
            using var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                list.Add(new HomevisitFollowUpDto
                {
                    FollowupDate = dr["FollowupDate"] as DateTime?,
                    Remark = dr["Remark"]?.ToString(),
                    Status = dr["Status"]?.ToString(),
                    IsEdited = dr["IsEdited"] as bool?
                });
            }

            return list;
        }
        catch (Exception ex)
        {
            throw;
        }
        
    }

    public async Task<bool> InsertFollowUpAsync(HomevisitFollowUpDto model)
    {
        try
        {
            using var con = GetConnection();
            using var cmd = new SqlCommand("sp_InsertFollowUp", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@LineListGuid", model.LineListGuid);
            cmd.Parameters.AddWithValue("@HomVisitGuid", model.HomeVistGuid);
            cmd.Parameters.AddWithValue("@FollowupDate", model.FollowupDate);
            cmd.Parameters.AddWithValue("@FollowupStatus", model.FollowupStatus);
            cmd.Parameters.AddWithValue("@Remark", model.Remark ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);

            await con.OpenAsync();

            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception ex)
        {
            throw;
        }
        
    }

    // =====================================
    // INSERT
    // =====================================
    public async Task<bool> SaveHomeVisitAsync(HomeVisitDTO model, int userId)
    {
        try
        {
            using var con = GetConnection();
        using var cmd = new SqlCommand("sp_homevisitinsertUpdate", con);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Action", "INSERT");
        cmd.Parameters.AddWithValue("@LineListGuid",model.LineListGuid);

        AddCommonParameters(cmd, model, userId);

        await con.OpenAsync();
        int result = await cmd.ExecuteNonQueryAsync();

        return result > 0 || result == -1;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    // =====================================
    // UPDATE
    // =====================================
    public async Task<bool> UpdateHomeVisitAsync(HomeVisitDTO model, int userId)
    {
        try
        {
            using var con = GetConnection();
            using var cmd = new SqlCommand("sp_homevisitinsertUpdate", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "UPDATE");
            cmd.Parameters.AddWithValue("@LineListGuid", model.LineListGuid);

            AddCommonParameters(cmd, model, userId);

            await con.OpenAsync();
            int result = await cmd.ExecuteNonQueryAsync();

            return result > 0 || result == -1;
        }
        catch (Exception ex)
        {
            throw;
        }
        
    }

    private void AddCommonParameters(SqlCommand cmd, HomeVisitDTO model, int userId)
    {
        cmd.Parameters.AddWithValue("@SubCenterId", model.SubCenterId);
        cmd.Parameters.AddWithValue("@VillageName", model.VillageName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@ASHAId", model.ASHAId);
        cmd.Parameters.AddWithValue("@ASHAName", model.ASHAName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@ClientParity", model.ClientParity);

        cmd.Parameters.AddWithValue("@IsCurrentlyPregnant", model.IsCurrentlyPregnant);
        cmd.Parameters.AddWithValue("@IsReceivingSocialBenefit", model.IsReceivingSocialBenefit);
        cmd.Parameters.AddWithValue("@IsUsingFamilyPlanning", model.IsUsingFamilyPlanning);
        cmd.Parameters.AddWithValue("@IsCounsellingDone", model.IsCounsellingDone);
        cmd.Parameters.AddWithValue("@IsReferredForServices", model.IsReferredForServices);
        cmd.Parameters.AddWithValue("@ReferredHealthCenterType", model.ReferredHealthCenterType ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@ReferredHealthCenterName", model.ReferredHealthCenterName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@IsConsentTaken", model.IsConsentTaken);
        cmd.Parameters.AddWithValue("@IsCallInitiated", model.IsCallInitiated);

        cmd.Parameters.AddWithValue("@SocialBenifits",
            model.SocialBenifits != null
            ? string.Join(",", model.SocialBenifits)
            : (object)DBNull.Value);

        cmd.Parameters.AddWithValue("@FamilyPlanningMethod",
            model.FamilyPlanningMethod != null
            ? string.Join(",", model.FamilyPlanningMethod)
            : (object)DBNull.Value);

        cmd.Parameters.AddWithValue("@UserId", userId);
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
