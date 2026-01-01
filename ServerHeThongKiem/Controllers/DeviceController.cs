using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ServerHeThongKiem.Models;
using ServerHeThongKiem.Services;



namespace ServerHeThongKiem.Controllers
{
    public class DeviceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DeviceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]

        public async Task<IActionResult> Register([FromBody] DeviceModel info)
        {
           // 1. Kiểm tra ID thiết bị (Ví dụ: TH0001) [cite: 28, 53]
            if (string.IsNullOrEmpty(info.DeviceID))
                return BadRequest("Mã thiết bị không được để trống");

           // 2. Lưu vào Database (Tên, Địa chỉ, SĐT...) [cite: 30, 31, 33]
            var newDevice = new DeviceModel
            {
                Id = info.DeviceID,
                Name = info.DeviceName,
                Customer = info.Customer,
                Address = info.Address,
                Phone = info.Phone,
                [cite_start]CreatedAt = DateTime.Now // [cite: 34]
            };

            _context.Devices.Add(newDevice);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đăng ký thiết bị thành công" });
        }


    }
}
