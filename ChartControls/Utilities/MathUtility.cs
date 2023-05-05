using System.Collections.Generic;
using System.Linq;

namespace ChartControls.Utilities
{
    public class MathUtility : IMathUtility
    {
        public double GetDiagramX(double diagramWidth, List<double> datarow, int idx, double startOffset, double endOffset)
        {
            var range = (datarow.Max() - datarow.Min()) + startOffset + endOffset;
            double koef = diagramWidth / range;
            var scaledDistance = datarow[idx] - datarow[0];

            return idx == 0 ? startOffset* koef : startOffset* koef + koef * scaledDistance;
        }

        public double GetDiagramY(double diagramHeight, List<double> datarow, int idx, double startOffset, double endOffset)
        {
            var range = (datarow.Max() - datarow.Min()) + startOffset + endOffset;
            double koef = diagramHeight / range;
            var scaledDistance = datarow[idx] - datarow[0];

            return idx == 0 ? diagramHeight - startOffset * koef : diagramHeight - koef * (startOffset + scaledDistance);
        }
    }
}
