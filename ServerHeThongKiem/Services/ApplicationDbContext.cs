using Microsoft.EntityFrameworkCore;
using ServerHeThongKiem.Models;
namespace ServerHeThongKiem.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<DeviceModel> Devices { get; set; } = null!;



    }
}

    

