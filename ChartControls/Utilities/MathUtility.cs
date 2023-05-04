using System.Collections.Generic;
using System.Linq;

namespace ChartControls.Utilities
{
    public class MathUtility : IMathUtility
    {
        public double GetDiagramX(double diagramWidth, List<double> datapoints, int idx, double startOffset, double endOffset)
        {
            var range = (datapoints.Max() - datapoints.Min()) + startOffset + endOffset;
            double koef = diagramWidth / range;
            var scaledDistance = datapoints[idx] - datapoints[0];

            return idx == 0 ? startOffset* koef : startOffset* koef + koef * scaledDistance;
        }

        public double GetDiagramY(double diagramHeight, List<double> datapoints, int idx, double startOffset, double endOffset)
        {
            var range = (datapoints.Max() - datapoints.Min()) + startOffset + endOffset;
            double koef = diagramHeight / range;
            var scaledDistance = datapoints[idx] - datapoints[0];

            return idx == 0 ? diagramHeight - startOffset * koef : diagramHeight - koef * (startOffset + scaledDistance);
        }
    }
}
