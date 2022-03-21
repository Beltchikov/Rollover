namespace UsMoversOpening
{
    public interface IStocksBuyer
    {
        bool SendOrders();
        bool Triggered(string timeToBuyString);
    }
}