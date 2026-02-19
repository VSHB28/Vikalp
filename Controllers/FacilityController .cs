using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;

public class FacilityController : Controller
{
    private readonly IFacilityService _facilityService;
    private readonly IDropdownService _dropdownService;
    private readonly ILocationBlockService _blockService;

    public FacilityController(
        IFacilityService facilityService,
        IDropdownService dropdownService,
        ILocationBlockService blockService)
    {
        _facilityService = facilityService;
        _dropdownService = dropdownService;
        _blockService = blockService;
    }

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

        ViewBag.yesNo = dropdowns.ContainsKey("YesNo")
            ? dropdowns["YesNo"]
            : new List<SelectListItem>();
        return View();
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

    public async Task<JsonResult> GetFacilities(int blockId)
    {
        var data = await _facilityService.GetByBlockAsync(blockId);       
        var types = _dropdownService.GetFacilityTypes()
                    .ToDictionary(t => t.Id.ToString(), t => t.Name); 

        var result = data.Select(f => new
        {
            f.FacilityId,
            f.FacilityName,
            facilityType = types.ContainsKey(f.FacilityType) ? types[f.FacilityType] : "", 
            f.NinNumber,
            f.IsActive,
            f.IsIntervention,
            f.ProfileId,
            f.HrId
        });
        var dropdowns = await _dropdownService.GetCommonDropdownsfamilyplanningAsync(userId: 1, languageId: 1);
        ViewBag.designation = dropdowns["Designation"];
        ViewBag.gender = dropdowns["Gender"];
        ViewBag.yesNo = dropdowns["YesNo"];
        return Json(result);
    }

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

    // GET: Create
    public IActionResult Create()
    {
        ViewBag.States = _dropdownService.GetStates();
        ViewBag.FacilityTypes = _dropdownService.GetFacilityTypes();
        return View(new FacilityDto());
    }

    // POST: Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FacilityDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.States = _dropdownService.GetStates();
            if (dto.RegionId > 0) ViewBag.Districts = _dropdownService.GetDistricts(dto.RegionId);
            if (dto.DistrictId > 0) ViewBag.Blocks = _dropdownService.GetBlocks(dto.DistrictId);

            return View(dto);
        }

        await _facilityService.AddAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    // GET: Edit
    public async Task<IActionResult> Edit(int id)
    {
        var facility = await _facilityService.GetByIdAsync(id);
        if (facility == null) return NotFound();

        ViewBag.States = _dropdownService.GetStates();

        if (facility.BlockId > 0)
        {
            var block = await _blockService.GetByIdAsync(facility.BlockId);
            if (block != null)
            {
                facility.RegionId = block.RegionId;
                facility.DistrictId = block.DistrictId;

                ViewBag.Districts = _dropdownService.GetDistricts(block.RegionId);
                ViewBag.Blocks = _dropdownService.GetBlocks(block.DistrictId);
            }
        }
        ViewBag.FacilityTypes = _dropdownService.GetFacilityTypes();

        return View(facility);
    }

    // POST: Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(FacilityDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.States = _dropdownService.GetStates();
            if (dto.RegionId > 0) ViewBag.Districts = _dropdownService.GetDistricts(dto.RegionId);
            if (dto.DistrictId > 0) ViewBag.Blocks = _dropdownService.GetBlocks(dto.DistrictId);
            return View(dto);
        }

        if (dto.FacilityId <= 0) return BadRequest("Facility ID missing");

        await _facilityService.UpdateAsync(dto);
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var facility = await _facilityService.GetByIdAsync(id);
        if (facility == null)
            return NotFound();

        return View(facility); 
    }

    // POST: Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var facility = await _facilityService.GetByIdAsync(id);
        if (facility == null)
            return NotFound();

        await _facilityService.DeleteAsync(id);
        return Json(new { success = true }); 
    }

    [HttpPost]
    public async Task<IActionResult> SaveFacilityProfile([FromBody] FacilityProfileDto model)
    {
        if (model == null)
            return BadRequest("Model is null");

        await _facilityService.SaveFacilityProfileAsync(model);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetFacilityProfile(int profileId)
    {
        var result = await _facilityService.GetFacilityProfileAsync(profileId);

        if (result == null)
            return NotFound();

        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> SaveHrStatus([FromBody] HrStatusDto model)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        model.CreatedBy = userId;
        await _facilityService.SaveHrStatusAsync(model);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetHrStatus(int hrId)
    {
        var result = await _facilityService.GetHrStatusAsync(hrId);
        return Json(result);
    }

}