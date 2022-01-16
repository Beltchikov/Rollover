using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prototype
{
    public partial class FormContractId : Form
    {
        public FormContractId()
        {
            InitializeComponent();
        }

        private void FormContractId_Load(object sender, EventArgs e)
        {
            txtDocumentation.Text = "DAX: \r\n" +
                "DAX EUR OPT DTB\r\n" +
                "20220318 16300 C\r\n" +
                "\r\n" +
                "DAX: \r\n" +
                "DAX EUR IND DTB\r\n";
        }
    }
}
