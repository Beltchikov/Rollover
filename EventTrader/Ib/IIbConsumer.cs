using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Eomn.Ib
{
    public interface IIbConsumer
    {
        public bool ConnectedToTws { get; set; }
        public List<string>? TwsMessageList { get; internal set; }
        public ObservableCollection<string> TwsMessageCollection{ get; internal set; }
    }
}
