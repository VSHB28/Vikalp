using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Vikalp.Models.DTO;
using Vikalp.Service;
using Vikalp.Service.Interfaces;

namespace Vikalp.Controllers
{
    public class HealthSystemactivityController : Controller
    {
        private readonly IHealthSystemService _service;
        private readonly ILogger<HealthSystemactivityController> _logger;
        private readonly IDropdownService _dropdownService;

        public HealthSystemactivityController(IHealthSystemService service, IDropdownService dropdownService)
        {
            _service = service;
            _dropdownService = dropdownService;
        }

        // ===================== LIST =====================
        [HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    var activities = await _service.GetAllAsync();
        //    return View(activities);
        //}

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            int userId = 1; // from session

            var result = await _service.GetPagedAsync(userId, page, pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRecords = result.TotalCount;
            ViewBag.TotalPages =
                (int)Math.Ceiling((double)result.TotalCount / pageSize);

            return View(result.Data);
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

            return View("Create", new HealthSystemActivityDto());
        }

        // ===================== CREATE (POST) =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJson([FromBody] HealthSystemActivityDto model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (model == null)
                return BadRequest("Invalid data");

            if (model.Activities == null || model.Activities.Count == 0)
                return BadRequest("At least one Activity is required");

            var result = await _service.SaveHealthSystemActivityJsonAsync(model, userId);

            if (!result)
                return StatusCode(500, "Save failed");

            return Ok(new { success = true });
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

        public IActionResult GetAshaDetails(int ashaId)
        {
            var data = _dropdownService.GetAshaDetails(ashaId);
            return Json(data);
        }

        private void LoadMasters()
        {
            ViewBag.States = _dropdownService.GetStates();
            ViewBag.Ashas = _dropdownService.GetAshas();
        }
    }
}
