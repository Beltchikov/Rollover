namespace IbClient.Types
{
    public enum TickType
    {
        BidSize =0,
        BidPrice = 1,
        AskPrice = 2,
        AskSize = 3,
        LastPrice = 4,
        LastSize = 5,
        ClosePrice = 9,

        DelayedBid= 66,
        DelayedAsk= 67,
        DelayedLast= 68,
        DelayedBidSize = 69,
        DelayedAskSize = 70,
        DelayedLastSize = 71,
        DelayedClose = 75
    }
}
