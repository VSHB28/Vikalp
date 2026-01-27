using Microsoft.AspNetCore.Mvc;
using Vikalp.DTO;
using Vikalp.Service.Interfaces;

public class LocationDistrictController : Controller
{
    private readonly ILocationDistrictService _service;
    private readonly IDropdownService _dropdownService;

    public LocationDistrictController(
        ILocationDistrictService service,
        IDropdownService dropdownService
    )
    {
        _service = service;
        _dropdownService = dropdownService;
    }

    public IActionResult Index(int? stateId)
    {
        var states = _dropdownService.GetStates();
        ViewBag.StateList = states;

     
        if (!stateId.HasValue && states.Any())
            stateId = states.First().Id;

        ViewBag.SelectedStateId = stateId;

        IEnumerable<LocationDistrictDTO> data = new List<LocationDistrictDTO>();
        if (stateId.HasValue)
        {
            data = _service.GetByStateId(stateId.Value) ?? new List<LocationDistrictDTO>();
        }

        return View(data);
    }

    public JsonResult GetDistrictsByState(int stateId)
    {
        var districts = _service.GetByStateId(stateId) ?? new List<LocationDistrictDTO>();

        var result = districts.Select(d => new
        {
            d.DistrictId,
            d.DistrictName,
            d.DistrictCode,
            d.StateId,
            d.IsActive,
            d.IsCentinalDistrict
        });

        return Json(result);
    }


    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.StateList = _dropdownService.GetStates();
        return View(new LocationDistrictDTO());
    }


    // POST: Create
    [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LocationDistrictDTO dto)
        {
            if (!ModelState.IsValid)
            {
               
                return View(dto);
            }

            try
            {
                _service.Insert(dto); 
            }
            catch (Exception ex)
            {
             
                ModelState.AddModelError("", "Error inserting district: " + ex.Message);
                return View(dto);
            }

       
            return RedirectToAction(nameof(Index));
        }
   
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var data = _service.GetById(id);
        if (data == null)
            return NotFound();

        ViewBag.StateList = _dropdownService.GetStates(); 

        return View(data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(LocationDistrictDTO model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.StateList = _dropdownService.GetStates(); 
            return View(model);
        }

        _service.Update(model);
        return RedirectToAction("Index");
    }



    [HttpGet]
    public IActionResult Delete(int id)
    {
        var data = _service.GetById(id);
        if (data == null)
            return NotFound();

        return View(data); 
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        _service.Delete(id);
        return Json(new { success = true });
    }

}
