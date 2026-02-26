using Humanizer;
using Microsoft.Data.SqlClient;
using System.Data;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;
public class EventActivityService : IEventActivityService
{
    private readonly IConfiguration _config;

    public EventActivityService(IConfiguration config)
    {
        _config = config;
    }

    private string Conn() => _config.GetConnectionString("DefaultConnection")
                             ?? throw new InvalidOperationException("No connection string");

    public async Task<(IEnumerable<EventActivityDto> Data, int TotalCount)> GetAllAsync(int userId, int page, int pageSize, int? StateId, int? DistrictId, int? BlockId,    int? FacilityId, int? SubCenterId)
    {
        var list = new List<EventActivityDto>();
        int totalCount = 0;

        using var conn = new SqlConnection(Conn());
        using var cmd = new SqlCommand("sp_GetEventActivities", conn)
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
            var dto = new EventActivityDto
            {
                // Primary
                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                EventGuid = reader["EventGuid"]?.ToString(),

                EventTypeId = reader.IsDBNull(reader.GetOrdinal("EventTypeId"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("EventTypeId")),

                EventDate = reader.IsDBNull(reader.GetOrdinal("EventDate"))
                    ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EventDate")),

                VillageName = reader["VillageName"]?.ToString(),

                // Location IDs (VERY IMPORTANT for dropdown autofill)
                StateId = reader.IsDBNull(reader.GetOrdinal("StateId"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("StateId")),

                DistrictId = reader.IsDBNull(reader.GetOrdinal("DistrictId"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("DistrictId")),

                BlockId = reader.IsDBNull(reader.GetOrdinal("BlockId"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("BlockId")),

                FacilityId = reader.IsDBNull(reader.GetOrdinal("FacilityId"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("FacilityId")),

                SubcentreId = reader.IsDBNull(reader.GetOrdinal("SubcentreId"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("SubcentreId")),

                // Women
                Women_0Children = GetNullableInt(reader, "Women_0Children"),
                Women_1Child = GetNullableInt(reader, "Women_1Child"),
                Women_2PlusChildren = GetNullableInt(reader, "Women_2PlusChildren"),

                MotherInLawCount = GetNullableInt(reader, "MotherInLawCount"),

                WomenAdopted_Parity0 = GetNullableInt(reader, "WomenAdopted_Parity0"),
                WomenAdopted_Parity1 = GetNullableInt(reader, "WomenAdopted_Parity1"),

                AntaraGiven_Parity0 = GetNullableInt(reader, "AntaraGiven_Parity0"),
                AntaraGiven_Parity1 = GetNullableInt(reader, "AntaraGiven_Parity1"),

                WomenReferred_TemporaryMethods =
                    GetNullableInt(reader, "WomenReferred_TemporaryMethods"),

                // Men
                Men_0Children = GetNullableInt(reader, "Men_0Children"),
                Men_1Child = GetNullableInt(reader, "Men_1Child"),
                Men_2PlusChildren = GetNullableInt(reader, "Men_2PlusChildren"),

                MenReferred_TemporaryMethods =
                    GetNullableInt(reader, "MenReferred_TemporaryMethods"),

                // Antara
                AntaraDosesGiven = GetNullableInt(reader, "AntaraDosesGiven"),

                // Session
                IPCSessionHeld = GetNullableInt(reader, "IPCSessionHeld"),

                // Helpline
                HelplineCalls = GetNullableInt(reader, "HelplineCalls"),
                AntaraLeadsSent = GetNullableInt(reader, "AntaraLeadsSent"),

                // IEC
                LeafletsDistributed = GetNullableInt(reader, "LeafletsDistributed"),
                HappinessKitDistributed = GetNullableInt(reader, "HappinessKitDistributed"),
                AntaraLeafletCount = GetNullableInt(reader, "AntaraLeafletCount"),
                NPKLeafletCount = GetNullableInt(reader, "NPKLeafletCount"),

                // Names (optional display)
                StateName = reader["StateName"]?.ToString(),
                DistrictName = reader["DistrictName"]?.ToString(),
                BlockName = reader["BlockName"]?.ToString(),
                FacilityName = reader["FacilityName"]?.ToString(),
                SubCenter = reader["SubCentre"]?.ToString()
            };

            // capture total count from COUNT(*) OVER()
            if (!reader.IsDBNull(reader.GetOrdinal("TotalRecords")))
            {
                totalCount = reader.GetInt32(reader.GetOrdinal("TotalRecords"));
            }

            list.Add(dto);
        }

        return (list, totalCount);
    }

    public async Task<EventActivityDto?> GetByIdAsync(int id)
    {
        using var conn = new SqlConnection(Conn());
        using var cmd = new SqlCommand("sp_GetEventActivityById", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@EventId", id);

        await conn.OpenAsync();

        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new EventActivityDto
            {
                // Primary
                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                EventGuid = reader["EventGuid"]?.ToString(),

                EventTypeId = reader.IsDBNull(reader.GetOrdinal("EventTypeId"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("EventTypeId")),

                EventDate = reader.IsDBNull(reader.GetOrdinal("EventDate"))
                    ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EventDate")),

                VillageName = reader["VillageName"]?.ToString(),

                // Location IDs (VERY IMPORTANT for dropdown autofill)
                StateId = reader.IsDBNull(reader.GetOrdinal("StateId"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("StateId")),

                DistrictId = reader.IsDBNull(reader.GetOrdinal("DistrictId"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("DistrictId")),

                BlockId = reader.IsDBNull(reader.GetOrdinal("BlockId"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("BlockId")),

                FacilityId = reader.IsDBNull(reader.GetOrdinal("FacilityId"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("FacilityId")),

                SubcentreId = reader.IsDBNull(reader.GetOrdinal("SubcentreId"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("SubcentreId")),

                // Women
                Women_0Children = GetNullableInt(reader, "Women_0Children"),
                Women_1Child = GetNullableInt(reader, "Women_1Child"),
                Women_2PlusChildren = GetNullableInt(reader, "Women_2PlusChildren"),

                MotherInLawCount = GetNullableInt(reader, "MotherInLawCount"),

                WomenAdopted_Parity0 = GetNullableInt(reader, "WomenAdopted_Parity0"),
                WomenAdopted_Parity1 = GetNullableInt(reader, "WomenAdopted_Parity1"),

                AntaraGiven_Parity0 = GetNullableInt(reader, "AntaraGiven_Parity0"),
                AntaraGiven_Parity1 = GetNullableInt(reader, "AntaraGiven_Parity1"),

                WomenReferred_TemporaryMethods =
                    GetNullableInt(reader, "WomenReferred_TemporaryMethods"),

                // Men
                Men_0Children = GetNullableInt(reader, "Men_0Children"),
                Men_1Child = GetNullableInt(reader, "Men_1Child"),
                Men_2PlusChildren = GetNullableInt(reader, "Men_2PlusChildren"),

                MenReferred_TemporaryMethods =
                    GetNullableInt(reader, "MenReferred_TemporaryMethods"),

                // Antara
                AntaraDosesGiven = GetNullableInt(reader, "AntaraDosesGiven"),

                // Session
                IPCSessionHeld = GetNullableInt(reader, "IPCSessionHeld"),

                // Helpline
                HelplineCalls = GetNullableInt(reader, "HelplineCalls"),
                AntaraLeadsSent = GetNullableInt(reader, "AntaraLeadsSent"),

                // IEC
                LeafletsDistributed = GetNullableInt(reader, "LeafletsDistributed"),
                HappinessKitDistributed = GetNullableInt(reader, "HappinessKitDistributed"),
                AntaraLeafletCount = GetNullableInt(reader, "AntaraLeafletCount"),
                NPKLeafletCount = GetNullableInt(reader, "NPKLeafletCount"),

                // Names (optional display)
                StateName = reader["StateName"]?.ToString(),
                DistrictName = reader["DistrictName"]?.ToString(),
                BlockName = reader["BlockName"]?.ToString(),
                FacilityName = reader["FacilityName"]?.ToString(),
                SubCenter = reader["SubCentre"]?.ToString()
            };
        }

        return null;
    }

    private int? GetNullableInt(SqlDataReader reader, string column)
    {
        int ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? (int?)null : reader.GetInt32(ordinal);
    }

    public async Task<bool> CreateAsync(EventActivityDto dto, int userId)
    {
        try
        {
            using var conn = new SqlConnection(Conn());

            await conn.OpenAsync(); // ✅ THIS LINE FIXES THE ERROR

            using var cmd = new SqlCommand("sp_CreateEventActivity", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EventGuid", Guid.NewGuid().ToString());

            cmd.Parameters.AddWithValue("@EventTypeId", (object?)dto.EventTypeId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EventDate", (object?)dto.EventDate ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@StateId", (object?)dto.StateId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictId", (object?)dto.DistrictId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BlockId", (object?)dto.BlockId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FacilityId", (object?)dto.FacilityId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SubcentreId", (object?)dto.SubcentreId ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@VillageName", dto.VillageName ?? "");

            cmd.Parameters.AddWithValue("@Women_0Children", (object?)dto.Women_0Children ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Men_0Children", (object?)dto.Men_0Children ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Women_1Child", (object?)dto.Women_1Child ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Men_1Child", (object?)dto.Men_1Child ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Women_2PlusChildren", (object?)dto.Women_2PlusChildren ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Men_2PlusChildren", (object?)dto.Men_2PlusChildren ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@MotherInLawCount", (object?)dto.MotherInLawCount ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@IPCSessionHeld",
                dto.IPCSessionHeld.HasValue ? dto.IPCSessionHeld : DBNull.Value);

            cmd.Parameters.AddWithValue("@HappinessKitDistributed",
                (object?)dto.HappinessKitDistributed ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@WomenReferred_TemporaryMethods",
                (object?)dto.WomenReferred_TemporaryMethods ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@MenReferred_TemporaryMethods",
                (object?)dto.MenReferred_TemporaryMethods ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@WomenAdopted_Parity0",
                (object?)dto.WomenAdopted_Parity0 ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@WomenAdopted_Parity1",
                (object?)dto.WomenAdopted_Parity1 ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@AntaraGiven_Parity0",
                (object?)dto.AntaraGiven_Parity0 ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@AntaraGiven_Parity1",
                (object?)dto.AntaraGiven_Parity1 ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@AntaraDosesGiven",
                (object?)dto.AntaraDosesGiven ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@HelplineCalls",
                (object?)dto.HelplineCalls ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@AntaraLeadsSent",
                (object?)dto.AntaraLeadsSent ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@LeafletsDistributed",
                (object?)dto.LeafletsDistributed ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@AntaraLeafletCount",
                (object?)dto.AntaraLeafletCount ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@NPKLeafletCount",
                (object?)dto.NPKLeafletCount ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@CreatedBy", userId);

            await cmd.ExecuteNonQueryAsync();

            return true;
        }
        catch (Exception ex)
        {
            // Optional: log ex.Message
            return false;
        }
    }

    public async Task<bool> UpdateAsync(EventActivityDto dto, int userId)
    {
        try
        {
            using var con = new SqlConnection(Conn());
            using var cmd = new SqlCommand("sp_UpdateEventActivity", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EventId", dto.EventId);
            cmd.Parameters.AddWithValue("@EventGuid", dto.EventGuid);
            cmd.Parameters.AddWithValue("@EventTypeId", dto.EventTypeId);
            cmd.Parameters.AddWithValue("@EventDate", dto.EventDate);

            cmd.Parameters.AddWithValue("@StateId", dto.StateId);
            cmd.Parameters.AddWithValue("@DistrictId", dto.DistrictId);
            cmd.Parameters.AddWithValue("@BlockId", dto.BlockId);
            cmd.Parameters.AddWithValue("@FacilityId", dto.FacilityId);
            cmd.Parameters.AddWithValue("@SubCentreId", dto.SubcentreId);

            cmd.Parameters.AddWithValue("@VillageName", dto.VillageName ?? (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@Women_0Children", dto.Women_0Children);
            cmd.Parameters.AddWithValue("@Women_1Child", dto.Women_1Child);
            cmd.Parameters.AddWithValue("@Women_2PlusChildren", dto.Women_2PlusChildren);

            cmd.Parameters.AddWithValue("@Men_0Children", dto.Men_0Children);
            cmd.Parameters.AddWithValue("@Men_1Child", dto.Men_1Child);
            cmd.Parameters.AddWithValue("@Men_2PlusChildren", dto.Men_2PlusChildren);

            cmd.Parameters.AddWithValue("@MotherInLawCount", dto.MotherInLawCount);

            cmd.Parameters.AddWithValue("@WomenAdopted_Parity0", dto.WomenAdopted_Parity0);
            cmd.Parameters.AddWithValue("@WomenAdopted_Parity1", dto.WomenAdopted_Parity1);

            cmd.Parameters.AddWithValue("@AntaraGiven_Parity0", dto.AntaraGiven_Parity0);
            cmd.Parameters.AddWithValue("@AntaraGiven_Parity1", dto.AntaraGiven_Parity1);

            cmd.Parameters.AddWithValue("@AntaraDosesGiven", dto.AntaraDosesGiven);

            cmd.Parameters.AddWithValue("@WomenReferred_TemporaryMethods", dto.WomenReferred_TemporaryMethods);
            cmd.Parameters.AddWithValue("@MenReferred_TemporaryMethods", dto.MenReferred_TemporaryMethods);

            cmd.Parameters.AddWithValue("@IPCSessionHeld", dto.IPCSessionHeld);

            cmd.Parameters.AddWithValue("@HelplineCalls", dto.HelplineCalls);
            cmd.Parameters.AddWithValue("@AntaraLeadsSent", dto.AntaraLeadsSent);

            cmd.Parameters.AddWithValue("@LeafletsDistributed", dto.LeafletsDistributed);
            cmd.Parameters.AddWithValue("@HappinessKitDistributed", dto.HappinessKitDistributed);
            cmd.Parameters.AddWithValue("@AntaraLeafletCount", dto.AntaraLeafletCount);
            cmd.Parameters.AddWithValue("@NPKLeafletCount", dto.NPKLeafletCount);

            cmd.Parameters.AddWithValue("@UpdatedBy", userId);

            await con.OpenAsync();

            await cmd.ExecuteNonQueryAsync();

            return true;
        }
        catch (Exception ex)
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