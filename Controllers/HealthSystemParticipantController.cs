using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using Vikalp.Models.DTO;
using Vikalp.Service;
using Vikalp.Service.Interfaces;

namespace Vikalp.Controllers
{
    public class HealthSystemParticipantController : Controller
    {
        private readonly IHealthSystemService _service;
        private readonly ILogger<HealthSystemactivityController> _logger;
        private readonly IDropdownService _dropdownService;

        public HealthSystemParticipantController(IHealthSystemService service, IDropdownService dropdownService)
        {
            _service = service;
            _dropdownService = dropdownService;
        }

        // ===================== LIST =====================
        public async Task<IActionResult> Index()
        {
            var activities = await _service.GetAllParticipantAsync();
            return View(activities);
        }

        // ===================== CREATE (GET) =====================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            LoadMasters();
            var dropdowns = await _dropdownService.GetCommonDropdownsAsync(userId: 1, languageId: 1);

            ViewBag.ActivityTypeName = dropdowns["ActivityTypeName"];
            ViewBag.ActivityType = dropdowns["ActivityType"];
            ViewBag.ActivityFormat = dropdowns["ActivityFormat"];
            ViewBag.Clinical = dropdowns["Clinical"];
            ViewBag.NonClinical = dropdowns["NonClinical"];
            ViewBag.FacilityType = dropdowns["FacilityType"];
            ViewBag.ProviderType = dropdowns["ProviderType"];
            ViewBag.YesNo = dropdowns["YesNo"];


            return View("Create", new HealthSystemParticipantDto());
        }

        // ===================== CREATE (POST) =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJson([FromBody] HealthSystemParticipantDto model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (model == null)
                return BadRequest("Invalid data");

            var result = await _service.SaveHealthSystemParticipantJsonAsync(model, userId);

            if (!result)
                return StatusCode(500, "Save failed");

            return Ok(new { success = true });
        }

        // ===================== AJAX APIs =====================

        [HttpGet]
        public async Task<IActionResult> SearchFacilities(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<object>());

            var result = await _service.SearchFacilitiesAsync(term);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetParticipantByFacility(int facilityId)
        {
            var result = await _service.GetparticipantByFacilityAsync(facilityId);
            return Json(result);
        }

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

        [HttpGet]
        public async Task<IActionResult> GetFacilityById(int facilityId)
        {
            var data = await _service.GetFacilityByIdAsync(facilityId);
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
