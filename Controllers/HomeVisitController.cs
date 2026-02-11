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
        var model = await _service.GetAllAsync(); // ✅ await resolves the task
        return View(model);
    }

    // ===================== CREATE =====================

    [HttpGet]
    public async Task<IActionResult> Create(Guid guid)
    {
        LoadMasters();
        var dropdowns = await _dropdownService.GetCommonDropdownsfamilyplanningAsync(userId: 1, languageId: 1);

        ViewBag.familyplanning = dropdowns["Familyplanning"];
        ViewBag.yesNo = dropdowns["YesNo"];
        ViewBag.socialbenifit = dropdowns["SocialBenifit"];
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var survey = await _service.GetByIdAsync(Guid.Parse("6B737A2C-7F42-4302-BAE3-AA14F0E8C381"), userId);

        if (survey == null)
            return NotFound();

        return View("Create", survey);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] HomeVisitDTO model)
    {
        if (model == null)
        {
            return Json(new { success = false, message = "No beneficiaries found" });
        }

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        bool result = await _service.InsertAsync(model);

        return Json(new { success = result });
    }


    // ===================== EDIT =====================
    [HttpGet]
    public IActionResult Edit(int id)
    {
        //var survey = _service.GetByIdAsync(id);
        //if (survey == null) return NotFound();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, HomeVisitDTO model)
    {
        if (id != model.VisitId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                model.UpdatedBy = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                model.UpdatedOn = DateTime.Now;

                _service.UpdateAsync(model);   // calls SP
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating survey");
                ModelState.AddModelError("", "Unable to save changes.");
            }
        }
        return View(model);
    }

    // ===================== DELETE =====================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        try
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            _service.DeleteAsync(id);   // calls SP
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting survey");
            return BadRequest("Unable to delete record.");
        }
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
