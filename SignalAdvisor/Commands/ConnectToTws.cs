
namespace SignalAdvisor.Commands
{
    class ConnectToTws
    {
        public static async Task<bool> RunAsync(ITwsVisitor visitor)
        {
            if (!visitor.IbHost.Consumer.ConnectedToTws)
            {
                bool connected = false;
                await visitor.IbHost.ConnectAndStartReaderThread(
                    visitor.Host,
                    visitor.Port,
                    visitor.ClientId,
                    (c) => { connected = c.IsConnected; },
                    (ma) => { },
                    (e) => { });

                //await Task.Run(()=> { while (!connected) { } });
                while (!connected) { };
                
                return connected;
            }
            else
            {
                visitor.IbHost.Disconnect();
            }

            return false;
        }

        public static async void Run(ITwsVisitor visitor)
        {
            if (!visitor.IbHost.Consumer.ConnectedToTws)
            {
                //visitor.IbHost.ConnectAndStartReaderThread(
                //    visitor.Host,
                //    visitor.Port,
                //    visitor.ClientId,
                //    visitor.Timeout);


                bool connected = false;
                await visitor.IbHost.ConnectAndStartReaderThread(
                    visitor.Host,
                    visitor.Port,
                    visitor.ClientId,
                    (c) => { connected = c.IsConnected; },
                    (ma) => { },
                    (e) => { });
            }
            else
            {
                visitor.IbHost.Disconnect();
            }
        }
    }
}
