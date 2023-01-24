using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace SsbHedger.SsbChartControl.MarkupExtensions
{
    public class TextXAxis : MarkupExtension
    {
        public string Text { get; set; }

        public TextXAxis() { Text = ""; }  

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            FormattedText text = new(
                Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                12,
                Brushes.Black,
                1);

            return text.BuildGeometry(new Point(0, 0));
        }
    }
}
