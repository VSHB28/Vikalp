using Microsoft.AspNetCore.Mvc;
using Vikalp.Models;
using Vikalp.Models.DTO;
using Vikalp.Service;
using Vikalp.Service.Interfaces;

namespace Vikalp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _service;
        private readonly IDropdownService _dropdownService;

        public UserController(IUserService service, IDropdownService dropdownService)
        {
            _service = service;
            _dropdownService = dropdownService;
        }

        public IActionResult Index()
        {
            LoadMasters();
            var users = _service.GetAll();
            return View(users);
        }

        public IActionResult Create()
        {
            LoadMasters();
            return View("AddUser", new UserDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserDto model)
        {
            model.LanguageId = Request.Form["LanguageId"].ToString();

            if (!ModelState.IsValid)
            {
                LoadMasters();
                return View("AddUser", model);
            }

            _service.Create(model);
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Edit(int id)
        {
            LoadMasters();
            var user = _service.GetById(id);
            if (user == null) return NotFound();
            return View("UpdateUser", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserDto model)
        {
            model.LanguageId = Request.Form["LanguageId"].ToString();

            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                foreach (var error in state.Errors)
                {
                    Console.WriteLine($"{key}: {error.ErrorMessage}");
                }
            }


            if (!ModelState.IsValid)
            {
                LoadMasters();
                return View("UpdateUser", model);
            }

            _service.Update(model);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var user = _service.GetById(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _service.Delete(id);
            return RedirectToAction(nameof(Index));
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
        private void LoadMasters()
        {
            ViewBag.States = _dropdownService.GetStates();
            ViewBag.Roles = _dropdownService.GetRoles();
            ViewBag.Languages = _dropdownService.GetLanguages();
            ViewBag.Genders = _dropdownService.GetGenders();
        }
    }
}
