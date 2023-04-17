using IbClient.messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsbHedger.MessageHelper
{
    public class PositionMessageBuffer : IPositionMessageBuffer
    {
        private List<PositionMessage> _messages = new List<PositionMessage>();    
        
        public void AddMessage(PositionMessage positionMessage)
        {
            _messages.Add(positionMessage);
        }

        public int? FirstCallSize()
        {
            if(!_messages.Any())
            {
                return null;
            }

            var callPositions = _messages.Where(m => m.Contract.Right == "C");
            return (int?)callPositions.First().Position;
        }


        public int? SecondCallSize()
        {
            var callPositions = _messages.Where(m => m.Contract.Right == "C");

            if (callPositions.Count() < 2)
            {
                return null;
            }
            return (int?)callPositions.Skip(1).First().Position;
        }

        public void Reset()
        {
            _messages.Clear();
        }
    }
}
