using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.VisualBasic;
using ServerHeThongKiem.Models;
using ServerHeThongKiem.Services;
using System.Net;
using System.Numerics;



namespace ServerHeThongKiem.Services
{
    [ApiController]
    [Route("api/[controller]")]
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
            // 1. Kiểm tra đầu vào [cite: 28, 53]
            if (info == null || string.IsNullOrEmpty(info.DeviceID))
                return BadRequest("Dữ liệu hoặc Mã thiết bị không hợp lệ.");

            // 2. Kiểm tra trùng ID [cite: 47, 53]
            var exists = await _context.Devices.AnyAsync(d => d.DeviceID == info.DeviceID);
            if (exists)
                return BadRequest($"Mã thiết bị {info.DeviceID} đã được đăng ký trước đó.");

            // 3. Khởi tạo thiết bị mới với đầy đủ thông tin quản lý [cite: 26, 30, 31, 33]
            var newDevice = new DeviceModel
            {
                DeviceID = info.DeviceID,
                DeviceName = info.DeviceName ?? "Hệ Thống Kiềm Trường Học",
                Customer = info.Customer,
                Address = info.Address,
                Phone = info.Phone,
                CreateDate = DateTime.Now,
                Status = "Online",
                IsAutoMode = true,
                Inputs = new List<DeviceInput>(),
                Outputs = new List<DeviceOutput>()
            };

            // 4. Tự động khởi tạo 23 danh mục Input mặc định 
            string[] inputNames = { "Phao Đầy 1", "Phao Cạn 1", "CB TDS 1", "CB Áp suất 1", "CB lưu lượng 1", "CB Lưu lượng 2", "Phao Đầy 2", "Phao Cạn 2", "CB lưu lượng nc thải", "CB lưu lượng trước màng", "CB lưu lượng nước RO", "Phao Đầy 3", "Phao Can 3", "CB lưu lượng ra kiểm", "CB Dòng bể 1", "CB Dòng bể 2", "CB Dòng bể 3", "CB Dòng bể 4", "CB dòng bể 5", "Van áp cao 1", "Van áp cao 2", "Van áp cao 3", "Dừng Khẩn" };

            for (int i = 0; i < inputNames.Length; i++)
            {
                newDevice.Inputs.Add(new DeviceInput { Order = i + 1, Name = inputNames[i], Status = "OFF", Value = "0" });
            }

            // 5. Tự động khởi tạo 16 danh mục Output mặc định 
            string[] outputNames = { "Bơm ngang", "Bơm RO", "Đèn cảnh báo", "Lượng nước tiêu thụ", "Điện năng tiêu thụ", "Van sả đáy UF", "Van sả bể điện phân", "Bể Điện Phân 1", "Bể Điện Phân 2", "Bể Điện Phân 3", "Bể Điện Phân 4", "Bể Điện Phân 5", "Bể Điện Phân 6", "Bể Điện Phân 7", "Bể Điện Phân 8", "Bể Điện Phân 9" };

            for (int i = 0; i < outputNames.Length; i++)
            {
                newDevice.Outputs.Add(new DeviceOutput { Order = i + 1, Name = outputNames[i], Status = "OFF", Value = "0" });
            }

            try
            {
                _context.Devices.Add(newDevice);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Đăng ký thành công", db_id = newDevice.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi Database: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

    }
}
