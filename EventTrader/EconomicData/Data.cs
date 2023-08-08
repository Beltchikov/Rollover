namespace EventTrader.EconomicData
{
    public class Data
    {
        private Data()
        {
                
        }

        public Data(DataTypeEnum type)
        {
            Type = type;
        }

        public DataTypeEnum Type { get; private set; }  
        public double Actual { get; set; }  
        public double Expected { get; set; }  
        public double Previous { get; set; }  
    }
}