namespace EventTrader.EconomicData
{
    public class Data
    {
        public Data(DataTypeEnum type, string name)
        {
            Type = type;
            Name = name;
        }

        public DataTypeEnum Type { get; private set; }  
        public string Name { get; private set; }  
        public double Actual { get; set; }  
        public double Expected { get; set; }  
        public double Previous { get; set; }  
    }
}