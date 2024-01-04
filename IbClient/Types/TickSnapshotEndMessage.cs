namespace IbClient.Types
{
    public class TickSnapshotEndMessage
    {
        public TickSnapshotEndMessage(int reqId)
        {
            ReqId = reqId;
        }

        public int ReqId { get; set; }
    }
}
