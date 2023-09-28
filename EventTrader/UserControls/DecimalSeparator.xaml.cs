using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Eomn.UserControls
{
    /// <summary>
    /// Interaction logic for DecimalSeparator.xaml
    /// </summary>
    public partial class DecimalSeparator : UserControl
    {
        public DecimalSeparator()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string Separator { get; set; }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
