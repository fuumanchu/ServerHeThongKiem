using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServerHeThongKiem.Services;
using ServerHeThongKiem.Models;

namespace ServerHeThongKiem.Pages.DeviceManagerPage
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext context;
        public List<DeviceModel> DeviceList { get; set; } = new ();
        public IndexModel(ApplicationDbContext context)
        {
            this.context = context;    
        }
        public void OnGet()
        {
            DeviceList = context.Devices.OrderByDescending(i => i.Id).ToList();
        }
    }
}
