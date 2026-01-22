using Microsoft.AspNetCore.Mvc;
using Vikalp.Models.DTO;
using Vikalp.Service;

namespace Vikalp.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService) =>
            _roleService = roleService;

        // GET: /Role or /Role/Index
        public IActionResult Index()
        {
            var roles = _roleService.GetAll();
            return View(roles);
        }

        // GET: /Role/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View("AddRole", new RoleDto());
        }

        // POST: /Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RoleDto model)
        {
            if (!ModelState.IsValid) return View("AddRole", model);

            _roleService.Create(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Role/Edit/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var role = _roleService.GetById(id);
            if (role == null) return NotFound();
            return View("UpdateRole", role);
        }

        // POST: /Role/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(RoleDto model)
        {
            if (!ModelState.IsValid) return View("UpdateRole", model);

            _roleService.Update(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Role/Details/{id}
        public IActionResult Details(int id)
        {
            var role = _roleService.GetById(id);
            if (role == null) return NotFound();
            return View(role);
        }

        // GET: /Role/Delete/{id} (confirmation)
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var role = _roleService.GetById(id);
            if (role == null) return NotFound();
            return View(role);
        }

        // POST: /Role/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _roleService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}