using Humanizer;
using Microsoft.Data.SqlClient;
using System.Data;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;
public class MeetingService : IMeetingService
{
    private readonly IConfiguration _config;

    public MeetingService(IConfiguration config)
    {
        _config = config;
    }

    private string Conn() => _config.GetConnectionString("DefaultConnection")
                             ?? throw new InvalidOperationException("No connection string");

    public async Task<(IEnumerable<MeetingDto> Data, int TotalCount)> GetAllAsync(
    int userId,
    int page,
    int pageSize,
    int? StateId,
    int? DistrictId,
    int? BlockId,
    int? FacilityId)
    {
        var list = new List<MeetingDto>();
        int totalCount = 0;

        using var conn = new SqlConnection(Conn());

        using var cmd = new SqlCommand("sp_GetMeetingsAllAsync", conn)
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

        await conn.OpenAsync();

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var dto = new MeetingDto
            {
                MeetingId = reader.GetInt32(reader.GetOrdinal("MeetingId")),
                MeetingGuid = reader["MeetingGuid"]?.ToString(),

                MeetingType = GetNullableInt(reader, "MeetingType"),

                MeetingDate = GetNullableDate(reader, "MeetingDate"),

                StateId = GetNullableInt(reader, "StateId"),
                DistrictId = GetNullableInt(reader, "DistrictId"),
                BlockId = GetNullableInt(reader, "BlockId"),
                FacilityId = GetNullableInt(reader, "FacilityId"),

                MeetingPlatform = GetNullableInt(reader, "MeetingPlatform"),

                ReversibleMethodsReview = GetNullableInt(reader, "ReversibleMethodsReview"),
                AshaIncentivesReview = GetNullableInt(reader, "AshaIncentivesReview"),
                FplmisAndStockStatusReview = GetNullableInt(reader, "FplmisAndStockStatusReview"),

                StaffFacilitateReversibleContraceptives =
                    GetNullableInt(reader, "StaffFacilitateReversibleContraceptives"),

                GovernmentOfficialJointVisit =
                    GetNullableInt(reader, "GovernmentOfficialJointVisit"),

                IsIDFstaffPresent =
                    GetNullableInt(reader, "IsIDFstaffPresent"),

                IdfStaffName = reader["IdfStaffName"]?.ToString(),

                MeetingVenue = reader["MeetingVenue"]?.ToString(),

                Remarks = reader["Remarks"]?.ToString(),

                Participants = reader["Participants"]?.ToString(),

                StateName = reader["StateName"]?.ToString(),
                DistrictName = reader["DistrictName"]?.ToString(),
                BlockName = reader["BlockName"]?.ToString(),
                FacilityName = reader["FacilityName"]?.ToString()
            };

            if (!reader.IsDBNull(reader.GetOrdinal("TotalRecords")))
            {
                totalCount = reader.GetInt32(reader.GetOrdinal("TotalRecords"));
            }

            list.Add(dto);
        }

