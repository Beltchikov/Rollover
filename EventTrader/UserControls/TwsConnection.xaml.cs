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
    /// Interaction logic for TwsConnection.xaml
    /// </summary>
    public partial class TwsConnection : UserControl
    {
        public TwsConnection()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public bool ConnectedToTws { get; set; }
        public bool TextFieldsAreEnabled
        {
            get
            {
                return !ConnectedToTws;
            }
        }

        public string ButtonContent
        {
            get
            {
                return ConnectedToTws
                    ? "Disconnect"
                    :"Connect to TWS";
            }
        }
    }
}
