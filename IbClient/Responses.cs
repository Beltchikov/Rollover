﻿using IbClient.messages;
using IbClient.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IbClient
{
    public class Responses : IResponses
    {
        private List<TickPriceMessage> _tickPriceMessages { get; set; }
        private List<TickSizeMessage> _tickSizeMessages { get; set; }

        public Responses()
        {
            _tickPriceMessages = new List<TickPriceMessage>();
            _tickSizeMessages = new List<TickSizeMessage>();
        }
        public bool TryGetValidPrice(
            int reqId,
            Predicate<TickPriceMessage> ValidPriceCallback,
            out double? price,
            out TickType? tickType)
        {
            var tickPriceMessagesCopy = _tickPriceMessages.ToArray();   
            TickPriceMessage tickPriceMessage = tickPriceMessagesCopy.FirstOrDefault(m => m.RequestId == reqId);
           
            if (tickPriceMessage == null)
            {
                price = null;
                tickType = null;    
                return false;
            }
            else
            {
                if (ValidPriceCallback(tickPriceMessage))
                {
                    price = tickPriceMessage.Price;
                    tickType = (TickType)Enum.Parse(typeof(TickType), tickPriceMessage.Field.ToString());
                    return true;
                }
                else
                {
                    price = null;
                    tickType = (TickType)Enum.Parse(typeof(TickType), tickPriceMessage.Field.ToString());
                    return false; 
                }   
            }
        }

        public void AddTickPriceMessage(TickPriceMessage tickPriceMessage)
        {
            object lockObject = new object();
            lock (lockObject)
            {
                _tickPriceMessages.Add(tickPriceMessage);
            }
        }
    }
}
