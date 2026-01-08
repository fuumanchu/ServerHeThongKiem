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
            // 1. Vệ sinh dữ liệu đầu vào
            info.DeviceID = info.DeviceID?.Trim();

            if (info == null || string.IsNullOrEmpty(info.DeviceID))
                return BadRequest("Dữ liệu hoặc Mã thiết bị không hợp lệ.");

            var exists = await _context.Devices.AnyAsync(d => d.DeviceID == info.DeviceID);
            if (exists)
                return BadRequest($"Mã thiết bị {info.DeviceID} đã được đăng ký trước đó.");

            var newDevice = new DeviceModel
            {
                DeviceID = info.DeviceID,
                DeviceName = info.DeviceName ?? "Hệ Thống Kiềm Trường Học",
                Customer = info.Customer,
                Address = info.Address,
                Phone = info.Phone,
                CreateDate = info.CreateDate,
                Status = "Online",
                IsAutoMode = true,
                Inputs = new List<DeviceInput>(),
                Outputs = new List<DeviceOutput>()
            };

            // --- 2. LOGIC INPUT (Mô phỏng giống hệt SQL) ---
            string[] inputNames = {
                "Phao Đầy 1", "Phao Cạn 1", "CB TDS 1", "CB Áp suất 1", "CB lưu lượng 1",
                "CB Lưu lượng 2", "Phao Đầy 2", "Phao Cạn 2", "CB lưu lượng nc thải",
                "CB lưu lượng trước màng", "CB lưu lượng nước RO", "Phao Đầy 3", "Phao Can 3",
                "CB lưu lượng ra kiểm", "CB Dòng bể 1", "CB Dòng bể 2", "CB Dòng bể 3",
                "CB Dòng bể 4", "CB dòng bể 5", "Van áp cao 1", "Van áp cao 2", "Van áp cao 3", "Dừng Khẩn"
            };

            for (int i = 0; i < inputNames.Length; i++)
            {
                string name = inputNames[i];

                // Mặc định cho loại Switch (ON/OFF)
                string range = "ON/OFF";
                string val = "0";
                string status = "OFF";

                // Cấu hình riêng cho từng loại cảm biến (Analog)
                if (name.Contains("CB TDS"))
                {
                    range = "50 ÷ 150";
                    val = "82"; // Giá trị an toàn (nằm giữa ngưỡng)
                    status = "ON";
                }
                else if (name.Contains("Áp suất") || name.Contains("lưu lượng"))
                {
                    range = "0.8 ÷ 2.5";
                    val = "1.5"; // Giá trị an toàn
                    status = "ON";
                }
                else if (name.Contains("Dòng bể"))
                {
                    range = "1 ÷ 5";
                    val = "3.0"; // Giá trị an toàn
                    status = "ON";
                }
                else if (name.Contains("Phao Cạn 1")) // Logic riêng trong SQL của bạn là ON
                {
                    status = "ON";
                    val = "1";
                }

                newDevice.Inputs.Add(new DeviceInput
                {
                    Order = i + 1,
                    Name = name,
                    Status = status,
                    Value = val,
                    SettingRange = range // <--- BẮT BUỘC CÓ
                });
            }

            // --- 3. LOGIC OUTPUT ---
            string[] outputNames = {
                "Bơm ngang", "Bơm RO", "Đèn cảnh báo", "Lượng nước tiêu thụ", "Điện năng tiêu thụ",
                "Van sả đáy UF", "Van sả bể điện phân", "Bể Điện Phân 1", "Bể Điện Phân 2",
                "Bể Điện Phân 3", "Bể Điện Phân 4", "Bể Điện Phân 5", "Bể Điện Phân 6",
                "Bể Điện Phân 7", "Bể Điện Phân 8", "Bể Điện Phân 9"
            };

            for (int i = 0; i < outputNames.Length; i++)
            {
                string name = outputNames[i];

                // Mặc định
                string range = "ON/OFF";
                string val = "0";
                string status = "OFF";

                if (name.Contains("tiêu thụ"))
                {
                    range = "Chỉ Số";
                    val = "0.0";
                    status = "ON";
                }
                else if (name.Contains("Bể Điện Phân"))
                {
                    range = "0 ÷ 5"; // Quan trọng để hiện thanh Progress Bar màu vàng
                    val = "3.0";     // Để thanh bar hiện 60% lúc đầu cho đẹp
                    status = "ON";
                }
                else if (name.Contains("Bơm"))
                {
                    status = "ON";
                    val = "1";
                }

                newDevice.Outputs.Add(new DeviceOutput
                {
                    Order = i + 1,
                    Name = name,
                    Status = status,
                    Value = val,
                    SettingRange = range // <--- BẮT BUỘC CÓ
                });
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
