using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using Vikalp.Models.DTO;
using Vikalp.Service;
using Vikalp.Service.Interfaces;

namespace Vikalp.Controllers
{
    public class FacilityProfileController : Controller
    {
        private readonly IFacilityProfileService _service;
        private readonly ILogger<FacilityProfileController> _logger;
        private readonly IDropdownService _dropdownService;

        public FacilityProfileController(IFacilityProfileService service, IDropdownService dropdownService)
        {
            _service = service;
            _dropdownService = dropdownService;
        }

        // ===================== LIST =====================
        public IActionResult Index()
        {
            //var profiles = _service.FacilityProfiles.ToList();
            return View();
        }

        // GET: FacilityProfile/Create
        public IActionResult Create()
        {
            return View();
        }

        //// POST: FacilityProfile/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(FacilityProfile model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        model.CreatedOn = DateTime.Now;
        //        _context.FacilityProfiles.Add(model);
        //        _context.SaveChanges();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View();
        //}

        //// GET: FacilityProfile/Edit/5
        //public IActionResult Edit(int id)
        //{
        //    var profile = _context.FacilityProfiles.Find(id);
        //    if (profile == null) return NotFound();
        //    return View(profile);
        //}

        //// POST: FacilityProfile/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(int id, FacilityProfile model)
        //{
        //    if (id != model.ProfileId) return NotFound();

        //    if (ModelState.IsValid)
        //    {
        //        var existing = _context.FacilityProfiles.Find(id);
        //        if (existing == null) return NotFound();

        //        // Map updated fields
        //        existing.FacilityId = model.FacilityId;
        //        existing.SubCenterId = model.SubCenterId;
        //        existing.PopulationCoveredByPHC = model.PopulationCoveredByPHC;
        //        existing.NumberOfHSC = model.NumberOfHSC;
        //        existing.PopulationCoveredByHWC = model.PopulationCoveredByHWC;
        //        existing.AverageOPDPerDay = model.AverageOPDPerDay;
        //        existing.NearestFacilityReferral = model.NearestFacilityReferral;
        //        existing.DistanceFromPHC = model.DistanceFromPHC;
        //        existing.IsDeliveryPoint = model.IsDeliveryPoint;
        //        existing.AvgDeliveryPerMonth = model.AvgDeliveryPerMonth;
        //        existing.DistanceFromDH = model.DistanceFromDH;
        //        existing.IsSeparateSpaceForFp = model.IsSeparateSpaceForFp;
        //        existing.UpdatedBy = model.UpdatedBy;
        //        existing.UpdatedOn = DateTime.Now;

        //        _context.SaveChanges();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(model);
        //}
    }
}
