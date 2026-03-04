using Humanizer;
using Microsoft.Data.SqlClient;
using System.Data;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;
public class ChecklistVisitService : IChecklistVisitService
{
    private readonly IConfiguration _config;

    public ChecklistVisitService(IConfiguration config)
    {
        _config = config;
    }

    private string Conn() => _config.GetConnectionString("DefaultConnection")
                             ?? throw new InvalidOperationException("No connection string");

    public async Task<(IEnumerable<ChecklistVisitDTO> Data, int TotalCount)>GetAllAsync(int userId, int page, int pageSize, int? StateId, int? DistrictId, int? BlockId, int? FacilityId, int? SubCenterId)
    {
        var list = new List<ChecklistVisitDTO>();
        int totalCount = 0;

        using var conn = new SqlConnection(Conn());
        using var cmd = new SqlCommand("sp_GetChecklistVisitall", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@Page", page);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        cmd.Parameters.AddWithValue("@StateId", (object?)StateId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DistrictId", (object?)DistrictId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BlockId", (object?)BlockId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FacilityId", (object?)FacilityId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@SubCenterId", (object?)SubCenterId ?? DBNull.Value);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var dto = new ChecklistVisitDTO
            {
                ChecklistId = reader.GetInt32(reader.GetOrdinal("ChecklistId")),
                ChecklistGuid = reader["ChecklistGuid"]?.ToString(),

                VisitDate = reader.IsDBNull(reader.GetOrdinal("VisitDate"))
                    ? (DateTime?)null
                    : reader.GetDateTime(reader.GetOrdinal("VisitDate")),

                VisitType = reader.GetInt32(reader.GetOrdinal("VisitType")),

                VisitorDesignation = GetNullableInt(reader, "VisitorDesignation"),
                VisitorName = reader["VisitorName"]?.ToString(),
                OtherVisitorName = reader["OtherVisitorName"]?.ToString(),

                StateId = GetNullableInt(reader, "StateId"),
                DistrictId = GetNullableInt(reader, "DistrictId"),
                BlockId = GetNullableInt(reader, "BlockId"),
                FacilityId = GetNullableInt(reader, "FacilityId"),
                SubcentreId = GetNullableInt(reader, "SubcentreId"),

                // ---------------- DISTRICT ----------------
                DistrictOfficialsOriented = GetNullableInt(reader, "DistrictOfficialsOriented"),
                DistrictOfficialsHMProtocol = GetNullableInt(reader, "DistrictOfficialsHMProtocol"),
                DistrictNoStockOut = GetNullableInt(reader, "DistrictNoStockOut"),
                DistrictASHAFelicitated = GetNullableInt(reader, "DistrictASHAFelicitated"),
                DistrictELAsIssued = GetNullableInt(reader, "DistrictELAsIssued"),
                DistrictReviewsConducted = GetNullableInt(reader, "DistrictReviewsConducted"),
                DistrictReviewsInDHS = GetNullableInt(reader, "DistrictReviewsInDHS"),
                DistrictFollowupLetterIssued = GetNullableInt(reader, "DistrictFollowupLetterIssued"),

                // ---------------- BLOCK ----------------
                BlockOfficialsOriented = GetNullableInt(reader, "BlockOfficialsOriented"),
                BlockOfficialsHMProtocol = GetNullableInt(reader, "BlockOfficialsHMProtocol"),
                BlockDashboardUsed = GetNullableInt(reader, "BlockDashboardUsed"),
                BlockDataEntryCompleted = GetNullableInt(reader, "BlockDataEntryCompleted"),
                BlockNoStockOut = GetNullableInt(reader, "BlockNoStockOut"),
                BlockASHAFelicitated = GetNullableInt(reader, "BlockASHAFelicitated"),
                BlockELAsIssued = GetNullableInt(reader, "BlockELAsIssued"),

                // ---------------- PHC ----------------
                PHCTrainedProviderAvailable = GetNullableInt(reader, "PHCTrainedProviderAvailable"),
                PHCExplainerVideoUsed = GetNullableInt(reader, "PHCExplainerVideoUsed"),
                PHCIECMaterialsAvailable = GetNullableInt(reader, "PHCIECMaterialsAvailable"),
                PHCSelfcareKitsInstalled = GetNullableInt(reader, "PHCSelfcareKitsInstalled"),
                PHCRecordsUpdated = GetNullableInt(reader, "PHCRecordsUpdated"),
                PHCNoStockOut = GetNullableInt(reader, "PHCNoStockOut"),
                PHCDashboardUsed = GetNullableInt(reader, "PHCDashboardUsed"),
                PHCMOProtocolArticulated = GetNullableInt(reader, "PHCMOProtocolArticulated"),
                PHCSPProtocolArticulated = GetNullableInt(reader, "PHCSPProtocolArticulated"),
                PHCELAsDisseminated = GetNullableInt(reader, "PHCELAsDisseminated"),
                PHCASHATrained = GetNullableInt(reader, "PHCASHATrained"),

                // ---------------- HWC ----------------
                HWCTrainedProviderAvailable = GetNullableInt(reader, "HWCTrainedProviderAvailable"),
                HWCExplainerVideoUsed = GetNullableInt(reader, "HWCExplainerVideoUsed"),
                HWCIECMaterialsAvailable = GetNullableInt(reader, "HWCIECMaterialsAvailable"),
                HWCSelfcareKitsInstalled = GetNullableInt(reader, "HWCSelfcareKitsInstalled"),
                HWCRecordsUpdated = GetNullableInt(reader, "HWCRecordsUpdated"),
                HWCNoStockOut = GetNullableInt(reader, "HWCNoStockOut"),
                HWCDashboardUsed = GetNullableInt(reader, "HWCDashboardUsed"),
                HWCAntaraDosesProvided = GetNullableInt(reader, "HWCAntaraDosesProvided"),
                HWCELAsDisseminated = GetNullableInt(reader, "HWCELAsDisseminated"),
                HWCASHATrained = GetNullableInt(reader, "HWCASHATrained"),
            };

            // Total count from COUNT(*) OVER()
            if (!reader.IsDBNull(reader.GetOrdinal("TotalRecords")))
            {
                totalCount = reader.GetInt32(reader.GetOrdinal("TotalRecords"));
            }

            list.Add(dto);
        }

        return (list, totalCount);
    }

