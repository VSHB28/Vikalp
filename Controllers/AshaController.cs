using Microsoft.AspNetCore.Mvc;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;

namespace Vikalp.Controllers
{
    public class AshaController : Controller
    {
        private readonly IAshaService _ashaService;

        public AshaController(IAshaService ashaService)
        {
            _ashaService = ashaService;
        }

        public IActionResult Index()
        {
            var data = _ashaService.GetAllAsha().Result;
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AshaDto model)  
        {
            if (ModelState.IsValid)
            {
           
                await _ashaService.Insert(model, 0);
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _ashaService.GetById(id); 
            if (data == null)
            {
                return NotFound();
            }
            return View(data); 
        }
 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AshaDto model)
        {
            if (ModelState.IsValid)
            {
             
                int userId = 1; 

                await _ashaService.Update(model, userId); 
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _ashaService.GetById(id); 
            if (data == null)
                return NotFound();

            return View(data); 
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _ashaService.Delete(id);
            return Json(new { success = true });
        }
      
    }
}