﻿using IbClient.messages;
using System;

namespace SsbHedger.ResponseProcessing
{
    public interface IResponseHandler
    {
        void OnNextValidId(ConnectionStatusMessage connectionStatusMessage);
        void OnError(int refId, int code, string message, Exception exception);
        void OnManagedAccounts(ManagedAccountsMessage managedAccountsMessage);
        void OnOpenOrder(OpenOrderMessage obj);
        void OnOpenOrderEnd();
        void OnOrderStatus(OrderStatusMessage obj);
        object? Dequeue();
        void HandleNextMessage();
    }
}