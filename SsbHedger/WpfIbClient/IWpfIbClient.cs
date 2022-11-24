﻿using SsbHedger.WpfIbClient.ResponseObservers;
using System;

namespace SsbHedger.WpfIbClient
{
    public interface IWpfIbClient : IObservable<Connection>
    {
        event Action<int, bool> NextValidId;
        event Action<int, string> Error;

        void Connect(string host, int port, int clientId);
        void InvokeError(int reqId, string message);
    }
}