using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Vikalp.Models.DTO;
using Vikalp.Service.Implementations;
using Vikalp.Service.Interfaces;

namespace Vikalp.Controllers
{
    public class HRController : Controller
    {
        private readonly IHRService _hRService;
        private readonly IDropdownService _dropdownService;

        public HRController(
     IHRService hRService,
     IDropdownService dropdownService
)
        {
            _hRService = hRService;
            _dropdownService = dropdownService;
           
        }
      
        public async Task<IActionResult> Index()
        {
            ViewBag.States = _dropdownService.GetStates();

            var dropdowns = await _dropdownService
                .GetCommonDropdownsfamilyplanningAsync(userId: 1, languageId: 1);

            ViewBag.designation = dropdowns.ContainsKey("Designation")
                ? dropdowns["Designation"]
                : new List<SelectListItem>();

            ViewBag.gender = dropdowns.ContainsKey("Gender")
                ? dropdowns["Gender"]
                : new List<SelectListItem>();

            ViewBag.yesNoNa = dropdowns.ContainsKey("YesNoNa")
                ? dropdowns["YesNoNa"]
                : new List<SelectListItem>();

            var hrList = await _hRService.GetHrListAsync();

            return View(hrList);
        }


        [HttpPost]
        public async Task<IActionResult> SaveHrStatus([FromBody] HrStatusDto model)
        {
            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            int userId = userClaim != null ? int.Parse(userClaim.Value) : 0;
            model.CreatedBy = userId;
            await _hRService.SaveHrStatusAsync(model);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetHrStatus(int hrId)
        {
            var result = await _hRService.GetHrStatusAsync(hrId);
            return Json(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetHrList()
        {
            var data = await _hRService.GetHrListAsync();
            return Json(data);
        }
    }

}