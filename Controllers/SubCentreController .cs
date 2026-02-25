using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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


    public IActionResult Index()
    {
        ViewBag.States = _dropdownService.GetStates();

        ViewBag.designation = _dropdownService.GetDropdown("Designation") ?? new List<SelectListItem>();
        ViewBag.gender = _dropdownService.GetDropdown("Gender") ?? new List<SelectListItem>();

        var yesNo = _dropdownService.GetDropdown("YesNo"); // example method
        ViewBag.yesNo = yesNo ?? new List<SelectListItem>();

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



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SubCentreDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.States = _dropdownService.GetStates();
            return View(dto);
        }

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



    [HttpPost]
    public async Task<IActionResult> SaveFacilityProfile([FromBody] FacilityProfileDto model)
    {
        if (model == null)
            return BadRequest("Model is null");

        await _service.SaveFacilityProfileAsync(model);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetFacilityProfile(int profileId)
    {
        var result = await _service.GetFacilityProfileAsync(profileId);

        if (result == null)
            return NotFound();

        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> SaveHrStatus([FromBody] HrStatusDto model)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        model.CreatedBy = userId;
        await _service.SaveHrStatusAsync(model);
        return Ok();
    }

}