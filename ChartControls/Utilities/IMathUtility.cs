﻿using System.Collections.Generic;

namespace ChartControls.Utilities
{
    public interface IMathUtility
    {
        double GetDiagramCoordinate(double diagramWidth, List<double> datapoints, int idx, double startOffset, double endOffset);
    }
}