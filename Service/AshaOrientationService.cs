using Humanizer;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Mono.TextTemplating;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vikalp.Models;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;
using Vikalp.Utilities;

namespace Vikalp.Service
{
    public class AshaOrientationService : IAshaOrientationService
    {
        private readonly IConfiguration _config;

        public AshaOrientationService(IConfiguration config)
        {
            _config = config;
        }

        private string Conn() => _config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("No connection string");

        public async Task<List<AshaOrientationDto>> GetAllAsync()
        {
            // Run the synchronous DB call on thread-pool to avoid blocking
            var dt = await Task.Run(() => SqlUtils.ExecuteSP(Conn(), "dbo.sp_GetVenueOrientationSummary", null));

            var list = dt.AsEnumerable().Select(r => new AshaOrientationDto
            {
                VenueId = r.Table.Columns.Contains("VenueId") && r["VenueId"] != DBNull.Value
                    ? Convert.ToInt32(r["VenueId"])
                    : 0,

                VenueGuid = r.Table.Columns.Contains("VenueGuid")
                    ? r.Field<string>("VenueGuid")
                    : null,

                FacilityId = r.Table.Columns.Contains("FacilityId") && r["FacilityId"] != DBNull.Value
                    ? Convert.ToInt32(r["FacilityId"])
                    : 0,

                FacilityName = r.Table.Columns.Contains("FacilityName")
                    ? r.Field<string>("FacilityName")
                    : null,

                StateId = r.Table.Columns.Contains("StateId") && r["StateId"] != DBNull.Value
                    ? Convert.ToInt32(r["StateId"])
                    : 0,

                StateName = r.Table.Columns.Contains("StateName")
                    ? r.Field<string>("StateName")
                    : null,

                DistrictId = r.Table.Columns.Contains("DistrictId") && r["DistrictId"] != DBNull.Value
                    ? Convert.ToInt32(r["DistrictId"])
                    : 0,

                DistrictName = r.Table.Columns.Contains("DistrictName")
                    ? r.Field<string>("DistrictName")
                    : null,

                BlockId = r.Table.Columns.Contains("BlockId") && r["BlockId"] != DBNull.Value
                    ? Convert.ToInt32(r["BlockId"])
                    : 0,

                BlockName = r.Table.Columns.Contains("BlockName")
                    ? r.Field<string>("BlockName")
                    : null,

                DateofOrientation = r.Table.Columns.Contains("DateofOrientation") ?r.Field<DateTime>("DateofOrientation") : DateTime.MinValue,

                InterventionCount = r.Table.Columns.Contains("InterventionCount") && r["InterventionCount"] != DBNull.Value
                    ? Convert.ToInt32(r["InterventionCount"])
                    : 0,

                NonInterventionCount = r.Table.Columns.Contains("NonInterventionCount") && r["NonInterventionCount"] != DBNull.Value
                    ? Convert.ToInt32(r["NonInterventionCount"])
                    : 0,

                TotalOrientations = r.Table.Columns.Contains("TotalOrientations") && r["TotalOrientations"] != DBNull.Value
                    ? Convert.ToInt32(r["TotalOrientations"])
                    : 0
            }).ToList();

            return list;
        }

        public async Task CreateAsync(AshaOrientationDto model)
        {
            await Task.Run(() =>
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@AshaId", SqlDbType.Int) { Value = (object?)model.AshaId ?? DBNull.Value },
                    new SqlParameter("@OrientationGuid", SqlDbType.NVarChar) { Value = (object?)model.OrientationGuid ?? DBNull.Value },
                    new SqlParameter("@VCAT_PreTest", SqlDbType.Int) { Value = (object?)model.VCAT_PreTest ?? DBNull.Value },
                    new SqlParameter("@VCAT_PostTest", SqlDbType.Int) { Value = (object?)model.VCAT_PostTest ?? DBNull.Value },
                    new SqlParameter("@IsOrientation", SqlDbType.Bit) { Value = (object?)model.IsOrientation ?? DBNull.Value },
                    new SqlParameter("@OrientationDate", SqlDbType.Date) { Value = (object?)model.DateofOrientation ?? DBNull.Value },
                    new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = (object?)model.CreatedBy ?? DBNull.Value }
                };

