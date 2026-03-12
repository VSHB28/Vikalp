using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using Vikalp.Models.DTO;
using Vikalp.Models.DTO;
using Vikalp.Service;
using Vikalp.Service.Implementations;
using Vikalp.Service.Interfaces;
using Vikalp.Service.Interfaces;

namespace Vikalp.Controllers
{

    public class AshaController : Controller
    {
        private readonly IAshaService _ashaService;
        private readonly IDropdownService _dropdownService;

        public AshaController(
            IAshaService service,
            IDropdownService dropdownService)
        {
            _ashaService = service;         
            _dropdownService = dropdownService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            ViewBag.States = _dropdownService.GetStates();

            var result = await _ashaService.GetAllAsha(page, pageSize);

            ViewBag.PageNumber = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = result.TotalCount;

            return View(result.Data);
        }

        [HttpGet]
        public JsonResult GetDistricts(int stateId)
        {
            var districts = _dropdownService.GetDistricts(stateId);
            if (districts == null) return Json(new List<object>());

            var data = districts.Select(d => new
            {
                id = d.Id,
                name = d.Name
            }).ToList();

            return Json(data);
        }

        public JsonResult GetBlocks(int districtId)
        {
            var data = _dropdownService.GetBlocks(districtId)
                .Select(b => new
                {
                    id = b.Id,
                    name = b.Name
                });

            return Json(data);
        }

        public JsonResult GetFacilities(int blockId)
        {
            var data = _dropdownService.GetFacilities(blockId)
                .Select(f => new
                {
                    id = f.Id,
                    name = f.Name
                });

            return Json(data);
        }


        [HttpGet]
        public async Task<JsonResult> GetByFilter(int? stateId, int? districtId, int? blockId, int? facilityId, int? subCentreId)
        {
            var data = await _ashaService.GetByFilter(stateId, districtId, blockId, facilityId, subCentreId);

            var result = data.Select(x => new
            {
                ashaId = x.AshaId,
                ashaName = x.AshaName,
                ashaMobile = x.AshaMobile,
                isActive = x.IsActive,
                attendedVCAT = x.AttendedVCAT
            });

            return Json(result);
        }

        public JsonResult GetSubCentres(int facilityId)
        {
            int userId = 1;

            var data = _dropdownService.GetSubCentre(facilityId, userId)
                .Select(s => new
                {
                    id = s.Id,
                    name = s.Name
                });

            return Json(data);
        }

        public IActionResult Create()
        {
            ViewBag.States = _dropdownService.GetStates();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AshaDto model)
        {
            if (ModelState.IsValid)
            {
                await _ashaService.Insert(model, 0);
                return RedirectToAction("Index");
            }

            ViewBag.States = _dropdownService.GetStates();   

            return View(model);
        }
     
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _ashaService.GetById(id);
          

            if (data == null)
                return NotFound();

            ViewBag.States = _dropdownService.GetStates();
            ViewBag.Districts = _dropdownService.GetDistricts(data.StateId ?? 0);
            ViewBag.Blocks = _dropdownService.GetBlocks(data.DistrictId ?? 0);
            ViewBag.Facilities = _dropdownService.GetFacilities(data.BlockId ?? 0);
            ViewBag.SubCentres = _dropdownService.GetSubCentre(data.FacilityId ?? 0, 1);

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AshaDto model)
        {
            if (ModelState.IsValid)
            {

                int userId = 1;

                await _ashaService.Update(model, userId);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _ashaService.GetById(id);

            if (data == null)
                return NotFound();

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _ashaService.DeleteAsync(id);
            return Json(new { success = true });
        }

    }
}
