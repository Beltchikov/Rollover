using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SsbHedger.SsbChartControl.MiscConverters;

namespace SsbHedger.SsbChartControl
{
    public record BarUnderlying(DateTime Time, double Open, double High, double Low, double Close);

    /// <summary>
    /// Interaction logic for SsbChart.xaml
    /// </summary>
    public partial class SsbChart : UserControl
    {
        private readonly int HOURS_INTERVAL = 2;
        private readonly int BAR_WIDTH = 5;
        private readonly int X_AXIS_HEIGHT = 15;
        private readonly int Y_AXIS_WIDTH = 35;
        private ILineValuesConverter _lineValuesConverter;
        private Rect _gridRect = Rect.Empty;

        // Test code
        //static List<BarUnderlying> bars = new List<BarUnderlying>()
        //    {
        //        new BarUnderlying(DateTime.ParseExact("20230111 10:00:00", "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture),
        //            390.44, 390.93, 390.2, 390.84),
        //        new BarUnderlying(DateTime.ParseExact("20230111 10:05:00", "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture),
        //            390.84, 391.18, 390.78, 391.01),
        //        new BarUnderlying(DateTime.ParseExact("20230111 10:10:00", "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture),
        //            391.01, 391.07, 390.93, 391.02),
        //        new BarUnderlying(DateTime.ParseExact("20230111 10:15:00", "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture),
        //            391.02, 391.5, 390.98, 391.46)
        //    };

        static List<BarUnderlying> bars = new List<BarUnderlying>()
            {
                new BarUnderlying(DateTime.ParseExact("20230111 15:30:00", "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture),
                    390.44, 390.93, 390.2, 390.84),              
                new BarUnderlying(DateTime.ParseExact("20230111 15:35:00", "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture),
                    390.84, 391.18, 390.78, 391.01),             
                new BarUnderlying(DateTime.ParseExact("20230111 15:40:00", "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture),
                    391.01, 391.07, 390.93, 391.02),             
                new BarUnderlying(DateTime.ParseExact("20230111 15:45:00", "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture),
                    391.02, 391.5, 390.98, 391.46)
            };
       
        public static readonly DependencyProperty SessionStartProperty =
            DependencyProperty.Register("SessionStart", typeof(DateTime), typeof(SsbChart), new PropertyMetadata(default(DateTime)));
        public static readonly DependencyProperty SessionEndProperty =
            DependencyProperty.Register("SessionEnd", typeof(DateTime), typeof(SsbChart), new PropertyMetadata(default(DateTime)));
        public static readonly DependencyProperty BarsUnderlyingProperty =
            DependencyProperty.Register("BarsUnderlying", typeof(List<BarUnderlying>), typeof(SsbChart), new PropertyMetadata(bars));
        
        public SsbChart()
        {
            InitializeComponent();
            _lineValuesConverter = new LineValuesConverter();   
            DataContext = this;
        }
        public DateTime SessionStart
        {
            get { return (DateTime)GetValue(SessionStartProperty); }
            set { SetValue(SessionStartProperty, value); }
        }
        public DateTime SessionEnd
        {
            get { return (DateTime)GetValue(SessionEndProperty); }
            set { SetValue(SessionEndProperty, value); }
        }

        public Dictionary<DateTime, bool> LineTimesEvery2Hours
        {
            get
            {
                return _lineValuesConverter.LineTimes(SessionStart, SessionEnd, HOURS_INTERVAL);
            }
        }

        public Dictionary<DateTime, bool> LineTimesEveryHour
        {
            get
            {
                return _lineValuesConverter.LineTimes(SessionStart, SessionEnd, 1);
            }
        }

        public int BarWidth => BAR_WIDTH;
        public int XAxisHeight => X_AXIS_HEIGHT;
        public int YAxisWidth => Y_AXIS_WIDTH;

        public List<BarUnderlying> BarsUnderlying
        {
            get { return (List<BarUnderlying>)GetValue(BarsUnderlyingProperty); }
            set { SetValue(BarsUnderlyingProperty, value); }
        }
    }
}
