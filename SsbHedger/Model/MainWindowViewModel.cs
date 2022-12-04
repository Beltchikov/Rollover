using System.Collections.ObjectModel;

namespace SsbHedger.Model
{
    public class MainWindowViewModel : NotifyPropertyChangedImplementation
    {
        private ObservableCollection<Message> messages = new ObservableCollection<Message>();

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
