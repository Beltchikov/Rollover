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
            //var scaledDistance = datarow[idx] - datarow[0];
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
            var range = (dataRowsCollection.SelectMany(x => x).Max() - dataRowsCollection.SelectMany(x => x).Min()) + startOffset + endOffset;
            double koef = diagramHeight / range;
            var scaledDistance = datarow[idx] - dataRowsCollection.SelectMany(x => x).Min();

            var result = idx == 0 ? diagramHeight - startOffset * koef : diagramHeight - koef * (startOffset + scaledDistance);
            return result;
        }
    }
}
