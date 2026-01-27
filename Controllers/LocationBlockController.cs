using Microsoft.AspNetCore.Mvc;
using Vikalp.Service.Interfaces;

namespace Vikalp.Controllers
{
    public class LocationBlockController : Controller
    {
        private readonly ILocationBlockService _blockService;
        private readonly IDropdownService _dropdownService;

        public LocationBlockController(
            ILocationBlockService blockService,
            IDropdownService dropdownService)
        {
            _blockService = blockService;
            _dropdownService = dropdownService;
        }

        public IActionResult Index()
        {
            ViewBag.States = _dropdownService.GetStates();
            return View();
        }

       
        public JsonResult GetDistricts(int stateId)
        {
            var districts = _dropdownService.GetDistricts(stateId);
            return Json(districts);
        }

    
        public async Task<JsonResult> GetBlocks(int districtId)
        {
            var blocks = await _blockService.GetBlocksByDistrictAsync(districtId);
            return Json(blocks);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.States = _dropdownService.GetStates();
            return View(new LocationBlockDto());
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LocationBlockDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.States = _dropdownService.GetStates();
                return View(model);
            }

            await _blockService.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var block = await _blockService.GetByIdAsync(id);
            if (block == null)
                return NotFound();

            ViewBag.States = _dropdownService.GetStates();

            ViewBag.SelectedStateId = block.RegionId;
            ViewBag.SelectedDistrictId = block.DistrictId;

            ViewBag.Districts = _dropdownService.GetDistricts(block.RegionId);

            return View(block);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LocationBlockDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.States = _dropdownService.GetStates();
                ViewBag.Districts = _dropdownService.GetDistricts(dto.RegionId);
                return View(dto);
            }

            await _blockService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

      
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var block = await _blockService.GetByIdAsync(id);
            if (block == null)
                return NotFound();

            return View(block);
        }

        // POST: Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var block = await _blockService.GetByIdAsync(id);
            if (block == null)
                return NotFound();

            await _blockService.DeleteAsync(id);
            return Json(new { success = true });
        }

    }
}
