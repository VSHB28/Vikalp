using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vikalp.Models;
using Vikalp.Service;

namespace Vikalp.ViewComponents;

public class MenuViewComponent : ViewComponent
{
    private readonly MenuService _menuService;
    private readonly ILogger<MenuViewComponent> _logger;

    public MenuViewComponent(MenuService menuService, ILogger<MenuViewComponent> logger)
    {
        _menuService = menuService;
        _logger = logger;
    }

    // Use InvokeAsync for potential I/O and to avoid blocking thread-pool threads.
    public async Task<IViewComponentResult> InvokeAsync()
    {
        // Log at entry so you can confirm the component is called
        _logger.LogInformation("MenuViewComponent.InvokeAsync called for user {User}", User?.Identity?.Name ?? "anonymous");

        // If GetMenusByRole does DB work synchronously, run it on the thread-pool.
        var menus = await Task.Run(() => _menuService.GetMenusByRole());

        _logger.LogInformation("MenuViewComponent: menu count = {Count}", menus?.Count ?? 0);

        // Always pass a non-null model to the view to simplify the Razor.
        return View(menus ?? new List<MenuViewModel>());
    }
}