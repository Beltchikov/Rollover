using Rollover.Ib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rollover.UnitTests
{
    public class ConnectedConditionShould
    {
        [Fact]
        void ReturnFalseIfNoErrorCode2104()
        {
            List<string> inputList = new List<string>
            {
                "errorCode=2104 msg=Market data farm connection is OK:eufarm Exception=",
                "Accounts found: U7292073"
            };

            IConnectedCondition sut = new ConnectedCondition();
            inputList.ForEach(i => sut.AddInput(i));

            var result = sut.IsConnected();
            Assert.False(result);
        }

        [Fact]
        void ReturnFalseIfNoIdMinusOne()
        {
            List<string> inputList = new List<string>
            {
                "id=-1 msg=Market data farm connection is OK:eufarm Exception=",
                "Accounts found: U7292073"
            };

            IConnectedCondition sut = new ConnectedCondition();
            inputList.ForEach(i => sut.AddInput(i));

            var result = sut.IsConnected();
            Assert.False(result);
        }

        [Fact]
        void ReturnFalseIfNoMarketData()
        {
            List<string> inputList = new List<string>
            {
                "id=-1 errorCode=2104 msg= farm connection is OK:eufarm Exception=",
                "Accounts found: U7292073"
            };

            IConnectedCondition sut = new ConnectedCondition();
            inputList.ForEach(i => sut.AddInput(i));

            var result = sut.IsConnected();
            Assert.False(result);
        }

        [Fact]
        void ReturnFalseIfNoOk()
        {
            List<string> inputList = new List<string>
            {
                "id=-1 errorCode=2104 msg=Market data farm connection is:eufarm Exception=",
                "Accounts found: U7292073"
            };

            IConnectedCondition sut = new ConnectedCondition();
            inputList.ForEach(i => sut.AddInput(i));

            var result = sut.IsConnected();
            Assert.False(result);
        }

        [Fact]
        void ReturnFalseIfNoAccountsFound()
        {
            List<string> inputList = new List<string>
            {
                "id=-1 errorCode=2104 msg=Market data farm connection is OK:eufarm Exception=",
                "Disconnected"
            };

            IConnectedCondition sut = new ConnectedCondition();
            inputList.ForEach(i => sut.AddInput(i));

            var result = sut.IsConnected();
            Assert.False(result);
        }

        [Fact]
        void ReturnFalseIfLessThenTwoInputs()
        {
            List<string> inputList = new List<string>
            {
                "id=-1 errorCode=2104 msg=Market data farm connection is OK:eufarm Exception=   Accounts found: U7292073"
            };

            IConnectedCondition sut = new ConnectedCondition();
            inputList.ForEach(i => sut.AddInput(i));

            var result = sut.IsConnected();
            Assert.False(result);
        }

        [Fact]
        void ReturnFalseIfDisconnectedFound()
        {
            List<string> inputList = new List<string>
            {
                "id=-1 errorCode=2104 msg=Market data farm connection is OK:eufarm Exception=",
                "Accounts found: U7292073",
                "Disconnected"
            };

            IConnectedCondition sut = new ConnectedCondition();
            inputList.ForEach(i => sut.AddInput(i));

            var result = sut.IsConnected();
            Assert.False(result);
        }

        [Fact]
        void ReturnFalseIfConnectedNotFound()
        {
            List<string> inputList = new List<string>
            {
                "id=-1 errorCode=2104 msg=Market data farm connection is OK:eufarm Exception=",
                "Accounts found: U7292073",
                "Something"
            };

            IConnectedCondition sut = new ConnectedCondition();
            inputList.ForEach(i => sut.AddInput(i));

            var result = sut.IsConnected();
            Assert.False(result);
        }

        [Fact]
        void ReturnTrueIfAllConditionsMet()
        {
            List<string> inputList = new List<string>
            {
                "id=-1 errorCode=2104 msg=Market data farm connection is OK:eufarm Exception=",
                "Accounts found: U7292073",
                "Connected"
            };

            IConnectedCondition sut = new ConnectedCondition();
            inputList.ForEach(i => sut.AddInput(i));

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
