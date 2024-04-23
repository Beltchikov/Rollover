using IbClient.IbHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTrader.Commands
{
    class ConnectToTws
    {
        public static void Run(ICommandVisitor visitor)
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
