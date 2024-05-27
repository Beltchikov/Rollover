using System.Collections.Generic;

namespace IbClient.IbHost
{
    public interface IRequestResponseMapper
    {
        void AddRequestId(int reqId);
        void AddResponse(int reqId, object response);
        IEnumerable<object> GetResponses(int reqId);
        T RemoveResponse<T>(int reqId) where T : class;
        List<T> RemoveResponses<T>(int reqId) where T : class;
    }
}