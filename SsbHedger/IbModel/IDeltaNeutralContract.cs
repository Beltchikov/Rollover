namespace SsbHedger.IbModel
{
    public interface IDeltaNeutralContract
    {
        int ConId { get; set; }
        double Delta { get; set; }
        double Price { get; set; }
    }
}
