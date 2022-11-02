using System;

namespace SsbHedger.ResponseProcessing
{
    public record ErrorInfo(
        int ReqId,
        int Code,
        string Message,
        Exception? exception);
    
}
