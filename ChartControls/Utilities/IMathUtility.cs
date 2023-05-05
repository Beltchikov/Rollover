using System.Collections.Generic;

namespace ChartControls.Utilities
{
    public interface IMathUtility
    {
        double GetDiagramX(double diagramWidth, List<double> datarow, int idx, double startOffset, double endOffset);
        double GetDiagramY(double diagramHeight, List<double> datarow, int idx, double startOffset, double endOffset);
    }
}