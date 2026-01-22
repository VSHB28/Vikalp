using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Create(AshaOrientationCreateDto model)
        {
            string? facilityName = null;

            if (model.IsIntervention == 1 && model.FacilityId.HasValue)
            {
                var facility = await _service.GetFacilityByIdAsync(model.FacilityId.Value);
                facilityName = facility?.FacilityName;
            }
            else
            {
                facilityName = model.FacilityName;
            }

            string venueGuid = Guid.NewGuid().ToString();

            // 1️⃣ Save Venue
            var venueDto = new OrientationVenueDetailsDto
            {
                VenueGuid = venueGuid,
                IsIntervention = model.IsIntervention,
                StateId = model.StateId,
                DistrictId = model.DistrictId,
                BlockId = model.BlockId,
                FacilityId = model.IsIntervention == 1 ? model.FacilityId : null,
                FacilityName = facilityName!, // NOT NULL
                DateofOrientation = model.DateofOrientation,
                NIN = model.IsIntervention == 0 ? model.NIN : null,
                CreatedBy = 1
            };

            await _service.SaveVenueAsync(venueDto);

            // 2️⃣ Save ASHA Rows
            foreach (var a in model.Attendees)
            {
                string orientationGuid = Guid.NewGuid().ToString();

                var dto = new AshaOrientationDto
                {
                    VenueGuid = venueGuid,
                    OrientationGuid = orientationGuid,            // REQUIRED
                    IsIntervention = model.IsIntervention,        // REQUIRED

                    AshaId = a.AshaId,
                    AshaName = a.AshaName!,                       // NOT NULL
                    AshaMobile = a.AshaMobile,

                    FacilityId = (int)model.FacilityId,
                    FacilityName = facilityName!,                 // REQUIRED

                    VCAT_PreTest = a.VCAT_PreTest,
                    VCAT_PostTest = a.VCAT_PostTest,
                    IsOrientation = a.IsOrientation ? 1 : 0,

                    NIN = model.IsIntervention == 0 ? model.NIN : null,
                    CreatedBy = 1
                };

                await _service.SaveAshaOrientationAsync(dto);
            }

            return RedirectToAction("Index");
        }

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
        }
    }
}
