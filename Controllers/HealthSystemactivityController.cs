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
        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllAsync();
            return View(list);
        }

        // ===================== CREATE (GET) =====================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ===================== CREATE (POST) =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(HealthSystemActivityDto dto)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
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
