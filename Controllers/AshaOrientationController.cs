using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using Vikalp.Models.DTO;
using Vikalp.Service;
using Vikalp.Service.Interfaces;

namespace Vikalp.Controllers
{
    public class AshaOrientationController : Controller
    {
        private readonly IAshaOrientationService _service;
        private readonly ILogger<AshaOrientationController> _logger;
        private readonly IDropdownService _dropdownService;

        public AshaOrientationController(IAshaOrientationService service, IDropdownService dropdownService)
        {
            _service = service;
            _dropdownService = dropdownService;
        }

        // ===================== LIST =====================
        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllAsync();
            return View(list);
        }

        // ===================== CREATE (GET) =====================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            LoadMasters();
            return View(new AshaOrientationCreateDto());
        }

        // ===================== CREATE (POST) =====================        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJson([FromBody] AshaOrientationCreateDto model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (model == null)
                return BadRequest("Invalid data");

            if (model.DateofOrientation == default)
                return BadRequest("Orientation date is required");

            if (model.Attendees == null || model.Attendees.Count == 0)
                return BadRequest("At least one ASHA is required");

            var result = await _service.SaveOrientationJsonAsync(model, userId);

            if (!result)
                return StatusCode(500, "Save failed");

            return Ok(new { success = true });
        }


        [HttpGet]
        public async Task<IActionResult> SearchFacilities(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<object>());

            var result = await _service.SearchFacilitiesAsync(term);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAshasByFacility(int facilityId)
        {
            var result = await _service.GetAshasByFacilityAsync(facilityId);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAshaDetailsSearch(int ashaId)
        {
            var result = await _service.GetAshaDetailsAsync(ashaId);
            return Json(result);
        }

        // EDIT (GET) → reuse Create.cshtml
        public async Task<IActionResult> Edit(string venueGuid)
        {
            LoadMasters();
            var model = await _service.GetOrientationForEditAsync(venueGuid);

            if (model == null)
                return NotFound();

            ViewBag.IsEdit = true;
            return View("Create", model);
        }

        public async Task<IActionResult> Details(string venueGuid)
        {
            var model = await _service.GetOrientationForEditAsync(venueGuid);

            if (model == null)
                return NotFound();

            return View(model);
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
            ViewBag.Topics = _dropdownService.GetTopicsCovered(1, 1);
        }
    }
}
