using IBSampleApp.messages;
using Rollover.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Rollover.Ib
{
    public class MessageProcessor : IMessageProcessor
    {
        private IPortfolio _portfolio;

        public MessageProcessor(IPortfolio portfolio)
        {
            _portfolio = portfolio;
        }

        private static List<PositionMessage> _positionMessageList = new List<PositionMessage>();

        public List<string> ConvertMessage(object obj)
        {
            if(obj == null)
            {
                return new List<string>();
            }
            else if (obj is string)
            {
                return ConvertMessageString(obj as string);
            }
            else if (obj is ConnectionStatusMessage)
            {
                return (obj as ConnectionStatusMessage).IsConnected
                    ? new List<string> { "Connected." }
                    : new List<string> { "Disconnected." };
            }
            else if (obj is ManagedAccountsMessage)
            {
                if (!(obj as ManagedAccountsMessage).ManagedAccounts.Any())
                {
                    throw new Exception("Unexpected: no positions.");
                }

                string msg = Environment.NewLine + "Accounts found: "
                    + (obj as ManagedAccountsMessage).ManagedAccounts.Aggregate((r, n) => r + ", " + n);
                return new List<string> { msg };
            }
            else if (obj is PositionMessage)
            {
                if ((obj as PositionMessage).Position > 0)
                {
                    _positionMessageList.Add((obj as PositionMessage));
                }

                return new List<string>();
            }
            
            throw new NotImplementedException();
        }

        private List<string> ConvertMessageString(string obj)
        {
            switch (obj)
            {
                case Constants.ON_POSITION_END:
                    _positionMessageList.ForEach(p => _portfolio.Add(p));
                    
                    List<string> resultList = _positionMessageList.Select(x => x.Contract.LocalSymbol)
                        .OrderBy(x => x).ToList();
                    resultList.Add(Constants.ENTER_SYMBOL_TO_TRACK);
                    _positionMessageList = new List<PositionMessage>();
                    return resultList;
               
                default:
                    return new List<string> { obj as string };
            }
        }
    }
}
