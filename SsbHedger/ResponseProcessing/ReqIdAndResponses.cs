using System.Collections.Generic;

namespace SsbHedger.ResponseProcessing
{
    public record ReqIdAndResponses (int ReqId, List<object> Response);
}
