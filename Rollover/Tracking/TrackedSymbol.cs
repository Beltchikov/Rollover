namespace Rollover.Tracking
{
    public class TrackedSymbol
    {
        public string Name { get; set; }
        
        // Three last figures are behind the comma figures
        public int NextStrike { get; set; }

        // Three last figures are behind the comma figures
        public int OverNextStrike { get; set; }

        public override string ToString()
        {
            return $"{Name} {NextStrike} {OverNextStrike}";
        }
    }
}
