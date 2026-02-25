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
        [HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    var activities = await _service.GetAllParticipantAsync();
        //    return View(activities);
        //}

        public async Task<IActionResult> Index(
    int page = 1,
    int pageSize = 10,
    int? StateId = null,
    int? DistrictId = null,
    int? BlockId = null,
    int? FacilityId = null)
        {
            LoadMasters();
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var result = await _service.GetAllParticipantAsync(
                userId,
                page,
                pageSize,
                StateId,
                DistrictId,
                BlockId,
                FacilityId);

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
            ViewBag.FacilityType = dropdowns["FacilityType"];
            ViewBag.ProviderType = dropdowns["ProviderType"];
            ViewBag.YesNo = dropdowns["YesNo"];


            return View("Create", new HealthSystemParticipantDto());
        }

        // ===================== CREATE (POST) =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveParticipantsJson([FromBody] HealthSystemParticipantSaveDto model)
        {
            if (model == null || model.Participants == null || !model.Participants.Any())
                return Json(new { success = false, message = "No participants found" });

            int userId = 1; // later from claims

            bool result = await _service.SaveParticipantsAsync(
                model.DateofActivity,
                model.StateId,
                model.DistrictId,
                model.FacilityTypeId,
                model.FacilityTypeOther,
                model.Participants,
                userId
            );

            return Json(new { success = result });
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
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


            //var model = await _service.GetParticipantsByIdAsync(id);

            //if (model == null)
            //    return NotFound();

            //return View(model);
            var model = await _service.GetParticipantsByIdAsync(id);
            Console.WriteLine(model.Participants.First().InterventionFacility);
            if (model?.Participants != null && model.Participants.Any())
            {
                model.InterventionFacility = model.Participants.First().InterventionFacility;
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateParticipantsJson([FromBody] HealthSystemParticipantSaveDto model)
        {
            if (model == null || model.Participants == null || !model.Participants.Any())
                return Json(new { success = false });

            int userId = 1;

            bool result = await _service.UpdateParticipantsAsync(
            model.ActivityDate ?? DateTime.Now,
            model.StateId,
            model.DistrictId,
            model.FacilityTypeId,
            model.ActivityTypeId,
            model.FacilityTypeOther,
            model.ActivityNameId,
            model.ProviderTypeId,
            model.Remarks,
            model.Participants,
            userId
        );

            return Json(new { success = result });
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
