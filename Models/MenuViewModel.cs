namespace Vikalp.Models
{
    public class MenuViewModel
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public int? ParentMenuId { get; set; }
        public string Url { get; set; }
        public string MenuIcon { get; set; }

        public List<MenuViewModel> Children { get; set; } = new();
    }

}
