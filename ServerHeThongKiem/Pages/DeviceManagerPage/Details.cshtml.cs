using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServerHeThongKiem.Models;
using ServerHeThongKiem.Services;

namespace ServerHeThongKiem.Pages.DeviceManagerPage
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context) => _context = context;

        // Dùng [BindProperty] ?? d? dàng truy c?p d? li?u ? file .cshtml
        public DeviceModel Device { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            // Eager Loading: T?i thi?t b? cùng v?i danh sách Inputs và Outputs
            var devicemodel = await _context.Devices
                .Include(d => d.Inputs)
                .Include(d => d.Outputs)
                .AsSplitQuery()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (devicemodel == null)
            {
                return NotFound();
            }
            else
            {
                Device = devicemodel;
            }

            return Page();
        }
    }
}