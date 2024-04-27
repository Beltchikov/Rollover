namespace NetBms
{
    public class ChatGptBatchResult
    {
        public ChatGptBatchResult()
        {
            BUY = [];
            SELL = [];
        }
        
        public List<string> BUY { get; set; }
        public List<string> SELL { get; set; }
    }
}
