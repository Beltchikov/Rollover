﻿using AutoFixture.Xunit2;
using IBApi;
using IBSampleApp.messages;
using NSubstitute;
using Rollover.Ib;
using Rollover.Tests.Shared;
using Rollover.Tracking;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Rollover.UnitTests
{
    public class RepositoryShould
    {
        [Theory, AutoNSubstituteData]
        public void CallMessageCollectorEConnectInConnect(
            [Frozen] IMessageCollector messageCollector,
            Repository sut)
        {
            string host = "host1";
            int port = 398;
            int clientId = 29;

            sut.Connect(host, port, clientId);
            messageCollector.Received().eConnect(host, port, clientId);
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientDisconnect(
            [Frozen] IIbClientWrapper ibClient,
            Repository sut)
        {
            sut.Disconnect();
            ibClient.Received().eDisconnect();
        }

        [Theory, AutoNSubstituteData]
        public void CallsMessageCollectorReqPositionsInAllPositions([Frozen] IMessageCollector messageCollector)
        {
            var sut = new Repository(null, null, messageCollector);
            sut.AllPositions();
            messageCollector.Received().reqPositions();
        }



        [Theory, AutoNSubstituteData]
        public void CallMessageCollectorEConnect(
            [Frozen] IMessageCollector messageCollector,
            [Frozen] IMessageProcessor messageProcessor)
        {
            messageProcessor.ConvertMessage(Arg.Any<object>())
                .Returns(new List<string> { "Some message" });

            IRepository sut = new Repository(null, messageProcessor, messageCollector);
            sut.Connect("localhost", 4001, 1);
            messageCollector.Received().eConnect("localhost", 4001, 1);
        }

        [Theory, AutoNSubstituteData]
        public void CallMessageCollectorReqPositions(
            [Frozen] IMessageCollector messageCollector)
        {
            IRepository sut = new Repository(null, null, messageCollector);
            sut.AllPositions();
            messageCollector.Received().reqPositions();
        }

        [Theory, AutoNSubstituteData]
        public void CallMessageCollectorPlaceOrder(
            Contract contract,
            Order order,
            [Frozen] IMessageCollector messageCollector,
            Repository sut)
        {
            sut.PlaceOrder(contract, order);
            messageCollector.Received().placeOrder(contract, order);
        }

        [Theory, AutoNSubstituteData]
        public void CallMessageCollectorReqIds(
            Contract contract,
            Order order,
            [Frozen] IMessageCollector messageCollector,
            Repository sut)
        {
            sut.PlaceOrder(contract, order);
            messageCollector.Received().reqId();
        }

        [Theory, AutoNSubstituteData]
        public void CheckTradingHours(Repository sut)
        {
            //var tradingHoursString = "20220214:0900-20220214:1730;" +
            //    "20220215:0900-20220215:1730;20220216:0900-20220216:1730;" +
            //    "20220217:0900-20220217:1730;20220218:0900-20220218:1730";

            //var monday10 = DateTime.Parse("14.02.2022 11:02", new CultureInfo("DE-de"));
            //bool result = sut.IsInTradingHours(Helper.DaxIndContract, monday10);
            //Assert.True(result);


        }
    }
}
