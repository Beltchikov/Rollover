using AutoFixture.Xunit2;
using NSubstitute;
using Rollover.Ib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rollover.UnitTests
{
    public class RequestSenderShould
    {
        [Theory, AutoNSubstituteData]
        public void CallIbClientEConnectInConnect(
            [Frozen] IIbClientWrapper ibClient,
            RequestSender sut)
        {
            string host = "host1";
            int port = 398;
            int clientId = 29;

            sut.Connect(host, port, clientId);
            ibClient.Received().Connect(host, port, clientId);
        }
    }
}
