namespace ServerHeThongKiem.Models
{
    public enum SettingType
    {
        Boolean,
        Numeric,
        ReadOnly,
    }
    public class IOSettingModel
    {
        public SettingType SettingType { get; set; }
        public double Value { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
    }
}
