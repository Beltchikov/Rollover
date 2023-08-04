using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Input;

namespace EventTrader
{
    public class ViewModel: ObservableObject
    {
        public ICommand StartSessionCommand { get; }

        public ViewModel()
        {
            StartSessionCommand = new RelayCommand(() => MessageBox.Show("USD 2"));
        }
        
    }
}
