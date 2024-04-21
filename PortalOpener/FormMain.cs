using PortalOpener.Opener;
using System.Reflection;

namespace PortalOpener
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void btGo_Click(object sender, EventArgs e)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var openerTypes = types.Where(t => t.GetInterfaces().Contains(typeof(IOpener)));
        }
    }
}
