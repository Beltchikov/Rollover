namespace NetBms
{
    public partial class FormStart : Form
    {
        public FormStart()
        {
            InitializeComponent();
        }

        private void btGo_Click(object sender, EventArgs e)
        {
            var input = txtBuy.Text;
            
            
            var formResults = new FormResults();
            formResults.ShowDialog();
        }
    }
}
