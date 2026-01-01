using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ServerHeThongKiem.Models
{
    public class DeviceModel
    {
        [Key]
        public int Id { get; set; }

        // --- Thông tin quản lý (Trang 3) ---
        [Required]
        public string DeviceID { get; set; } = ""; // Ví dụ: TH0001 
        public string DeviceName { get; set; } = "Hệ Thống Kiềm Trường Học"; 
        public string Customer { get; set; } = ""; // Trường THCS Hùng Vương 01 
        public string Address { get; set; } = ""; 
        public string Phone { get; set; } = ""; 
        public DateTime CreateDate { get; set; } = DateTime.Now; 

        // --- Trạng thái vận hành ---
        public string Status { get; set; } = "Offline"; // Online, Offline, Alarm [cite: 118]
        public bool IsAutoMode { get; set; } = true; // Chế độ Tự động/Thủ công [cite: 75, 76]

        // --- Danh sách I/O (Trang 4) ---
        // Sử dụng quan hệ 1-nhiều trong Entity Framework
        public List<DeviceInput> Inputs { get; set; } = new List<DeviceInput>(); 
        public List<DeviceOutput> Outputs { get; set; } = new List<DeviceOutput>(); 
    }

    public class DeviceInput
    {
        public int Id { get; set; }
        public int Order { get; set; } // Stt từ 1 đến 23 
        public string Name { get; set; } = ""; // Ví dụ: CB TDS 1 
        public string Status { get; set; } = "OFF"; // ON/OFF 
        public string Value { get; set; } = ""; // Giá trị hiện tại từ MQTT
        public string SettingRange { get; set; } = ""; // Ví dụ: 50-150 
    }

    public class DeviceOutput
    {
        public int Id { get; set; }
        public int Order { get; set; } // Stt từ 1 đến 16 
        public string Name { get; set; } = ""; // Ví dụ: Bơm RO 
        public string Status { get; set; } = "OFF"; // ON/OFF 
        public string Value { get; set; } = ""; // Giá trị hiện tại từ MQTT
        public string SettingRange { get; set; } = ""; // Ví dụ: 50-150 
    }
}