                SqlUtils.ExecuteSP(Conn(), "dbo.sp_InsertAshaOrientation", parameters);
            });
        }

        public async Task<List<DropdownDto>> SearchFacilitiesAsync(string term)
        {
            return await Task.Run(() =>
            {
                var param = new SqlParameter[]
                {
            new SqlParameter("@Term", term)
                };

                var dt = SqlUtils.ExecuteSP(Conn(), "sp_SearchFacilities", param);

                return dt.AsEnumerable().Select(r => new DropdownDto
                {
                    Id = r.Field<int>("FacilityId"),
                    Name = r.Field<string>("FacilityName")
                }).ToList();
            });
        }

        public async Task<List<AshaListDto>> GetAshasByFacilityAsync(int facilityId)
        {
            return await Task.Run(() =>
            {
                var param = new SqlParameter[]
                {
            new SqlParameter("@FacilityId", facilityId)
                };

                var dt = SqlUtils.ExecuteSP(
                    Conn(),
                    "sp_GetAshasByFacility",
                    param
                );

                return dt.AsEnumerable()
                    .Select(r => new AshaListDto
                    {
                        AshaId = r.Field<int>("AshaId"),

                        FacilityId = r.Field<int>("FacilityId"),
                        AshaName = r.Field<string>("AshaName"),

                        // ? BIGINT ? string safely
                        AshaMobile = r["AshaMobile"] == DBNull.Value
                            ? null
                            : Convert.ToInt64(r["AshaMobile"]).ToString(),

                        SubCenter = r.Field<string?>("SubCentre"),
                        FacilityName = r.Field<string?>("FacilityName")
                    })
                    .ToList();
            });
        }


        public async Task<object?> GetAshaDetailsAsync(int ashaId)
        {
            return await Task.Run(() =>
            {
                var param = new SqlParameter[]
                {
            new SqlParameter("@AshaId", ashaId)
                };

                var dt = SqlUtils.ExecuteSP(Conn(), "sp_GetAshaById", param);

                return dt.AsEnumerable().Select(r => new
                {
                    AshaId = r.Field<int>("AshaId"),
                    MobileNumber = r.Field<long>("AshaMobile").ToString(),
                    AshaName = r.Field<string>("AshaName")
                }).FirstOrDefault();
            });
        }
        public async Task<MstFacilityDto?> GetFacilityByIdAsync(int facilityId)
        {
            return await Task.Run(() =>
            {
                var param = new SqlParameter[]
                {
            new SqlParameter("@FacilityId", facilityId)
                };

                var dt = SqlUtils.ExecuteSP(Conn(), "sp_GetFacilityById", param);

                return dt.AsEnumerable().Select(r => new MstFacilityDto
                {
                    FacilityId = r.Field<int>("FacilityId"),
                    FacilityName = r.Field<string>("FacilityName"),
                    NinNumber = r.Field<long?>("NinNumber")
                }).FirstOrDefault();
            });
        }

        public async Task SaveVenueAsync(OrientationVenueDetailsDto dto)
        {
            await Task.Run(() =>
            {
                var param = new SqlParameter[]
                {
            new("@VenueGuid", dto.VenueGuid),
            new("@IsIntervention", dto.IsIntervention),
            new("@StateId", dto.StateId),
            new("@DistrictId", dto.DistrictId),
            new("@BlockId", dto.BlockId),
            new("@FacilityId", (object?)dto.FacilityId ?? DBNull.Value),
            new("@FacilityName", dto.FacilityName), // NOT NULL
            new("@DateofOrientation", dto.DateofOrientation),
            new("@NIN", (object?)dto.NIN ?? DBNull.Value),
            new("@CreatedBy", dto.CreatedBy)
                };

                SqlUtils.ExecuteSP(Conn(), "sp_SaveOrientationVenue", param);
            });
        }


        public async Task SaveAshaOrientationAsync(AshaOrientationDto dto)
        {
            await Task.Run(() =>
            {
                var param = new SqlParameter[]
                {
            new("@VenueGuid", dto.VenueGuid),
            new("@OrientationGuid", dto.OrientationGuid), // REQUIRED
            new("@IsIntervention", dto.IsIntervention),   // REQUIRED
            new("@AshaId", (object?)dto.AshaId ?? DBNull.Value),
            new("@AshaName", dto.AshaName),               // REQUIRED
            new("@AshaMobile", (object?)dto.AshaMobile ?? DBNull.Value),
            new("@FacilityId", (object?)dto.FacilityId ?? DBNull.Value),
            new("@FacilityName", dto.FacilityName),       // REQUIRED
            new("@NIN", (object?)dto.NIN ?? DBNull.Value),
            new("@VCAT_PreTest", (object?)dto.VCAT_PreTest ?? DBNull.Value),
            new("@VCAT_PostTest", (object?)dto.VCAT_PostTest ?? DBNull.Value),
            new("@IsOrientation", dto.IsOrientation),
            new("@CreatedBy", dto.CreatedBy)
                };

                SqlUtils.ExecuteSP(Conn(), "sp_SaveAshaOrientation", param);
            });
        }

        public async Task<bool> SaveOrientationJsonAsync(AshaOrientationCreateDto model, int userId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    string topicsCoveredCsv = model.TopicsCovered != null && model.TopicsCovered.Any() ? string.Join(",", model.TopicsCovered) : null;
                    var venueGuid = Guid.NewGuid().ToString();

                    // 1?? Save Venue
                    var venueParams = new SqlParameter[]
                    {
                new SqlParameter("@VenueGuid", venueGuid),
                new SqlParameter("@IsIntervention", model.IsIntervention),
                new SqlParameter("@StateId", model.StateId),
                new SqlParameter("@DistrictId", model.DistrictId),
                new SqlParameter("@BlockId", model.BlockId),
                new SqlParameter("@FacilityId", (object?)model.FacilityId ?? DBNull.Value),
                new SqlParameter("@FacilityName", model.FacilityName ?? ""),
                new SqlParameter("@DateofOrientation", model.DateofOrientation),
                new SqlParameter("@TopicsCovered", (object?)topicsCoveredCsv ?? DBNull.Value),
                new SqlParameter("@NIN", (object?)model.NIN ?? DBNull.Value),
                new SqlParameter("@CreatedBy", userId)
                    };

                    SqlUtils.ExecuteSP(Conn(), "sp_InsertOrientationVenue", venueParams);

                    // 2?? Save ASHA rows
                    foreach (var a in model.Attendees)
                    {
                        var orientationGuid = Guid.NewGuid().ToString();

                        var ashaParams = new SqlParameter[]
                        {
                    new SqlParameter("@VenueGuid", venueGuid),
                    new SqlParameter("@OrientationGuid", orientationGuid),
                    new SqlParameter("@IsIntervention", model.IsIntervention),
                    new SqlParameter("@AshaId", (object?)a.AshaId ?? DBNull.Value),
                    new SqlParameter("@AshaName", a.AshaName ?? ""),
                    new SqlParameter("@AshaMobile", (object?)a.AshaMobile ?? DBNull.Value),
                    new SqlParameter("@FacilityId", (object?)a.VenueId ?? DBNull.Value),
                    new SqlParameter("@FacilityName", a.VenueName),
                    new SqlParameter("@NIN", (object?)model.NIN ?? DBNull.Value),
                    new SqlParameter("@VCAT_PreTest", (object?)a.VCAT_PreTest ?? DBNull.Value),
                    new SqlParameter("@VCAT_PostTest", (object?)a.VCAT_PostTest ?? DBNull.Value),
                    new SqlParameter("@IsOrientation", a.IsOrientation),
                    new SqlParameter("@CreatedBy", userId)
                        };

                        SqlUtils.ExecuteSP(Conn(), "sp_InsertAshaOrientation", ashaParams);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    // log error
                    return false;
                }
            });
        }

        public async Task<AshaOrientationCreateDto?> GetOrientationForEditAsync(string venueGuid)
        {
            return await Task.Run(() =>
            {
                var param = new SqlParameter[]
                {
            new SqlParameter("@VenueGuid", venueGuid)
                };

                var ds = SqlUtils.ExecuteSP_DS(Conn(), "dbo.sp_GetOrientationForEdit", param);

                if (ds.Tables.Count < 2 || ds.Tables[0].Rows.Count == 0)
                    return null;

                var venueRow = ds.Tables[0].Rows[0];

                var model = new AshaOrientationCreateDto
                {
                    StateId = Convert.ToInt32(venueRow["StateId"]),
                    DistrictId = Convert.ToInt32(venueRow["DistrictId"]),
                    BlockId = Convert.ToInt32(venueRow["BlockId"]),
                    FacilityId = venueRow["FacilityId"] == DBNull.Value ? null : (int?)Convert.ToInt32(venueRow["FacilityId"]),
                    FacilityName = venueRow["FacilityName"].ToString(),
                    IsIntervention = Convert.ToInt32(venueRow["IsIntervention"]),
                    DateofOrientation = Convert.ToDateTime(venueRow["DateofOrientation"]),
                    NIN = venueRow["NIN"] == DBNull.Value ? null : (long?)Convert.ToInt64(venueRow["NIN"]),
                    Attendees = new List<AshaOrientationRowDto>(),
                    VenueGuid = venueRow["VenueGuid"].ToString(),
                    TopicsCovered = venueRow["TopicsCovered"] == DBNull.Value ? new List<int>() : venueRow["TopicsCovered"].ToString().Split(',').Select(s => int.Parse(s)).ToList()
                };

                foreach (DataRow r in ds.Tables[1].Rows)
                {
                    model.Attendees.Add(new AshaOrientationRowDto
                    {
                        AshaId = r["AshaId"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["AshaId"]),
                        AshaName = r["AshaName"].ToString(),
                        AshaMobile = r["AshaMobile"] == DBNull.Value ? null : r["AshaMobile"].ToString(),
                        VCAT_PreTest = r["VCAT_PreTest"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["VCAT_PreTest"]),
                        VCAT_PostTest = r["VCAT_PostTest"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["VCAT_PostTest"]),
                        IsOrientation = r["IsOrientation"] == DBNull.Value ? 0 : Convert.ToInt32(r["IsOrientation"]),
                        VenueName = r["FacilityName"].ToString()
                    });
                }

                return model;
            });
        }


        public async Task<bool> UpdateOrientationAsync(int userId, AshaOrientationCreateDto model)
        {
            return await Task.Run(() =>
            {
                var param = new SqlParameter[]
                {                    
                new SqlParameter("@VenueGuid", model.VenueGuid),
                new SqlParameter("@StateId", model.StateId),
                new SqlParameter("@DistrictId", model.DistrictId),  
                new SqlParameter("@BlockId", model.BlockId),
                new SqlParameter("@FacilityId", (object?)model.FacilityId ?? DBNull.Value),
                new SqlParameter("@FacilityName", model.FacilityName ?? (object)DBNull.Value),
                new SqlParameter("@IsIntervention", model.IsIntervention),
                new SqlParameter("@DateofOrientation", model.DateofOrientation),
                new SqlParameter("@NIN", (object?)model.NIN ?? DBNull.Value),
                new SqlParameter("@TopicsCovered", model.TopicsCovered != null && model.TopicsCovered.Any() ? string.Join(",", model.TopicsCovered) : (object)DBNull.Value),
                new SqlParameter("@ModifiedBy", userId)
                };

                SqlUtils.ExecuteSP(Conn(), "dbo.sp_UpdateOrientationVenue", param);

                // Delete & re-insert attendees (simplest + safest)';
                var delParam = new SqlParameter[]
                {
                    new SqlParameter("@VenueGuid",  model.VenueGuid)
                };


                SqlUtils.ExecuteSP(Conn(), "dbo.sp_DeleteOrientationAttendees", delParam);

                foreach (var a in model.Attendees)
                {
                    var orientationGuid = Guid.NewGuid().ToString();
                    var attendeeParam = new SqlParameter[]
                        {
                    new SqlParameter("@VenueGuid", model.VenueGuid),
                    new SqlParameter("@OrientationGuid", orientationGuid),
                    new SqlParameter("@IsIntervention", model.IsIntervention),
                    new SqlParameter("@AshaId", (object?)a.AshaId ?? DBNull.Value),
                    new SqlParameter("@AshaName", a.AshaName ?? ""),
                    new SqlParameter("@AshaMobile", (object?)a.AshaMobile ?? DBNull.Value),
                    new SqlParameter("@FacilityId", (object?)a.VenueId ?? DBNull.Value),
                    new SqlParameter("@FacilityName", a.VenueName),
                    new SqlParameter("@NIN", (object?)model.NIN ?? DBNull.Value),
                    new SqlParameter("@VCAT_PreTest", (object?)a.VCAT_PreTest ?? DBNull.Value),
                    new SqlParameter("@VCAT_PostTest", (object?)a.VCAT_PostTest ?? DBNull.Value),
                    new SqlParameter("@IsOrientation", a.IsOrientation),
                    new SqlParameter("@CreatedBy", userId)
                        };

                    SqlUtils.ExecuteSP(Conn(), "sp_InsertAshaOrientation", attendeeParam);
                }

                return true;
            });
        }

        public void Delete(int venueId)
        {
            var param = new[]
            {
        new SqlParameter("@VenueId", venueId)
    };

            SqlUtils.ExecuteSP(Conn(), "dbo.sp_DeleteOrientation", param);
        }

    }
}