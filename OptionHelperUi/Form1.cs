using OptionHelper;
using System.Globalization;

namespace OptionHelperUi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            txtUnderlyingPrice.Text = "606,5";
            txtStrike.Text = "607";
            txtVolatility.Text = "0,4825";

            txtInterestRate.Text = "0,045";
            txtDividendYield.Text = "0";
            txtDaysToExpiration.Text = "0,27";

            cbIsCall.Checked = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void btCalculatePrice_Click(object sender, EventArgs e)
        {
            var sut = new BlackScholes();
            double result = cbIsCall.Checked
                ? sut.CallPrice(Convert.ToDouble(txtUnderlyingPrice.Text),
                   Convert.ToDouble(txtStrike.Text),
                   Convert.ToDouble(txtVolatility.Text),
                   Convert.ToDouble(txtInterestRate.Text),
                   Convert.ToDouble(txtDividendYield.Text),
                   Convert.ToDouble(txtDaysToExpiration.Text))
                : sut.PutPrice(Convert.ToDouble(txtUnderlyingPrice.Text),
                   Convert.ToDouble(txtStrike.Text),
                   Convert.ToDouble(txtVolatility.Text),
                   Convert.ToDouble(txtInterestRate.Text),
                   Convert.ToDouble(txtDividendYield.Text),
                   Convert.ToDouble(txtDaysToExpiration.Text));

            txtPrice.Text = result.ToString("F3", new CultureInfo("DE-de"));   
        }
    }
}
