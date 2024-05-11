namespace SignalAdvisor.Commands
{
    class ConnectToTws
    {
        public static void Run(ITwsVisitor visitor)
        {
            if (!visitor.IbHost.Consumer.ConnectedToTws)
            {
                visitor.IbHost.ConnectAndStartReaderThread(
                    visitor.Host,
                    visitor.Port,
                    visitor.ClientId,
                    visitor.Timeout);
            }
            else
            {
                visitor.IbHost.Disconnect();
            }
        }
    }
}
