using System.Collections.Generic;

namespace ChartControls.Utilities
{
    public interface IMathUtility
    {
        double GetDiagramX(
            double diagramWidth,
            List<List<double>> seriesCollection,
            List<double> series,
            int idx,
            double startOffset,
            double endOffset);
        double GetDiagramY(
            double diagramHeight,
            List<List<double>> seriesCollection,
            List<double> series,
            int idx,
            double startOffset,
            double endOffset);
    }
}