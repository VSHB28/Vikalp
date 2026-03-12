using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Vikalp.Service;

namespace Vikalp.Controllers
{
    public class MeetingController : Controller
    {
        private readonly IMeetingService _service;
        private readonly ILogger<HealthSystemactivityController> _logger;
        private readonly IDropdownService _dropdownService;

        public MeetingController(IMeetingService service, IDropdownService dropdownService)
        {
            _service = service;
            _dropdownService = dropdownService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
    int page = 1,
    int pageSize = 10,
    int? StateId = null,
    int? DistrictId = null,
    int? BlockId = null,
    int? FacilityId = null)
        {
            // Load dropdowns for filters
            LoadMasters();

            // Get userId safely
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            // Call service with all filters
            
            var result = await _service.GetAllAsync(
                userId,
                page,
                pageSize,
                StateId,
                DistrictId,
                BlockId,
                FacilityId);

            // Pagination info
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRecords = result.TotalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);

            // Preserve filters for view
            ViewBag.StateId = StateId;
            ViewBag.DistrictId = DistrictId;
            ViewBag.BlockId = BlockId;
            ViewBag.FacilityId = FacilityId;

            return View(result.Data);
        }

        // ===================== CREATE (GET) =====================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            LoadMasters();
            var dropdowns = await _dropdownService.GetMeetingCommonDropdownsAsync(userId: 1, languageId: 1);
            ViewBag.blockParticipant = dropdowns["BlockParticipant"];
            ViewBag.yesNo = dropdowns["YesNo"];
            ViewBag.meetinplatform = dropdowns["MeetingPlatform"];
            ViewBag.sectorparticipant = dropdowns["SectorParticipant"];

            return View("Create", new MeetingDto());
        }

        // ===================== CREATE (POST JSON) =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJson([FromBody] MeetingDto model)
        {
            if (model == null)
                return BadRequest("Invalid data");

            if (model.MeetingType == null || model.MeetingType == 0)
                return BadRequest("Event type is required");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var result = await _service.CreateAsync(model, userId);

            if (result)
                return Json(new { success = true });

            return Json(new
            {
                success = false,
                message = "Save failed"
            });
        }

        // ===================== Edit (GET) =====================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            LoadMasters();
            var dropdowns = await _dropdownService.GetMeetingCommonDropdownsAsync(userId: 1, languageId: 1);
            ViewBag.blockParticipant = dropdowns["BlockParticipant"];
            ViewBag.yesNo = dropdowns["YesNo"];
            ViewBag.meetinplatform = dropdowns["MeetingPlatform"];
            ViewBag.sectorparticipant = dropdowns["SectorParticipant"];
            var model = await _service.GetByIdAsync(id);

            if (model == null)
                return NotFound();

            ViewBag.IsEdit = true;
            return View("Update", model);
        }

        // ===================== Edit (POST) =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateJson([FromBody] MeetingDto model)
        {
            if (model == null || model.MeetingId == 0)
                return BadRequest();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _service.UpdateAsync(model, userId);

            return Json(new { success = result });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var success = await _service.DeleteAsync(id, userId);
            if (!success) return NotFound();
            return NoContent();
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

        private void LoadMasters()
        {
            ViewBag.MeetingList = _dropdownService.GetMeetings();
            ViewBag.States = _dropdownService.GetStates();
            ViewBag.Ashas = _dropdownService.GetAshas();
        }
    }
}
