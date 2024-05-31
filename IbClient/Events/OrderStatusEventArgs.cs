using IBSampleApp.messages;
using System;

namespace IbClient.Events
{
    public class OrderStatusEventArgs : EventArgs
    {
        public OrderStatusMessage Message { get; set; }


        public OrderStatusEventArgs(OrderStatusMessage message)
        {
            Message = message;
        }
    }
}