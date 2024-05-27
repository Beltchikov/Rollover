using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace IbClient.IbHost
{
    public class RequestResponseMapper : IRequestResponseMapper
    {
        private ConcurrentDictionary<int, ConcurrentBag<object>> _requestDictionary;

        public RequestResponseMapper()
        {
            _requestDictionary = new ConcurrentDictionary<int, ConcurrentBag<object>>();
        }

        public void AddRequestId(int reqId)
        {
            _requestDictionary[reqId] = new ConcurrentBag<object>();
        }

        public void AddResponse(int reqId, object response)
        {
            if (!_requestDictionary.ContainsKey(reqId))  
                _requestDictionary[reqId] = new ConcurrentBag<object> { response };
            else
                _requestDictionary[reqId].Add(response);

        }

        public IEnumerable<object> GetResponses(int reqId)
        {
            return _requestDictionary[reqId];
        }

        public T RemoveResponse<T>(int reqId) where T : class
        {
            object lockObject = new object();

            lock (lockObject)
            {
                var responseList = new ConcurrentBag<object>(_requestDictionary[reqId].ToArray());
                var response = responseList.FirstOrDefault(m => m is T);
                var responseCopy = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(response));

                if (response != null)
                {
                    ConcurrentBag<object> bagCopy = new ConcurrentBag<object>(_requestDictionary[reqId]);
                    var listCopy = bagCopy.ToList();
                    if (!listCopy.Remove(response)) throw new Exception("Can not remove response");
                    bagCopy = new ConcurrentBag<object>(listCopy);

                    _requestDictionary[reqId] = bagCopy;
                    return responseCopy;
                }

                return null;
            }

        }

        public List<T> RemoveResponses<T>(int reqId) where T : class
        {
            throw new NotImplementedException();
        }
    }
}