using System.Collections.Generic;

namespace ChartControls.Utilities
{
    public interface IMathUtility
    {
        double GetDiagramX(double diagramWidthOrLength, List<double> datapoints, int idx, double startOffset, double endOffset);
    }
}