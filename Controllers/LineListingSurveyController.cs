using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vikalp.Models.DTO;
using Vikalp.Service;
using Vikalp.Service.Interfaces;

namespace Vikalp.Controllers
{
    public class LineListingSurveyController : Controller
    {
        private readonly ILineListingSurveyService _service;
        private readonly IDropdownService _dropdownService;
        private readonly ILogger<LineListingSurveyController> _logger;

        public LineListingSurveyController(ILineListingSurveyService service, IDropdownService dropdownService, ILogger<LineListingSurveyController> logger)
        {
            _service = service;
            _dropdownService = dropdownService;
            _logger = logger;
        }

        // ===================== LIST =====================
        public IActionResult Index()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var surveys = _service.GetAllSurveys(userId);
            return View(surveys);
        }

        // ===================== CREATE =====================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            LoadMasters();
            var dropdowns = await _dropdownService.GetCommonDropdownsfamilyplanningAsync(userId: 1, languageId: 1);

            ViewBag.familyplanning = dropdowns["Familyplanning"];
            ViewBag.yesNo = dropdowns["YesNo"];

            return View("Create", new LineListingSurveyDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] LineListingSurveyCreateDto model)
        {
            if (model == null || model.Women == null || !model.Women.Any())
            {
                return Json(new { success = false, message = "No beneficiaries found" });
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            bool result = await _service.InsertLineListingAsync(model, userId);

            return Json(new { success = result });
        }


        // ===================== EDIT =====================

        // 1. GET: View Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var data = await _service.GetLineListingByIdAsync(id);
            if (data == null) return NotFound();

            return View(data);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _service.GetLineListingByIdAsync(id);
            if (model == null) return NotFound();

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");


            ViewBag.States = _dropdownService.GetStates();


            if (model.StateId > 0)
                ViewBag.Districts = _dropdownService.GetDistricts(model.StateId.Value);

            if (model.DistrictId > 0)
                ViewBag.Blocks = _dropdownService.GetBlocks(model.DistrictId.Value);

            if (model.BlockId > 0)
            {

                ViewBag.Facilities = _dropdownService.GetFacilities(model.BlockId.Value);
                ViewBag.SubCentres = _dropdownService.GetSubCentre(model.BlockId.Value, userId);
            }

            if (model.SubCenterId > 0)
            {

                ViewBag.Ashas = _dropdownService.GetAshaDetailsbySubcentre(model.SubCenterId.Value, userId);
            }


            var dropdowns = await _dropdownService.GetCommonDropdownsfamilyplanningAsync(userId: 1, languageId: 1);
            ViewBag.familyplanning = dropdowns["Familyplanning"];
            ViewBag.yesNo = dropdowns["YesNo"];

            return View("Edit", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] LineListingSurveyUpdateDto model)
        {
            if (model == null || model.Women == null)
            {
                return Json(new { success = false, message = "Invalid data" });
            }


            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            bool result = await _service.UpdateSurvey(model, userId);

            return Json(new { success = result, message = result ? "Updated successfully" : "Update failed" });
        }


        // ===================== Concent =====================
        [HttpGet]
        public async Task<IActionResult> Consent(string guid)
        {
            var model = await _service.GetConsentPrefillAsync(guid);
            if (model == null) return NotFound();
            ViewBag.gender = _dropdownService.GetGenders();
            var dropdowns = await _dropdownService.GetCommonDropdownsfamilyplanningAsync(userId: 1, languageId: 1);

            ViewBag.familyplanning = dropdowns["Familyplanning"];
            ViewBag.yesNo = dropdowns["YesNo"];
            ViewBag.whoattandcall = dropdowns["WhoAttandcall"];
            return View("Consent", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveConsentJson([FromBody] LineListingConsentDto model)
        {
            if (model == null)
                return Json(new { success = false, message = "Invalid data" });

            int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _service.SaveConsentAsync(model, UserId);

            return Json(new { success = true });
        }

        // ===================== DELETE =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                _service.DeleteSurvey(id, userId);   // calls SP
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
}