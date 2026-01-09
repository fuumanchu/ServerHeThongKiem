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
        public string DeviceID { get; set; } = ""; 
        public string DeviceName { get; set; } = "Hệ Thống Kiềm Trường Học"; 
        public string Customer { get; set; } = ""; 
        public string Address { get; set; } = ""; 
        public string Phone { get; set; } = ""; 
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime? LastSeen { get; set; }
        // --- Trạng thái vận hành ---
        public string Status { get; set; } = "Offline"; 
        public bool IsAutoMode { get; set; } = true; 

        public List<DeviceInput> Inputs { get; set; } = new List<DeviceInput>(); 
        public List<DeviceOutput> Outputs { get; set; } = new List<DeviceOutput>();

        public DeviceModel()
        {
            Inputs = new List<DeviceInput>();
            Outputs = new List<DeviceOutput>();
        }

        

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