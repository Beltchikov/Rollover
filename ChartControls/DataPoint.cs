using System.Windows.Media;

namespace ChartControls
{
    public class DataPoint
    {
        public object? XValue { get; set; }
        public object? YValue { get; set; }
        public double DotSize { get; set; }
        public Brush? Fill { get; set; }
        public string ToolTip => $"Point: {XValue}  Value: {YValue}";
    }
}
