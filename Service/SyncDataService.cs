using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Vikalp.Data;
using Vikalp.Models;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;

public class SyncDataService : ISyncDataService
{
    private readonly ApplicationDbContext _context;

    public SyncDataService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponseDto<object>> SaveAshaOrientationFromJsonAsync(string rawJson, int userId)
    {
        try
        {
            var model = JsonSerializer.Deserialize<AshaOrientationCreateDto>(rawJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (model == null)
            {
                return ApiResponseDto<object>.ErrorResponse("Invalid JSON payload", 400);
            }

            // ---------------- VALIDATIONS ----------------
            var errors = new List<string>();

            if (model.StateId <= 0) errors.Add("State is required");
            if (model.DistrictId <= 0) errors.Add("District is required");
            if (model.BlockId <= 0) errors.Add("Block is required");
            if (model.DateofOrientation == default) errors.Add("Orientation date is required");

            if (model.IsIntervention == 1 && !model.FacilityId.HasValue)
                errors.Add("Facility is required for intervention");

            if (model.IsIntervention == 0 && string.IsNullOrWhiteSpace(model.Venue))
                errors.Add("Facility name is required for non-intervention");

            if (model.Attendees == null || model.Attendees.Count == 0)
                errors.Add("At least one ASHA is required");

            if (errors.Any())
            {
                return ApiResponseDto<object>.ErrorResponse(errors, 400);
            }
            
            string venueGuid = Guid.NewGuid().ToString();

            using var tx = await _context.Database.BeginTransactionAsync();

            // ---------------- SAVE VENUE ----------------
            var venue = new OrientationVenueDetails
            {
                VenueGuid = venueGuid,
                IsIntervention = model.IsIntervention,
                StateId = model.StateId,
                DistrictId = model.DistrictId,
                BlockId = model.BlockId,
                FacilityId = model.FacilityId,
                FacilityName = model.FacilityName,
                DateofOrientation = model.DateofOrientation,
                NIN = model.NIN,
                CreatedOn = DateTime.Now,
                CreatedBy = userId
            };

            _context.OrientationVenueDetails.Add(venue);
            await _context.SaveChangesAsync();

            // ---------------- SAVE ASHAS ----------------
            foreach (var row in model.Attendees)
            {
                var asha = new TblAshaOrientation
                {
                    VenueGuid = venueGuid,
                    OrientationGuid = Guid.NewGuid().ToString(),
                    IsIntervention = model.IsIntervention,
                    AshaId = row.AshaId,
                    AshaName = row.AshaName!,
                    AshaMobile = row.AshaMobile,
                    FacilityId = model.FacilityId,
                    FacilityName = model.FacilityName,
                    NIN = model.NIN,
                    VCAT_PreTest = row.VCAT_PreTest,
                    VCAT_PostTest = row.VCAT_PostTest,
                    IsOrientation = row.IsOrientation ? 1 : 0,
                    CreatedOn = DateTime.Now,
                    CreatedBy = userId
                };

                _context.TblAshaOrientations.Add(asha);
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return ApiResponseDto<object>.SuccessResponse(null, "ASHA Orientation saved successfully", 200);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<object>.ErrorResponse(ex.Message, 500);
        }
    }
}
