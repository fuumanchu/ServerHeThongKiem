using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServerHeThongKiem.Models;
using ServerHeThongKiem.Services;
using ServerHeThongKiem.Services.Interfaces;

namespace ServerHeThongKiem.Pages.DeviceManagerPage
{

    public class ToggleCommandDto
    {
        public int Order { get; set; }
        public string Type { get; set; } = "";
        public string State { get; set; } = "";
    }

    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IMqttPublishService _mqtt; 

        public DetailsModel(ApplicationDbContext context, IMqttPublishService mqtt)
        {
            _context = context;
            _mqtt = mqtt;
        }

        // Dùng [BindProperty] ?? d? dàng truy c?p d? li?u ? file .cshtml
        [BindProperty]
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

        public IActionResult OnPostBackToList()
        {
            return RedirectToPage("/DeviceManagerPage/Index");
        }

        public async Task<IActionResult> OnPostSendCommandAsync(string commandType, int outputOrder)
        {
            if (Device == null || string.IsNullOrEmpty(Device.DeviceID))
            {
                return BadRequest("Thiết bị không hợp lệ.");
            }
            string topic = $"Devices/{Device.DeviceID}/Commands";
            string payload = $"{commandType}:{outputOrder}";
            await _mqtt.PublishAsync(topic, payload);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostToggleAsync(
            [FromBody] ToggleCommandDto cmd)
        {   
            if (cmd == null)
                return BadRequest("Payload null");

            // Lấy DeviceID từ route/query (vì Razor Page đang ở trang chi tiết)
            if (!int.TryParse(Request.Query["id"], out int deviceDbId))
                return BadRequest("Missing device id");

            var device = await _context.Devices
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == deviceDbId);

            if (device == null)
                return NotFound("Device not found");

            string topic = $"Devices/{device.DeviceID}/Commands";

            /*
             Payload GỬI XUỐNG FIRMWARE – NÊN RÕ RÀNG
             Ví dụ:
             {
                "type":"Output",
                "order":3,
                "state":"ON"
             }
            */
            var payload = System.Text.Json.JsonSerializer.Serialize(new
            {
                cmd.Type,
                cmd.Order,
                cmd.State
            });

            await _mqtt.PublishAsync(topic, payload);

            return new JsonResult(new { success = true });
        }



    }
}