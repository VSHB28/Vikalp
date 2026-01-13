using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Vikalp.Data;
using Vikalp.Models;
using Vikalp.Models.DTO;
using System.Linq;

public class AshaOrientationController : Controller
{
    private readonly ApplicationDbContext _context;

    public AshaOrientationController(ApplicationDbContext context)
    {
        _context = context;
    }

    // LIST
    public async Task<IActionResult> Index()
    {
        var data = await _context.TblAshaOrientations
            .FromSqlRaw("EXEC sp_GetAshaOrientationList")
            .ToListAsync();

        // Map entity to DTO expected by the view
        var dto = data.Select(x => new AshaOrientationDto
        {
            UID = x.UID,
            ANMname = x.ANMname,
            FacilityId = x.FacilityId,
            FacilityNIN = x.FacilityNIN,
            Mobile = x.Mobile,
            VCAT = x.VCAT,
            IsOrientation = x.IsOrientation,
            StateId = x.StateId,
            DistrictId = x.DistrictId
        }).ToList();

        return View(dto);
    }

    // CREATE FORM
    [HttpPost]
    public async Task<IActionResult> Create(TblAshaOrientation model)
    {
        if (ModelState.IsValid)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_InsertAshaOrientation @ANMname, @FacilityId, @FacilityNIN, @Mobile, @VCAT, @IsOrientation, @StateId, @DistrictId",
                new SqlParameter("@ANMname", model.ANMname ?? (object)DBNull.Value),
                new SqlParameter("@FacilityId", model.FacilityId ?? (object)DBNull.Value),
                new SqlParameter("@FacilityNIN", model.FacilityNIN ?? (object)DBNull.Value),
                new SqlParameter("@Mobile", model.Mobile ?? (object)DBNull.Value),
                new SqlParameter("@VCAT", model.VCAT ?? (object)DBNull.Value),
                new SqlParameter("@IsOrientation", model.IsOrientation ?? (object)DBNull.Value),
                new SqlParameter("@StateId", model.StateId ?? (object)DBNull.Value),
                new SqlParameter("@DistrictId", model.DistrictId ?? (object)DBNull.Value)
            );

            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }
}
