using OptionHelper;

namespace OptionHelperUi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            txtUnderlyingPrice.Text = "36,07";
            txtStrike.Text = "35";
            txtVolatility.Text = "0,4825";

            txtInterestRate.Text = "0,01";
            txtDividendYield.Text = "0";
            txtDaysToExpiration.Text = "26";

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
            //var result = sut.CallPrice(
            //   underlying,
            //   strike,
            //   volatility,
            //   interestRate,
            //   dividendYield,
            //   daysToExpiration);
        }
    }
}
