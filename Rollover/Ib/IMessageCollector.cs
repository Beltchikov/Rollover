using IBSampleApp.messages;
using System.Collections.Generic;

namespace Rollover.Ib
{
    public interface IMessageCollector
    {
        ConnectionMessages eConnect(string host, int port, int clientId);
        public List<PositionMessage> reqPositions();
    }
}