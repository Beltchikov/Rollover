namespace PortfolioTrader.Commands
{
    interface IBuyConfirmationModelVisitor : ITwsVisitor
    {
        string StocksToBuyAsString { get; set; }
        string StocksToSellAsString { get; set; }
        public int InvestmentAmount { get; set; }
        bool PositionsCalculated { get; set; }
        string StocksWithoutPrice { get; set; }
        string StocksWithoutMargin { get; set; }
        string OrdersLongWithError { get; set; }
        string OrdersShortWithError { get; set; }
        
        DateTime TimeEntryBar { get; set; }

        public void CalculateWeights();
        void ClearQueueOrderOpenMessage();
    }
}
