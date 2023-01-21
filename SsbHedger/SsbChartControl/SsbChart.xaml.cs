using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace SsbHedger.SsbChartControl
{
    public record BarUnderlying(DateTime Time, double Open, double High, double Low, double Close);

    /// <summary>
    /// Interaction logic for SsbChart.xaml
    /// </summary>
    public partial class SsbChart : UserControl
    {
        private readonly int HOURS_INTERVAL = 2;
        private ILineValuesConverter _lineValuesConverter;

        // Test code
        static List<BarUnderlying> bars = new List<BarUnderlying>()
            {
                new BarUnderlying(DateTime.ParseExact("20230111 10:00:00", "yyyyMMdd hh:mm:ss", CultureInfo.InvariantCulture),
                    390.44, 390.93, 390.2, 390.84),
                new BarUnderlying(DateTime.ParseExact("20230111 10:05:00", "yyyyMMdd hh:mm:ss", CultureInfo.InvariantCulture),
                    390.84, 391.18, 390.78, 391.01),
                new BarUnderlying(DateTime.ParseExact("20230111 10:10:00", "yyyyMMdd hh:mm:ss", CultureInfo.InvariantCulture),
                    391.01, 391.07, 390.93, 391.02),
                new BarUnderlying(DateTime.ParseExact("20230111 10:15:00", "yyyyMMdd hh:mm:ss", CultureInfo.InvariantCulture),
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

        public List<DateTime> LineTimes
        {
            get
            {
                return _lineValuesConverter.LineTimes(SessionStart, SessionEnd, HOURS_INTERVAL);
            }
        }

        public Rect GridsRect
        {
            get
            {
                return new Rect(0,0,20,20);
            }
        }

        public List<BarUnderlying> BarsUnderlying
        {
            get { return (List<BarUnderlying>)GetValue(BarsUnderlyingProperty); }
            set { SetValue(BarsUnderlyingProperty, value); }
        }
    }
}
