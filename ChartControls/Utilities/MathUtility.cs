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
            var range = (dataRowsCollection.SelectMany(x => x).Max() - dataRowsCollection.SelectMany(x => x).Min()) + startOffset + endOffset;
            double koef = diagramWidth / range;
            var scaledDistance = datarow[idx] - dataRowsCollection.SelectMany(x => x).Min();

            return idx == 0 ? startOffset* koef : startOffset* koef + koef * scaledDistance;
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
            //double koef = Math.Floor((diagramHeight / range)*100) / 100;
            double koef = diagramHeight / range;
            var scaledDistance = datarow[idx] - minValue;

            var value = datarow[idx];
            //var result = idx == 0 ? diagramHeight - startOffset * koef : diagramHeight - koef * (startOffset + scaledDistance);
            var result = diagramHeight - koef * (startOffset + scaledDistance);
            
            return result;
        }
    }
}
