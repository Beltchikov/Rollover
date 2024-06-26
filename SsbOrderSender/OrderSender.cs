﻿using IbClient;
using IbClient.IbHost;
using IBSampleApp.messages;

namespace SsbOrderSender
{
    public class OrderSender : IOrderSender
    {
        const string HOST = "localhost";
        const int PORT = 4001;
        const int CLIENT_ID = 1;
        IIBClient _ibClient = null!;

        public void Run()
        {
            _ibClient = IBClient.CreateClient();

            _ibClient.Error -= _ibClient_Error;
            _ibClient.Error += _ibClient_Error;

            _ibClient.NextValidId -= _ibClient_NextValidId;
            _ibClient.NextValidId += _ibClient_NextValidId;

            // See Rollover project for how to connect from a concole app
            //_ibClient.ConnectAndStartReaderThread(HOST, PORT, CLIENT_ID);

            Console.WriteLine("SsbOrderSender: press Q to quit");
            while (Console.ReadKey().KeyChar.ToString().ToUpper() != "Q") { };
        }

        void _ibClient_Error(int id, int errorCode, string msg, string orderRejectionReason,Exception ex)
        {
            string msgError = $"OnError: id={id} errorCode={errorCode} msg={msg} exception={ex}";
            Console.WriteLine(msgError);
        }

        void _ibClient_NextValidId(ConnectionStatusMessage statusMessage)
        {
            string msgNextValidId = statusMessage.IsConnected
                ? $"Connected to TWS! HOST: {HOST} PORT: {PORT} CLIENT_ID: {CLIENT_ID} Next Order ID: {_ibClient.NextOrderId}"
                : $"Error while connecting to TWS! HOST: {HOST} PORT: {PORT} CLIENT_ID: {CLIENT_ID}";
            Console.WriteLine(msgNextValidId);
        }
    }
}
