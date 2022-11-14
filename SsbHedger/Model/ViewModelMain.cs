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
        private ObservableCollection<string> _messages;


        public ViewModelMain()
        {
            _messages = new ObservableCollection<string>();
            //_messages.Add(new MessageListBoxItem { Content = "55555" });
            _messages.Add("55555");
        }

        public ObservableCollection<string> Messages
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
