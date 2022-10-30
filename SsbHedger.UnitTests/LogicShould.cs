using AutoFixture.Xunit2;
using IbClient;
using IbClient.messages;
using NSubstitute;
using System.Reflection;

namespace SsbHedger.UnitTests
{
    public class LogicShould
    {
        [Theory]
        [InlineData("NextValidId")]
        [InlineData("Error")]
        [InlineData("ManagedAccounts")]
        [InlineData("ContractDetails")]
        [InlineData("TickPrice")]
        [InlineData("TickSize")]
        [InlineData("TickString")]
        [InlineData("TickOptionCommunication")]
        [InlineData("OpenOrder")]
        [InlineData("OpenOrderEnd")]
        [InlineData("OrderStatus")]
        public void AttachEventHandlers(string eventName)
        {
            var ibClient = IBClient.CreateClient();
            var sut = new Logic(ibClient);
            sut.Execute();
            VerifyDelegateAttachedTo(ibClient, eventName);
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientConnectAndStartReaderThread(
            [Frozen] IIBClient ibClient,
            Logic sut)
        {
            sut.Execute();
            ibClient.Received().ConnectAndStartReaderThread(
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<int>());
        }

        [Theory, AutoNSubstituteData]
        public void CallNextValidIdHandler(
            ConnectionStatusMessage connectionStatusMessage,
            [Frozen] IIBClient ibClient)
        {
            var wasCalled = false;
            ibClient.NextValidId += (m) =>wasCalled = true;

            ibClient.NextValidId += Raise.Event<Action<ConnectionStatusMessage>>(
                connectionStatusMessage);
            
            Assert.True(wasCalled);
        }


        //[Theory, AutoNSubstituteData]
        //public void CallResponseQueueOnNextValidId(
        //   ConnectionStatusMessage connectionStatusMessage,
        //   [Frozen] IIBClient ibClient,
        //   Logic sut)
        //{
        //    var wasCalled = false;
        //    ibClient.NextValidId += (m) =>
        //    {
        //        wasCalled = true;
        //    };

        //    ibClient.NextValidId += Raise.Event<Action<ConnectionStatusMessage>>(
        //        connectionStatusMessage);
        //    Assert.True(wasCalled);

        //}

        private void VerifyDelegateAttachedTo(object objectWithEvent, string eventName)
        {
            var allBindings = BindingFlags.IgnoreCase | BindingFlags.Public |
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            var type = objectWithEvent.GetType();
            var fieldInfo = type.GetField(eventName, allBindings);

            var value = fieldInfo?.GetValue(objectWithEvent);

            var handler = value as Delegate;
            Assert.NotNull(handler);
        }

    }

}