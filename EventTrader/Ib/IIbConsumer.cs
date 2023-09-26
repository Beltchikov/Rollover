using System.Collections.Generic;

namespace Eomn.Ib
{
    public interface IIbConsumer
    {
        public bool ConnectedToTws { get; set; }
        public List<string>? TwsMessageList { get; internal set; }
    }
}
