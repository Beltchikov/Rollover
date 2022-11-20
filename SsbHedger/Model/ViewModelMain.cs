using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SsbHedger.Model
{
    public class ViewModelMain : NotifyPropertyChangedImplementation
    {
        private ObservableCollection<Message> _messages = new ObservableCollection<Message>();


        public ViewModelMain()
        {
            Messages = new ObservableCollection<Message>();
            //_messages.Add(new MessageListBoxItem { Content = "55555" });
            Messages.Add(new Message { ReqId = 0, Body = "TTT"});
        }

        public ObservableCollection<Message> Messages
        {
            get { return _messages; }

            set
            {
                _messages = value;
                RaisePropertyChanged();
            }
        }
    }
}