        return (list, totalCount);
    }

    public async Task<MeetingDto?> GetByIdAsync(int id)
    {
        using var conn = new SqlConnection(Conn());
        using var cmd = new SqlCommand("sp_GetMeetingsByidAsync", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@MeetingId", id);

        await conn.OpenAsync();

        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new MeetingDto
            {
                // Primary
                MeetingId = reader.GetInt32(reader.GetOrdinal("MeetingId")),
                MeetingGuid = reader["MeetingGuid"]?.ToString(),

                MeetingType = GetNullableInt(reader, "MeetingType"),

                MeetingDate = GetNullableDate(reader, "MeetingDate"),

                StateId = GetNullableInt(reader, "StateId"),
                DistrictId = GetNullableInt(reader, "DistrictId"),
                BlockId = GetNullableInt(reader, "BlockId"),
                FacilityId = GetNullableInt(reader, "FacilityId"),

                MeetingPlatform = GetNullableInt(reader, "MeetingPlatform"),

                ReversibleMethodsReview = GetNullableInt(reader, "ReversibleMethodsReview"),
                AshaIncentivesReview = GetNullableInt(reader, "AshaIncentivesReview"),
                FplmisAndStockStatusReview = GetNullableInt(reader, "FplmisAndStockStatusReview"),

                StaffFacilitateReversibleContraceptives =
                    GetNullableInt(reader, "StaffFacilitateReversibleContraceptives"),

                GovernmentOfficialJointVisit =
                    GetNullableInt(reader, "GovernmentOfficialJointVisit"),

                IsIDFstaffPresent =
                    GetNullableInt(reader, "IsIDFstaffPresent"),

                IdfStaffName = reader["IdfStaffName"]?.ToString(),
                MeetingVenue = reader["MeetingVenue"]?.ToString(),

                Remarks = reader["Remarks"]?.ToString(),

                Participants = reader["Participants"]?.ToString(),

                StateName = reader["StateName"]?.ToString(),
                DistrictName = reader["DistrictName"]?.ToString(),
                BlockName = reader["BlockName"]?.ToString(),
                FacilityName = reader["FacilityName"]?.ToString()
            };
        }

        return null;
    }

    private int? GetNullableInt(SqlDataReader reader, string column)
    {
        return reader.IsDBNull(reader.GetOrdinal(column))
            ? (int?)null
            : reader.GetInt32(reader.GetOrdinal(column));
    }

    private DateTime? GetNullableDate(SqlDataReader reader, string column)
    {
        return reader.IsDBNull(reader.GetOrdinal(column))
            ? (DateTime?)null
            : reader.GetDateTime(reader.GetOrdinal(column));
    }

    public async Task<bool> CreateAsync(MeetingDto dto, int userId)
    {
        try
        {
            using var conn = new SqlConnection(Conn());
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_CreateMeetingDetails", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@MeetingGuid", Guid.NewGuid().ToString());

            cmd.Parameters.AddWithValue("@MeetingTypeId",
                (object?)dto.MeetingType ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@MeetingDate",
                (object?)dto.MeetingDate ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@StateId",
                (object?)dto.StateId ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@DistrictId",
                (object?)dto.DistrictId ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@BlockId",
                (object?)dto.BlockId ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@FacilityId",
                (object?)dto.FacilityId ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@MeetingVenue",
                dto.MeetingVenue ?? "");

            cmd.Parameters.AddWithValue("@MeetingPlatform",
                (object?)dto.MeetingPlatform ?? DBNull.Value);

            // Multi-select participants
            cmd.Parameters.AddWithValue("@Participants",
                dto.Participants ?? "");

            cmd.Parameters.AddWithValue("@ParticipantOther",
                dto.ParticipantOther ?? "");

            cmd.Parameters.AddWithValue("@ReversibleMethodsReview",
                (object?)dto.ReversibleMethodsReview ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@AshaIncentivesReview",
                (object?)dto.AshaIncentivesReview ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@FplmisAndStockStatusReview",
                (object?)dto.FplmisAndStockStatusReview ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@StaffFacilitateReversibleContraceptives",
                (object?)dto.StaffFacilitateReversibleContraceptives ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@GovernmentOfficialJointVisit",
                (object?)dto.GovernmentOfficialJointVisit ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@IsIDFstaffPresent",
                (object?)dto.IsIDFstaffPresent ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@IdfStaffName",
                dto.IdfStaffName ?? "");

            cmd.Parameters.AddWithValue("@Remarks",
                dto.Remarks ?? "");

            cmd.Parameters.AddWithValue("@CreatedBy", userId);

            await cmd.ExecuteNonQueryAsync();

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(MeetingDto dto, int userId)
    {
        try
        {
            using var con = new SqlConnection(Conn());
            using var cmd = new SqlCommand("sp_UpdateMeetingDetails", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@MeetingId", dto.MeetingId);
            cmd.Parameters.AddWithValue("@MeetingGuid", dto.MeetingGuid);

            cmd.Parameters.AddWithValue("@MeetingTypeId",
                (object?)dto.MeetingType ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@MeetingDate",
                (object?)dto.MeetingDate ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@StateId",
                (object?)dto.StateId ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@DistrictId",
                (object?)dto.DistrictId ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@BlockId",
                (object?)dto.BlockId ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@FacilityId",
                (object?)dto.FacilityId ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@MeetingVenue",
                dto.MeetingVenue ?? (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@MeetingPlatform",
                (object?)dto.MeetingPlatform ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@Participants",
                dto.Participants ?? (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@ParticipantOther",
                dto.ParticipantOther ?? (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@ReversibleMethodsReview",
                (object?)dto.ReversibleMethodsReview ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@AshaIncentivesReview",
                (object?)dto.AshaIncentivesReview ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@FplmisAndStockStatusReview",
                (object?)dto.FplmisAndStockStatusReview ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@StaffFacilitateReversibleContraceptives",
                (object?)dto.StaffFacilitateReversibleContraceptives ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@GovernmentOfficialJointVisit",
                (object?)dto.GovernmentOfficialJointVisit ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@IsIDFstaffPresent",
                (object?)dto.IsIDFstaffPresent ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@IdfStaffName",
                dto.IdfStaffName ?? (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@Remarks",
                dto.Remarks ?? (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@UpdatedBy", userId);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        using var conn = new SqlConnection(Conn());
        using var cmd = new SqlCommand("sp_DeleteMeeting", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@EventId", id);
        cmd.Parameters.AddWithValue("@DeletedBy", userId);

        await conn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }
}