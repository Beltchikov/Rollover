using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SsbHedger
{
    public class NotifyPropertyChangedImplementation : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
