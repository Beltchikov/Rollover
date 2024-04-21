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

            Type[] openerTypes = executingAssembly.GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IOpener)))
                .ToArray();
            openerNames = openerTypes.Select(t => t.FullName == null? "NULL" : t.FullName).ToArray();

            cmbOpener.Items.AddRange(openerNames);
            cmbOpener.SelectedItem = openerNames.First(n => n.EndsWith("SeekingAlphaOpener"));
        }

        private void btGo_Click(object sender, EventArgs e)
        {
            var assemblyName = executingAssembly.FullName;
            if (assemblyName != null)
            {
                var openerWraped = Activator.CreateInstance(assemblyName, openerNames[0]);
                if(openerWraped != null)
                {
                    var openerUnwrapped = openerWraped.Unwrap();
                    if (openerUnwrapped != null)
                    {
                        var opener = (IOpener)openerUnwrapped;
                        opener.Execute(new string[1]);
                    }
                    else throw new Exception("Unexpected! openerUnwrapped is null");
                }
                else
                {
                    throw new Exception("Unexpected! openerWraped is null");
                }
            }
            else throw new Exception("Unexpected! assemblyName is null");
        }
    }
}
