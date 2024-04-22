using PortalOpener.Opener;
using System.Reflection;
using System.Text;

namespace PortalOpener
{
    public partial class FormMain : Form
    {
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        string[] openerNames;

        public FormMain()
        {
            InitializeComponent();

            LoadTestData();

            Type[] openerTypes = executingAssembly.GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IOpener)))
                .ToArray();
            openerNames = openerTypes.Select(t => t.FullName == null? "NULL" : t.FullName).ToArray();

            cmbOpener.Items.AddRange(openerNames);
            cmbOpener.SelectedItem = openerNames.First(n => n.EndsWith("SeekingAlphaOpener"));
        }

        private void LoadTestData()
        {
            txtSymbols.Text = @"GOOG
MSFT";
        }

        private void btGo_Click(object sender, EventArgs e)
        {
            var assemblyName = executingAssembly.FullName;
            if (assemblyName == null) throw new Exception("Unexpected! assemblyName is null");

            var selectedItem = cmbOpener.SelectedItem;
            if (selectedItem == null) throw new Exception("Unexpected! selectedItem is null");

            var selectedOpenerFullName = selectedItem.ToString();
            if (selectedOpenerFullName == null) throw new Exception("Unexpected! selectedOpenerFullName is null");

            var openerWraped = Activator.CreateInstance(assemblyName, selectedOpenerFullName.ToString());
            if (openerWraped == null) throw new Exception("Unexpected! openerWraped is null");

            var openerUnwrapped = openerWraped.Unwrap();
            if (openerUnwrapped == null) throw new Exception("Unexpected! openerUnwrapped is null");

            var opener = (IOpener)openerUnwrapped;
            var openerResult = opener.Execute(new string[1]);

        }
    }
}
