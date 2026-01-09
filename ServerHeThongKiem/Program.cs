
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Client;
using ServerHeThongKiem.Services;
using ServerHeThongKiem.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddHostedService<ServerHeThongKiem.Services.DeviceStatusWorker>();

builder.Services.AddDbContext<ServerHeThongKiem.Services.ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});


var mqttFactory = new MQTTnet.MqttFactory();
var mqttClient = mqttFactory.CreateMqttClient();


// 2. Đăng ký Client này vào DI container dưới dạng Singleton
builder.Services.AddSingleton<IMqttClient>(mqttClient);

// 3. Đăng ký các dịch vụ khác
builder.Services.AddSingleton<IDeviceCacheService, DeviceCacheService>();
builder.Services.AddSingleton<IMqttPublishService, MqttPublishService>();

// 4. Đăng ký Worker sử dụng chung mqttClient ở trên
builder.Services.AddHostedService<MqttWorkerService>();

// 5. hub 
builder.Services.AddSignalR();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Đường dẫn đến trang đăng nhập
        options.LogoutPath = "/Logout"; // Đường dẫn đến trang đăng xuất
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Thời gian hết hạn cookie
    });
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/"); // Yêu cầu đăng nhập toàn bộ
    options.Conventions.AllowAnonymousToPage("/Login"); // <--- THÊM DÒNG NÀY: Cho phép truy cập trang Login mà không cần mật khẩu
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); // Thêm xác thực
app.MapHub<DeviceHub>("/deviceHub");
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
