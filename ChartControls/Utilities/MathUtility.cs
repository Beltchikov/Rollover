using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartControls.Utilities
{
    public class MathUtility : IMathUtility
    {
        public double GetDiagramX(
            double diagramWidth,
            List<List<double>> dataRowsCollection,
            List<double> datarow,
            int idx,
            double startOffset,
            double endOffset)
        {
            var minValue = dataRowsCollection.SelectMany(x => x).Min();
            var range = (dataRowsCollection.SelectMany(x => x).Max() - minValue) + startOffset + endOffset;
            double koef = diagramWidth / range;
            var scaledDistance = datarow[idx] - minValue;

            return startOffset* koef + koef * scaledDistance;
        }

        public double GetDiagramY(
            double diagramHeight,
            List<List<double>> dataRowsCollection,
            List<double> datarow,
            int idx,
            double startOffset,
            double endOffset)
        {
            var minValue = dataRowsCollection.SelectMany(x => x).Min();
            var range = (dataRowsCollection.SelectMany(x => x).Max() - minValue) + startOffset + endOffset;
            double koef = diagramHeight / range;
            var scaledDistance = datarow[idx] - minValue;

            return diagramHeight - koef * (startOffset + scaledDistance); 
        }
    }
}
