using System.Collections;

namespace Ta.Indicators
{
    public class DataPoints : IEnumerable<DataPoint>    
    {
        private List<DataPoint> _dataPoints;

        public DataPoints()
        {
                _dataPoints = new List<DataPoint>();    
        }

        public int Count => _dataPoints.Count;

        public void Add(DataPoint dataPoint) {
            _dataPoints.Add(dataPoint);
        }

        public DataPoint BarsAgo(int barsAgo)
        {
            return _dataPoints[_dataPoints.Count - 1 - barsAgo];
        }

        public void AddRange(DataPoints dataPoints)
        {
            _dataPoints.AddRange(dataPoints);
        }

        public IEnumerator<DataPoint> GetEnumerator()
        {
            return ((IEnumerable<DataPoint>)_dataPoints).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dataPoints).GetEnumerator();
        }
    }
}