    public async Task<ChecklistVisitDTO?> GetByIdAsync(int id)
    {
        using var conn = new SqlConnection(Conn());
        using var cmd = new SqlCommand("sp_GetChecklisVisitById", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ChecklistId", id);

        await conn.OpenAsync();

        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new ChecklistVisitDTO
            {
                ChecklistId = reader.GetInt32(reader.GetOrdinal("ChecklistId")),
                ChecklistGuid = reader["ChecklistGuid"]?.ToString(),

                VisitDate = reader.IsDBNull(reader.GetOrdinal("VisitDate"))
                    ? (DateTime?)null
                    : reader.GetDateTime(reader.GetOrdinal("VisitDate")),

                VisitType = reader.GetInt32(reader.GetOrdinal("VisitType")),

                VisitorDesignation = GetNullableInt(reader, "VisitorDesignation"),
                VisitorName = reader["VisitorName"]?.ToString(),
                OtherVisitorName = reader["OtherVisitorName"]?.ToString(),

                StateId = GetNullableInt(reader, "StateId"),
                DistrictId = GetNullableInt(reader, "DistrictId"),
                BlockId = GetNullableInt(reader, "BlockId"),
                FacilityId = GetNullableInt(reader, "FacilityId"),
                SubcentreId = GetNullableInt(reader, "SubcentreId"),

                // ---------------- DISTRICT ----------------
                DistrictOfficialsOriented = GetNullableInt(reader, "DistrictOfficialsOriented"),
                DistrictOfficialsHMProtocol = GetNullableInt(reader, "DistrictOfficialsHMProtocol"),
                DistrictNoStockOut = GetNullableInt(reader, "DistrictNoStockOut"),
                DistrictASHAFelicitated = GetNullableInt(reader, "DistrictASHAFelicitated"),
                DistrictELAsIssued = GetNullableInt(reader, "DistrictELAsIssued"),
                DistrictReviewsConducted = GetNullableInt(reader, "DistrictReviewsConducted"),
                DistrictReviewsInDHS = GetNullableInt(reader, "DistrictReviewsInDHS"),
                DistrictFollowupLetterIssued = GetNullableInt(reader, "DistrictFollowupLetterIssued"),

                // ---------------- BLOCK ----------------
                BlockOfficialsOriented = GetNullableInt(reader, "BlockOfficialsOriented"),
                BlockOfficialsHMProtocol = GetNullableInt(reader, "BlockOfficialsHMProtocol"),
                BlockDashboardUsed = GetNullableInt(reader, "BlockDashboardUsed"),
                BlockDataEntryCompleted = GetNullableInt(reader, "BlockDataEntryCompleted"),
                BlockNoStockOut = GetNullableInt(reader, "BlockNoStockOut"),
                BlockASHAFelicitated = GetNullableInt(reader, "BlockASHAFelicitated"),
                BlockELAsIssued = GetNullableInt(reader, "BlockELAsIssued"),

                // ---------------- PHC ----------------
                PHCTrainedProviderAvailable = GetNullableInt(reader, "PHCTrainedProviderAvailable"),
                PHCExplainerVideoUsed = GetNullableInt(reader, "PHCExplainerVideoUsed"),
                PHCIECMaterialsAvailable = GetNullableInt(reader, "PHCIECMaterialsAvailable"),
                PHCSelfcareKitsInstalled = GetNullableInt(reader, "PHCSelfcareKitsInstalled"),
                PHCRecordsUpdated = GetNullableInt(reader, "PHCRecordsUpdated"),
                PHCNoStockOut = GetNullableInt(reader, "PHCNoStockOut"),
                PHCDashboardUsed = GetNullableInt(reader, "PHCDashboardUsed"),
                PHCMOProtocolArticulated = GetNullableInt(reader, "PHCMOProtocolArticulated"),
                PHCSPProtocolArticulated = GetNullableInt(reader, "PHCSPProtocolArticulated"),
                PHCELAsDisseminated = GetNullableInt(reader, "PHCELAsDisseminated"),
                PHCASHATrained = GetNullableInt(reader, "PHCASHATrained"),

                // ---------------- HWC ----------------
                HWCTrainedProviderAvailable = GetNullableInt(reader, "HWCTrainedProviderAvailable"),
                HWCExplainerVideoUsed = GetNullableInt(reader, "HWCExplainerVideoUsed"),
                HWCIECMaterialsAvailable = GetNullableInt(reader, "HWCIECMaterialsAvailable"),
                HWCSelfcareKitsInstalled = GetNullableInt(reader, "HWCSelfcareKitsInstalled"),
                HWCRecordsUpdated = GetNullableInt(reader, "HWCRecordsUpdated"),
                HWCNoStockOut = GetNullableInt(reader, "HWCNoStockOut"),
                HWCDashboardUsed = GetNullableInt(reader, "HWCDashboardUsed"),
                HWCAntaraDosesProvided = GetNullableInt(reader, "HWCAntaraDosesProvided"),
                HWCELAsDisseminated = GetNullableInt(reader, "HWCELAsDisseminated"),
                HWCASHATrained = GetNullableInt(reader, "HWCASHATrained"),
            };
        }

        return null;
    }

