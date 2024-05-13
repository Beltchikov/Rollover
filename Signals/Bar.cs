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

        double Open {  get; set; }
        double High {  get; set; }
        double Low {  get; set; }
        double Close {  get; set; }
        DateTime Time {  get; set; }
    }
}
