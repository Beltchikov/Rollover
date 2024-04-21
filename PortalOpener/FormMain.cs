using PortalOpener.Opener;
using System.Reflection;
using System.Text;

namespace PortalOpener
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

            //this.cmbOpener.
            //cmbOpener.Items.Add("Tokyo");
        }

        private void btGo_Click(object sender, EventArgs e)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            var openerTypes = executingAssembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IOpener)));
            var openerNames = openerTypes.Select(t => t.FullName).ToList();

            var assemblyName = executingAssembly.FullName;
            if (assemblyName != null)
            {
                var instance = Activator.CreateInstance(assemblyName, openerNames[0]);
                var unwraped = ((IOpener)instance.Unwrap());
                unwraped.Execute(new string[1]);
            }
            else throw new Exception("Unexpected! assemblyName is null");

            //Object o = Activator.CreateInstance(typeof(StringBuilder));

            //// Append a string into the StringBuilder object and display the
            //// StringBuilder.
            //StringBuilder sb = (StringBuilder)o;
            //sb.Append("Hello, there.");
            //MessageBox.Show(sb.ToString());

            ///////////////////////////////////////
        }
    }
}
