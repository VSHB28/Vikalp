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
        [HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    LoadMasters();
        //    var list = await _service.GetAllAsync();
        //    return View(list);
        //}
        public async Task<IActionResult> Index(
    int page = 1,
    int pageSize = 10,
    int? StateId = null,
    int? DistrictId = null,
    int? BlockId = null,
    int? FacilityId = null,
    DateTime? OrientationDate = null)
        {
            LoadMasters();
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var result = await _service.GetPagedAsync(
                userId,
                page,
                pageSize,
                StateId,
                DistrictId,
                BlockId,
                FacilityId,
                OrientationDate);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRecords = result.TotalCount;
            ViewBag.TotalPages =
                (int)Math.Ceiling((double)result.TotalCount / pageSize);

            // preserve filters
            ViewBag.StateId = StateId;
            ViewBag.DistrictId = DistrictId;
            ViewBag.BlockId = BlockId;
            ViewBag.FacilityId = FacilityId;
            ViewBag.OrientationDate = OrientationDate;

            return View(result.Data);
        }
        //public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        //{
        //    LoadMasters();
        //    var list = await _service.GetAllAsync();

        //    var totalRecords = list.Count();

        //    var pagedData = list
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToList();

        //    ViewBag.CurrentPage = page;
        //    ViewBag.PageSize = pageSize;
        //    ViewBag.TotalRecords = totalRecords;
        //    ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

        //    return View(pagedData);
        //}


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



        // ===================== Edit (GET) =====================

        [HttpGet]
        public async Task<IActionResult> Edit(string venueGuid)
        {
            LoadMasters();
            var model = await _service.GetOrientationForEditAsync(venueGuid);

            if (model == null)
                return NotFound();

            ViewBag.IsEdit = true;
            return View("Update", model);
        }

        // ===================== Edit (POST) =====================

        [HttpPost]
        public async Task<IActionResult> UpdateJson([FromBody] AshaOrientationCreateDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false });
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _service.UpdateOrientationAsync(userId, model);

            return Json(new { success = result });
        }


        //===================== Delete Orientation (POST) =====================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([FromBody] int venueId)
        {
            if (venueId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid user id"
                });
            }

            try
            {
                _service.Delete(venueId);

                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
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
