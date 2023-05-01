using System.Windows.Media;

namespace ChartControls
{
    public class DataPoint
    {
        public object? Value { get; set; }
        public double DotSize { get; set; }
        public Brush? Fill { get; set; }
    }
}
