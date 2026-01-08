using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServerHeThongKiem.Services; // Namespace chứa DbContext

namespace ServerHeThongKiem.Pages
{
    [Authorize] // Bắt buộc đăng nhập mới xem được thống kê
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Các biến lưu trữ số liệu thống kê
        public int TotalDevices { get; set; }
        public int OnlineDevices { get; set; }
        public int OfflineDevices { get; set; }

        public void OnGet()
        {
            // Lấy dữ liệu từ DB (giả sử tên bảng trong DbContext là Devices)
            // Nếu bảng của bạn tên khác, hãy sửa lại (ví dụ: _context.DeviceModels)

            // 1. Tổng thiết bị
            TotalDevices = _context.Devices.Count();

            // 2. Thiết bị Online (So sánh chuỗi hoặc bool tùy data của bạn)
            OnlineDevices = _context.Devices.Count(d => d.Status == "Online");

            // 3. Thiết bị Offline
            OfflineDevices = TotalDevices - OnlineDevices;
        }
    }
}