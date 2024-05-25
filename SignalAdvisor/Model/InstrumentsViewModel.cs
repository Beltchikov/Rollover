using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace SignalAdvisor.Model
{
    internal class InstrumentsViewModel : ObservableObject
    {
        private ObservableCollection<Instrument> _instruments = [];

        public ObservableCollection<Instrument> Instruments
        {
            get => _instruments;
            set
            {
                SetProperty(ref _instruments, value);
            }
        }
    }
}
