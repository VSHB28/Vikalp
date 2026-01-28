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
        public IActionResult CreateJson([FromBody] UserDto model)
        {
            if (model == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid data"
                });
            }

            try
            {
                _service.Create(model);

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



        public IActionResult Edit(int id)
        {
            LoadMasters();
            var user = _service.GetById(id);
            if (user == null) return NotFound();
            return View("UpdateUser", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromBody] UserDto model)
        {
            if (model == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid data"
                });
            }

            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Validation failed"
                });
            }

            try
            {
                _service.Update(model);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([FromBody] int userId)
        {
            if (userId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid user id"
                });
            }

            try
            {
                _service.Delete(userId);

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