    private int? GetNullableInt(SqlDataReader reader, string column)
    {
        int ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? (int?)null : reader.GetInt32(ordinal);
    }

    public async Task<bool> CreateAsync(ChecklistVisitDTO dto, int userId)
    {
        try
        {
            using var conn = new SqlConnection(Conn());
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_CreateChecklistVisit", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // ================= BASIC =================

            cmd.Parameters.AddWithValue("@ChecklistGuid", Guid.NewGuid().ToString());
            cmd.Parameters.AddWithValue("@VisitType", (object?)dto.VisitType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@VisitDate", (object?)dto.VisitDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@VisitorDesignation", (object?)dto.VisitorDesignation??DBNull.Value);
            cmd.Parameters.AddWithValue("@VisitorName", (object?)dto.VisitorName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OtherVisitorName", (object?)dto.OtherVisitorName ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@StateId", (object?)dto.StateId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictId", (object?)dto.DistrictId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BlockId", (object?)dto.BlockId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FacilityId", (object?)dto.FacilityId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SubcentreId", (object?)dto.SubcentreId ?? DBNull.Value);

            // ================= DISTRICT =================

            cmd.Parameters.AddWithValue("@DistrictOfficialsOriented", (object?)dto.DistrictOfficialsOriented ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictOfficialsHMProtocol", (object?)dto.DistrictOfficialsHMProtocol ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictNoStockOut", (object?)dto.DistrictNoStockOut ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictASHAFelicitated", (object?)dto.DistrictASHAFelicitated ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictELAsIssued", (object?)dto.DistrictELAsIssued ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictReviewsConducted", (object?)dto.DistrictReviewsConducted ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictReviewsInDHS", (object?)dto.DistrictReviewsInDHS ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictFollowupLetterIssued", (object?)dto.DistrictFollowupLetterIssued ?? DBNull.Value);

            // ================= BLOCK =================

            cmd.Parameters.AddWithValue("@BlockOfficialsOriented", (object?)dto.BlockOfficialsOriented ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BlockOfficialsHMProtocol", (object?)dto.BlockOfficialsHMProtocol ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BlockDashboardUsed", (object?)dto.BlockDashboardUsed ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BlockDataEntryCompleted", (object?)dto.BlockDataEntryCompleted ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BlockNoStockOut", (object?)dto.BlockNoStockOut ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BlockASHAFelicitated", (object?)dto.BlockASHAFelicitated ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BlockELAsIssued", (object?)dto.BlockELAsIssued ?? DBNull.Value);

            // ================= PHC =================

            cmd.Parameters.AddWithValue("@PHCTrainedProviderAvailable", (object?)dto.PHCTrainedProviderAvailable ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCExplainerVideoUsed", (object?)dto.PHCExplainerVideoUsed ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCIECMaterialsAvailable", (object?)dto.PHCIECMaterialsAvailable ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCSelfcareKitsInstalled", (object?)dto.PHCSelfcareKitsInstalled ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCRecordsUpdated", (object?)dto.PHCRecordsUpdated ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCNoStockOut", (object?)dto.PHCNoStockOut ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCDashboardUsed", (object?)dto.PHCDashboardUsed ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCMOProtocolArticulated", (object?)dto.PHCMOProtocolArticulated ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCSPProtocolArticulated", (object?)dto.PHCSPProtocolArticulated ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCELAsDisseminated", (object?)dto.PHCELAsDisseminated ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCASHATrained", (object?)dto.PHCASHATrained ?? DBNull.Value);

            // ================= HWC / SUBCENTRE =================

            cmd.Parameters.AddWithValue("@HWCTrainedProviderAvailable", (object?)dto.HWCTrainedProviderAvailable ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HWCExplainerVideoUsed", (object?)dto.HWCExplainerVideoUsed ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HWCIECMaterialsAvailable", (object?)dto.HWCIECMaterialsAvailable ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HWCSelfcareKitsInstalled", (object?)dto.HWCSelfcareKitsInstalled ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HWCRecordsUpdated", (object?)dto.HWCRecordsUpdated ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HWCNoStockOut", (object?)dto.HWCNoStockOut ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HWCDashboardUsed", (object?)dto.HWCDashboardUsed ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HWCAntaraDosesProvided", (object?)dto.HWCAntaraDosesProvided ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HWCELAsDisseminated", (object?)dto.HWCELAsDisseminated ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HWCASHATrained", (object?)dto.HWCASHATrained ?? DBNull.Value);

            // ================= AUDIT =================

            cmd.Parameters.AddWithValue("@CreatedBy", userId);

            await cmd.ExecuteNonQueryAsync();

            return true;
        }
        catch (Exception ex)
        {
            // Log error properly in production
            return false;
        }
    }

    public async Task<bool> UpdateAsync(ChecklistVisitDTO dto, int userId)
    {
        try
        {
            using var con = new SqlConnection(Conn());
            using var cmd = new SqlCommand("sp_UpdateChecklistVisit", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ChecklistId", dto.ChecklistId);

            cmd.Parameters.AddWithValue("@VisitDate", (object?)dto.VisitDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@VisitType", dto.VisitType);

            cmd.Parameters.AddWithValue("@VisitorDesignation", (object?)dto.VisitorDesignation ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@VisitorName", (object?)dto.VisitorName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OtherVisitorName", (object?)dto.OtherVisitorName ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@StateId", (object?)dto.StateId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictId", (object?)dto.DistrictId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BlockId", (object?)dto.BlockId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FacilityId", (object?)dto.FacilityId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SubcentreId", (object?)dto.SubcentreId ?? DBNull.Value);

            // District
            cmd.Parameters.AddWithValue("@DistrictOfficialsOriented", (object?)dto.DistrictOfficialsOriented ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictOfficialsHMProtocol", (object?)dto.DistrictOfficialsHMProtocol ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictNoStockOut", (object?)dto.DistrictNoStockOut ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictASHAFelicitated", (object?)dto.DistrictASHAFelicitated ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictELAsIssued", (object?)dto.DistrictELAsIssued ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictReviewsConducted", (object?)dto.DistrictReviewsConducted ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictReviewsInDHS", (object?)dto.DistrictReviewsInDHS ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictFollowupLetterIssued", (object?)dto.DistrictFollowupLetterIssued ?? DBNull.Value);

            // Block
            cmd.Parameters.AddWithValue("@BlockOfficialsOriented", (object?)dto.BlockOfficialsOriented ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BlockOfficialsHMProtocol", (object?)dto.BlockOfficialsHMProtocol ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BlockDashboardUsed", (object?)dto.BlockDashboardUsed ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BlockDataEntryCompleted", (object?)dto.BlockDataEntryCompleted ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BlockNoStockOut", (object?)dto.BlockNoStockOut ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BlockASHAFelicitated", (object?)dto.BlockASHAFelicitated ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BlockELAsIssued", (object?)dto.BlockELAsIssued ?? DBNull.Value);

            // PHC
            cmd.Parameters.AddWithValue("@PHCTrainedProviderAvailable", (object?)dto.PHCTrainedProviderAvailable ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCExplainerVideoUsed", (object?)dto.PHCExplainerVideoUsed ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCIECMaterialsAvailable", (object?)dto.PHCIECMaterialsAvailable ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCSelfcareKitsInstalled", (object?)dto.PHCSelfcareKitsInstalled ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCRecordsUpdated", (object?)dto.PHCRecordsUpdated ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCNoStockOut", (object?)dto.PHCNoStockOut ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCDashboardUsed", (object?)dto.PHCDashboardUsed ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCMOProtocolArticulated", (object?)dto.PHCMOProtocolArticulated ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCSPProtocolArticulated", (object?)dto.PHCSPProtocolArticulated ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCELAsDisseminated", (object?)dto.PHCELAsDisseminated ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PHCASHATrained", (object?)dto.PHCASHATrained ?? DBNull.Value);

            // HWC
            cmd.Parameters.AddWithValue("@HWCTrainedProviderAvailable", (object?)dto.HWCTrainedProviderAvailable ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HWCExplainerVideoUsed", (object?)dto.HWCExplainerVideoUsed ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HWCIECMaterialsAvailable", (object?)dto.HWCIECMaterialsAvailable ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HWCSelfcareKitsInstalled", (object?)dto.HWCSelfcareKitsInstalled ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HWCRecordsUpdated", (object?)dto.HWCRecordsUpdated ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HWCNoStockOut", (object?)dto.HWCNoStockOut ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HWCDashboardUsed", (object?)dto.HWCDashboardUsed ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HWCAntaraDosesProvided", (object?)dto.HWCAntaraDosesProvided ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HWCELAsDisseminated", (object?)dto.HWCELAsDisseminated ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HWCASHATrained", (object?)dto.HWCASHATrained ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@UpdatedBy", userId);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return true;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        using var conn = new SqlConnection(Conn());
        using var cmd = new SqlCommand("sp_DeleteEventActivity", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@EventId", id);
        cmd.Parameters.AddWithValue("@DeletedBy", userId);

        await conn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }
}