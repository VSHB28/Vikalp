using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using Vikalp.Models.DTO;
using Vikalp.Service.Implementations;
using Vikalp.Service.Interfaces;

public class SubCentreController : Controller
{
    private readonly ISubCentreService _service;
    private readonly IDropdownService _dropdownService;
    private readonly ILocationBlockService _blockService;

    public SubCentreController(
        ISubCentreService service,
        IDropdownService dropdownService,
        ILocationBlockService blockService)
    {
        _service = service;
        _dropdownService = dropdownService;
        _blockService = blockService;
    }

    // ================= INDEX =================


    public async Task<IActionResult> Index()
    {
        ViewBag.States = _dropdownService.GetStates();

        var dropdowns = await _dropdownService
            .GetCommonDropdownsfamilyplanningAsync(userId: 1, languageId: 1);

        ViewBag.designation = dropdowns.ContainsKey("Designation")
            ? dropdowns["Designation"]
            : new List<SelectListItem>();

        ViewBag.gender = dropdowns.ContainsKey("Gender")
            ? dropdowns["Gender"]
            : new List<SelectListItem>();

        ViewBag.yesNo = dropdowns.ContainsKey("YesNoNa")
            ? dropdowns["YesNoNa"]
            : new List<SelectListItem>();

        return View();
    }

    // ================= DROPDOWNS =================

    public JsonResult GetDistricts(int stateId)
    {
        var data = _dropdownService.GetDistricts(stateId)
            .Select(d => new
            {
                id = d.Id,
                name = d.Name
            });

        return Json(data);
    }

    public JsonResult GetBlocks(int districtId)
    {
        var data = _dropdownService.GetBlocks(districtId)
            .Select(b => new
            {
                id = b.Id,
                name = b.Name
            });

        return Json(data);
    }

    public JsonResult GetFacilities(int blockId)
    {
        var data = _dropdownService.GetFacilities(blockId)
            .Select(f => new
            {
                id = f.Id,
                name = f.Name
            });

        return Json(data);
    }

    public async Task<JsonResult> GetByBlock(int blockId, int? facilityId)
    {
        var data = await _service.GetByBlockAsync(blockId, facilityId);
        return Json(data);
    }
    // ================= CREATE =================

    public IActionResult Create()
    {
        ViewBag.States = _dropdownService.GetStates();

        return View(new SubCentreDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubCentreDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.States = _dropdownService.GetStates();
            return View(dto);
        }

        await _service.AddAsync(dto);
        return RedirectToAction(nameof(Index));
    }



    public async Task<IActionResult> Edit(int id)
    {
        var subCentre = await _service.GetByIdAsync(id);
        if (subCentre == null)
            return NotFound();

        ViewBag.States = _dropdownService.GetStates();

        if (subCentre.BlockId > 0)
        {
            var block = await _blockService.GetByIdAsync(subCentre.BlockId.Value);
            if (block != null)
           
 {
                subCentre.RegionId = block.RegionId;
                subCentre.DistrictId = block.DistrictId;

                ViewBag.Districts = _dropdownService.GetDistricts(block.RegionId);
                ViewBag.Blocks = _dropdownService.GetBlocks(block.DistrictId);
            }
        }
        return View(subCentre);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SubCentreDto dto)
    {

        if (dto == null)
            return BadRequest("DTO is null");

     

        if (!ModelState.IsValid)
        {
            ViewBag.States = _dropdownService.GetStates();
            if (dto.RegionId > 0) ViewBag.Districts = _dropdownService.GetDistricts(dto.RegionId);
            if (dto.DistrictId > 0) ViewBag.Blocks = _dropdownService.GetBlocks(dto.DistrictId);
            if (dto.DistrictId > 0) ViewBag.Facilities = _dropdownService.GetFacilities((int)dto.FacilityId);
            return View(dto);
        }

        if (dto.SubCentreId <= 0) return BadRequest("SebCenter ID missing");

        await _service.UpdateAsync(dto);
        return RedirectToAction(nameof(Index));
    }



    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var SubCenter = await _service.GetByIdAsync(id);
        if (SubCenter == null)
            return NotFound();

        return View(SubCenter);
    }

    // POST: Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var SubCenter = await _service.GetByIdAsync(id);
        if (SubCenter == null)
            return NotFound();

        await _service.DeleteAsync(id);
        return Json(new { success = true });
    }

}