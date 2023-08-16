namespace SsbHedger.IbModel
{
    public interface IComboLeg
    {
        string Action { get; set; }
        int ConId { get; set; }
        string DesignatedLocation { get; set; }
        string Exchange { get; set; }
        int ExemptCode { get; set; }
        int OpenClose { get; set; }
        int Ratio { get; set; }
        int ShortSaleSlot { get; set; }
    }
}
