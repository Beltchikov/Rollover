using System.Collections.ObjectModel;

namespace SsbHedger.Model
{
    public class MainWindowViewModel : NotifyPropertyChangedImplementation
    {
        private ObservableCollection<Message> messages = new ObservableCollection<Message>();
        private string connectionMessage = "";

        public string ConnectionMessage
        {
            get => connectionMessage;
            set
            {
                connectionMessage = value;
                RaisePropertyChanged();
            }
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
