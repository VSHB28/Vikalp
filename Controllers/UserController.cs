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
            var selectedLanguages = Request.Form["LanguageId"].ToList();

            if (selectedLanguages.Any())
            {
                model.LanguageId = string.Join(",", selectedLanguages);
            }

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
            var selectedLanguages = Request.Form["LanguageId"].ToList();

            if (selectedLanguages.Any())
            {
                model.LanguageId = string.Join(",", selectedLanguages);
            }

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
        private void LoadMasters()
        {
            ViewBag.Roles = _dropdownService.GetRoles();
            ViewBag.Languages = _dropdownService.GetLanguages();
            ViewBag.Genders = _dropdownService.GetGenders();
        }

    }
}
