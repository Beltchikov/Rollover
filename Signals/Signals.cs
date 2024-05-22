namespace Ta
{
    public class Signals
    {
        public static int OppositeColor(bool forLongTrade, List<Bar> bars, List<int> signals)
        {
            // TODO
            signals = new List<int>();

            return -2;
        }

        public static int InsideUpDown(List<Bar> bars, List<int> signals)
        {
            // TODO
            signals = new List<int>();

            return -2;
        }

        //private double _InsideUpDownHigh, _InsideUpDownHighBefore;
        //private double _InsideUpDownLow, _InsideUpDownLowBefore;

        //protected override void OnBarUpdate()
        //{
        //    const int MIN_BARS = 10;
        //    if (CurrentBar <= MIN_BARS)
        //    {
        //        Main[0] = 0;
        //        return;
        //    }

        //    if (Bars.IsFirstBarOfSession && IsFirstTickOfBar)
        //    {
        //        _InsideUpDownHigh = 0;
        //        _InsideUpDownLow = 0;
        //    }

        //    if (IsInsideUpDown())
        //    {
        //        // Old inside bar cancelled
        //        if (Close[0] > _InsideUpDownHigh || Close[0] < _InsideUpDownLow)
        //        {
        //            // New inside bar?
        //            if (Close[0] <= High[1] && Close[0] >= Low[1] && Open[0] <= High[1] && Open[0] >= Low[1])
        //            {
        //                _InsideUpDownHigh = High[1];
        //                _InsideUpDownLow = Low[1];
        //            }
        //            else
        //            {
        //                _InsideUpDownHigh = 0;
        //                _InsideUpDownLow = 0;
        //            }
        //        }

        //        // Old inside bar expanded?
        //        if (High[1] >= _InsideUpDownHigh && Low[1] <= _InsideUpDownLow)
        //        {
        //            _InsideUpDownHigh = High[1];
        //            _InsideUpDownLow = Low[1];
        //        }
        //        if (High[1] >= _InsideUpDownHigh && Low[1] >= _InsideUpDownLow)
        //        {
        //            _InsideUpDownHigh = High[1];
        //        }
        //        if (High[1] <= _InsideUpDownHigh && Low[1] <= _InsideUpDownLow)
        //        {
        //            _InsideUpDownLow = Low[1];
        //        }
        //    }
        //    else if (Close[0] <= High[1] && Close[0] >= Low[1] && Open[0] <= High[1] && Open[0] >= Low[1]) // Not inside bar
        //    {
        //        _InsideUpDownHigh = High[1];
        //        _InsideUpDownLow = Low[1];
        //    }

        //    var main0 = IsInsideUpDown() ? 1 : 0;
        //    var main1 = IsInsideUpDownBefore() ? 1 : 0;

        //    if (main0 == 0 && main1 != 0)
        //    {
        //        if (Close[0] > Close[1])
        //        {
        //            Main[0] = 1;
        //        }
        //        else if (Close[0] < Close[1])
        //        {
        //            Main[0] = -1;
        //        }
        //        else
        //        {
        //            Main[0] = 0;
        //        }
        //    }
        //    else
        //    {
        //        Main[0] = 0;
        //    }

        //    //Main[0] = main0;

        //    _InsideUpDownHighBefore = _InsideUpDownHigh;
        //    _InsideUpDownLowBefore = _InsideUpDownLow;
    //}
    }
}
