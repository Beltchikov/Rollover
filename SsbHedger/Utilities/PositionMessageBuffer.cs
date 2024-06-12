using IbClient.messages;
using IBSampleApp.messages;
using System.Collections.Generic;
using System.Linq;

namespace SsbHedger.Utilities
{
    public class PositionMessageBuffer : IPositionMessageBuffer
    {
        private List<PositionMessage> _messages = new List<PositionMessage>();

        public List<PositionMessage> Messages { get => _messages; }

        public void AddMessage(PositionMessage positionMessage)
        {
            _messages.Add(positionMessage);
        }

        public int? FirstCallSize()
        {
            if (!_messages.Any())
            {
                return null;
            }

            var callPositions = _messages.Where(m => m.Contract.Right == "C");
            return (int?)callPositions.First().Position;
        }


        public void Reset()
        {
            _messages.Clear();
        }

        int? IPositionMessageBuffer.FirstCallSize()
        {
            if (!_messages.Any())
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

        public double? FirstCallStrike()
        {
            if (!_messages.Any())
            {
                return null;
            }

            var callPositions = _messages.Where(m => m.Contract.Right == "C");
            return callPositions.First().Contract?.Strike;
        }

        public double? SecondCallStrike()
        {
            var callPositions = _messages.Where(m => m.Contract.Right == "C");

            if (callPositions.Count() < 2)
            {
                return null;
            }
            return callPositions.Skip(1).First().Contract?.Strike;
        }
    }
}
