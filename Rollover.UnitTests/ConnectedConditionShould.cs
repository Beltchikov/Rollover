using IBSampleApp.messages;
using Rollover.Ib;
using System.Collections.Generic;
using Xunit;

namespace Rollover.UnitTests
{
    public class ConnectedConditionShould
    {
        [Fact]
        void ReturnTrueIfNotAllConditionsMet()
        {
            List<string> inputList = new List<string>
            {
                "id=-1 errorCode=2104 msg=Market data farm connection is OK:eufarm Exception=",
                "Accounts found: U7292073",
                "Something"
            };

            IConnectedCondition sut = new ConnectedCondition();
            inputList.ForEach(i => sut.AddMessage(i));

            var result = sut.IsConnected();
            Assert.False(result);
        }

        [Fact]
        void ReturnTrueIfAllConditionsMet()
        {
            List<object> messageList = new List<object>
            {
                new ManagedAccountsMessage("aaa,bbb"),
                new ConnectionStatusMessage(true)
            };

            IConnectedCondition sut = new ConnectedCondition();
            messageList.ForEach(i => sut.AddMessage(i));

            var result = sut.IsConnected();
            Assert.True(result);
        }

        // id = -1 errorCode = 2104 msg = Market data farm connection is OK:usfarm.nj Exception =

        // Accounts found: U7292073
        // id = -1 errorCode = 2104 msg = Market data farm connection is OK:eufarm Exception =
        // id = -1 errorCode = 2104 msg = Market data farm connection is OK:usfuture Exception =
        // id = -1 errorCode = 2104 msg = Market data farm connection is OK:cashfarm Exception =
        // id = -1 errorCode = 2104 msg = Market data farm connection is OK:usopt Exception =
        // id = -1 errorCode = 2104 msg = Market data farm connection is OK:usfarm Exception =
        // id = -1 errorCode = 2106 msg = HMDS data farm connection is OK:euhmds Exception =
        // id = -1 errorCode = 2106 msg = HMDS data farm connection is OK:ushmds Exception =
        // id = -1 errorCode = 2158 msg = Sec - def data farm connection is OK:secdefil Exception =

    }
}
