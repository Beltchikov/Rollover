using Rollover.Ib;
using Rollover.Input;

namespace Rollover
{
    public class TwsConnector : ITwsConnector
    {
        private IInputQueue _inputQueue;
        private IRepository _repository;

        public TwsConnector(
            IInputQueue inputQueue, 
            IRepository repository)
        {
            _inputQueue = inputQueue;
            _repository = repository;
        }

        public void Connect(string host, int port, int clientId)
        {
            // Connect
            var connectedResult = _repository.Connect(host, port, clientId);
            if (!connectedResult.Success)
            {
                connectedResult.Value.ForEach(m => _inputQueue.Enqueue(m));
                _inputQueue.Enqueue(Constants.CAN_NOT_CONNECT);
                return;
            }
            connectedResult.Value.ForEach(m => _inputQueue.Enqueue(m));
        }
    }
}