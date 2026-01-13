using Microsoft.AspNetCore.Mvc;
using Vikalp.Models;
using Vikalp.Models.DTO;
using Vikalp.Service;

namespace Vikalp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            var users = _service.GetAll();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new UserDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserDto model)
        {
            if (!ModelState.IsValid) return View(model);

            // TODO: hash model.Password before storing (service or controller)
            _service.Create(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(Guid Userid)
        {
            var user = _service.GetById(Userid);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserDto model)
        {
            if (!ModelState.IsValid) return View(model);

            // If password is empty, your service should keep the existing password
            _service.Update(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(Guid Userid)
        {
            var user = _service.GetById(Userid);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(Guid Userid)
        {
            _service.Delete(Userid);
            return RedirectToAction(nameof(Index));
        }

        
    }
}
