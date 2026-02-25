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
                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                EventGuid = reader["EventGuid"]?.ToString(),
                EventTypeId = reader.IsDBNull(reader.GetOrdinal("EventTypeId"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("EventTypeId")),
                EventDate = reader.IsDBNull(reader.GetOrdinal("EventDate"))
                    ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EventDate")),
                VillageName = reader["VillageName"]?.ToString(),
                Women_0Children = reader.IsDBNull(reader.GetOrdinal("Women_0Children"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("Women_0Children")),
                Men_0Children = reader.IsDBNull(reader.GetOrdinal("Men_0Children"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("Men_0Children")),
                HelplineCalls = reader.IsDBNull(reader.GetOrdinal("HelplineCalls"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("HelplineCalls")),
                AntaraLeadsSent = reader.IsDBNull(reader.GetOrdinal("AntaraLeadsSent"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("AntaraLeadsSent")),
                LeafletsDistributed = reader.IsDBNull(reader.GetOrdinal("LeafletsDistributed"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("LeafletsDistributed")),

                // ✅ joined columns
                FacilityName = reader.IsDBNull(reader.GetOrdinal("FacilityName"))
                    ? null : reader.GetString(reader.GetOrdinal("FacilityName")),
                SubCenter = reader.IsDBNull(reader.GetOrdinal("SubCentre"))
                    ? null : reader.GetString(reader.GetOrdinal("SubCentre")),
                StateName = reader.IsDBNull(reader.GetOrdinal("StateName"))
                    ? null : reader.GetString(reader.GetOrdinal("StateName")),
                DistrictName = reader.IsDBNull(reader.GetOrdinal("DistrictName"))
                    ? null : reader.GetString(reader.GetOrdinal("DistrictName")),
                BlockName = reader.IsDBNull(reader.GetOrdinal("BlockName"))
                    ? null : reader.GetString(reader.GetOrdinal("BlockName"))
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
                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                EventGuid = reader["EventGuid"]?.ToString(),
                EventTypeId = reader.IsDBNull(reader.GetOrdinal("EventTypeId"))
                ? (int?)null : reader.GetInt32(reader.GetOrdinal("EventTypeId")),
                EventDate = reader.IsDBNull(reader.GetOrdinal("EventDate"))
                ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EventDate")),
                VillageName = reader["VillageName"]?.ToString(),

                Women_0Children = reader.IsDBNull(reader.GetOrdinal("Women_0Children"))
                ? (int?)null : reader.GetInt32(reader.GetOrdinal("Women_0Children")),
                Men_0Children = reader.IsDBNull(reader.GetOrdinal("Men_0Children"))
                ? (int?)null : reader.GetInt32(reader.GetOrdinal("Men_0Children")),
                HelplineCalls = reader.IsDBNull(reader.GetOrdinal("HelplineCalls"))
                ? (int?)null : reader.GetInt32(reader.GetOrdinal("HelplineCalls")),
                AntaraLeadsSent = reader.IsDBNull(reader.GetOrdinal("AntaraLeadsSent"))
                ? (int?)null : reader.GetInt32(reader.GetOrdinal("AntaraLeadsSent")),
                LeafletsDistributed = reader.IsDBNull(reader.GetOrdinal("LeafletsDistributed"))
                ? (int?)null : reader.GetInt32(reader.GetOrdinal("LeafletsDistributed")),

                // ✅ joined columns
                FacilityName = reader.IsDBNull(reader.GetOrdinal("FacilityName"))
                ? null : reader.GetString(reader.GetOrdinal("FacilityName")),
                SubCenter = reader.IsDBNull(reader.GetOrdinal("SubCentre"))
                ? null : reader.GetString(reader.GetOrdinal("SubCentre")),
                StateName = reader.IsDBNull(reader.GetOrdinal("StateName"))
                ? null : reader.GetString(reader.GetOrdinal("StateName")),
                DistrictName = reader.IsDBNull(reader.GetOrdinal("DistrictName"))
                ? null : reader.GetString(reader.GetOrdinal("DistrictName")),
                BlockName = reader.IsDBNull(reader.GetOrdinal("BlockName"))
                ? null : reader.GetString(reader.GetOrdinal("BlockName")),

                StateId = reader.IsDBNull(reader.GetOrdinal("StateId"))
    ? (int?)null : reader.GetInt32(reader.GetOrdinal("StateId")),

                DistrictId = reader.IsDBNull(reader.GetOrdinal("DistrictId"))
    ? (int?)null : reader.GetInt32(reader.GetOrdinal("DistrictId")),

                BlockId = reader.IsDBNull(reader.GetOrdinal("BlockId"))
    ? (int?)null : reader.GetInt32(reader.GetOrdinal("BlockId")),

                FacilityId = reader.IsDBNull(reader.GetOrdinal("FacilityId"))
    ? (int?)null : reader.GetInt32(reader.GetOrdinal("FacilityId")),

                SubcentreId = reader.IsDBNull(reader.GetOrdinal("SubCentreId"))
    ? (int?)null : reader.GetInt32(reader.GetOrdinal("SubCentreId"))
            };
        }
        return null;
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
        using var conn = new SqlConnection(Conn());
        using var cmd = new SqlCommand("sp_UpdateEventActivity", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@EventId", dto.EventId);
        cmd.Parameters.AddWithValue("@EventGuid", dto.EventGuid ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@EventTypeId", dto.EventTypeId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@EventDate", dto.EventDate ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@VillageName", dto.VillageName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@UpdatedBy", userId);

        await conn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
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