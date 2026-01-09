using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerHeThongKiem.Services;

namespace ServerHeThongKiem.Controllers
{
    [ApiController]
    [Route("api/devices")]
    public class DeviceQueryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DeviceQueryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/devices/search?q=abc&status=Online
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] string? status)
        {
            var query = _context.Devices.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(d =>
                    d.DeviceID.Contains(q) ||
                    d.DeviceName.Contains(q) ||
                    d.Customer.Contains(q) ||
                    d.Phone.Contains(q) ||
                    d.Address.Contains(q)
                );
            }

            // ⏱ Ngưỡng Online: 30 giây
            var threshold = DateTime.Now.AddSeconds(-30);

            var result = await query
    .OrderByDescending(d => d.Id)
    .Select(d => new DeviceSearchDto
    {
        Id = d.Id,
        DeviceID = d.DeviceID,
        DeviceName = d.DeviceName,
        Customer = d.Customer,
        Address = d.Address,
        Phone = d.Phone,
        CreateDate = d.CreateDate,

        // ✅ lấy trực tiếp từ DB
        Status = d.Status
    })
    .ToListAsync();


            // Lọc theo status SAU khi tính
            if (!string.IsNullOrWhiteSpace(status))
            {
                result = result.Where(d => d.Status == status).ToList();
            }

            return Ok(result);
        }

        // DELETE theo ID
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var device = await _context.Devices
                .Include(d => d.Inputs)
                .Include(d => d.Outputs)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
                return NotFound(new { message = "Không tìm thấy thiết bị", id });

            if (device.Inputs?.Count > 0)
                _context.RemoveRange(device.Inputs);

            if (device.Outputs?.Count > 0)
                _context.RemoveRange(device.Outputs);

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa thiết bị", id = device.Id, deviceId = device.DeviceID });
        }

        public class DeviceSearchDto
        {
            public int Id { get; set; }
            public string DeviceID { get; set; } = "";
            public string DeviceName { get; set; } = "";
            public string Customer { get; set; } = "";
            public string Address { get; set; } = "";
            public string Phone { get; set; } = "";
            public string Status { get; set; } = "";
            public DateTime CreateDate { get; set; }
        }
    }
}
