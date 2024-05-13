namespace Ta
{
    public class Bar
    {
        public Bar(double open, double high, double low, double close, DateTime time)
        {
            Open = open;
            High = high; 
            Low = low; 
            Close = close; 
            Time = time;
        }

        public double Open {  get; set; }
        public double High {  get; set; }
        public double Low {  get; set; }
        public double Close {  get; set; }
        public DateTime Time {  get; set; }
    }
}
