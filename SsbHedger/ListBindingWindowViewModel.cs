using SsbHedger;
using SsbHedger.Model;
using System.Collections.ObjectModel;

namespace ViewModel.ListBinding
{
    public class ListBindingWindowViewModel : NotifyPropertyChangedImplementation
    {
        private ObservableCollection<Message> messages = new ObservableCollection<Message>();


        public ListBindingWindowViewModel()
        {
            Messages = new ObservableCollection<Message>
            {
                new Message() { ReqId = -1, Body = "ddd" }
            };
        }

        public ObservableCollection<Message> Messages
        {
            get { return messages; }

            set
            {
                if (messages != value)
                {
                    messages = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
