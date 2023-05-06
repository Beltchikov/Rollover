using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartControls.Utilities
{
    public class MathUtility : IMathUtility
    {
        public double GetDiagramX(
            double diagramWidth,
            List<List<double>> seriesCollection,
            List<double> series,
            int idx,
            double startOffset,
            double endOffset)
        {
            var minValue = seriesCollection.SelectMany(x => x).Min();
            var range = (seriesCollection.SelectMany(x => x).Max() - minValue) + startOffset + endOffset;
            double koef = diagramWidth / range;
            var scaledDistance = series[idx] - minValue;

            return startOffset* koef + koef * scaledDistance;
        }

        public double GetDiagramY(
            double diagramHeight,
            List<List<double>> seriesCollection,
            List<double> series,
            int idx,
            double startOffset,
            double endOffset)
        {
            var minValue = seriesCollection.SelectMany(x => x).Min();
            var range = (seriesCollection.SelectMany(x => x).Max() - minValue) + startOffset + endOffset;
            double koef = diagramHeight / range;
            var scaledDistance = series[idx] - minValue;

            return diagramHeight - koef * (startOffset + scaledDistance); 
        }
    }
}
