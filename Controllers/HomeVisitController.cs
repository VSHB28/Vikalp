using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vikalp.Controllers;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;


public class HomeVisitController : Controller
{
    private readonly IHomeVisitService _service;
    private readonly IDropdownService _dropdownService;
    private readonly ILogger<LineListingSurveyController> _logger;

    public HomeVisitController(IHomeVisitService service, IDropdownService dropdownService, ILogger<LineListingSurveyController> logger)
    {
        _service = service;
        _dropdownService = dropdownService;
        _logger = logger;
    }

    // ===================== LIST =====================
    public async Task<IActionResult> Index()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var model = await _service.GetAllAsync(userId); // ✅ await resolves the task
        return View(model);
    }

    // ===================== CREATE =====================

    [HttpGet("HomeVisit/Create/{guid}")]
    public async Task<IActionResult> Create(Guid guid)
    {
        LoadMasters();
        var dropdowns = await _dropdownService.GetCommonDropdownsfamilyplanningAsync(userId: 1, languageId: 1);

        ViewBag.familyplanning = dropdowns["Familyplanning"];
        ViewBag.yesNo = dropdowns["YesNo"];
        ViewBag.socialbenifit = dropdowns["SocialBenifit"];
        ViewBag.FacilityType = dropdowns["FacilityType"];

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var survey = await _service.GetByIdAsync(guid, userId);

        if (survey == null)
            return NotFound();

        return View("Create", survey);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HomeVisitDTO model)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, message = "Invalid data" });

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        bool result;

        result = await _service.SaveHomeVisitAsync(model, userId);

        return Json(new { success = result });
    }

    // ===================== DELETE =====================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting survey");
            return BadRequest("Unable to delete record.");
        }
    }

    // ===================== Followup =====================
    [HttpGet("HomeVisit/Followup/{guid}")]
    public async Task<IActionResult> Followup(Guid guid)
    {
        LoadMasters();

        var dropdowns = await _dropdownService
            .GetCommonDropdownsfamilyplanningAsync(userId: 1, languageId: 1);

        ViewBag.familyplanning = dropdowns["Familyplanning"];
        ViewBag.yesNo = dropdowns["YesNo"];
        ViewBag.socialbenifit = dropdowns["SocialBenifit"];
        ViewBag.callstatus = dropdowns["CallStatus"];
        ViewBag.FacilityType = dropdowns["FacilityType"];

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // 🔹 Load main HomeVisit data
        var homeVisit = await _service.GetByIdAsync(guid, userId);

        if (homeVisit == null)
            return NotFound();   // IMPORTANT

        // 🔹 Load follow-up history
        var followUps = await _service.GetFollowUpHistoryAsync(guid, userId);

        homeVisit.FollowUpHistory = followUps ?? new List<HomevisitFollowUpDto>();

        return View("Followup", homeVisit);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveFollowup([FromBody] HomevisitFollowUpDto model)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, message = "Invalid data" });

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        model.CreatedBy = userId;

        bool result = await _service.InsertFollowUpAsync(model);

        return Json(new
        {
            success = result,
            message = result ? "Followup saved successfully" : "Failed to save"
        });
    }


    // ===================== AJAX APIs =====================

    public IActionResult GetDistricts(int stateId)
    {
        var data = _dropdownService.GetDistricts(stateId);
        return Json(data);
    }

    public IActionResult GetBlocks(int districtId)
    {
        var data = _dropdownService.GetBlocks(districtId);
        return Json(data);
    }

    public IActionResult GetFacilities(int blockId)
    {
        var data = _dropdownService.GetFacilities(blockId);
        return Json(data);
    }

    public IActionResult GetSubCentre(int blockId)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var data = _dropdownService.GetSubCentre(blockId, userId);
        return Json(data);
    }
    public IActionResult GetAshabySubcentre(int subcentreId)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var data = _dropdownService.GetAshaDetailsbySubcentre(subcentreId, userId);
        return Json(data);
    }
    public IActionResult GetAshaDetails(int ashaId)
    {
        var data = _dropdownService.GetAshaDetails(ashaId);
        return Json(data);
    }

    private void LoadMasters()
    {
        ViewBag.States = _dropdownService.GetStates();
        ViewBag.Ashas = _dropdownService.GetAshas();
        ViewBag.Genders = _dropdownService.GetGenders();
    }
}
