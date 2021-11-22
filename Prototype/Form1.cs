using IBApi;
using IBSampleApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prototype
{
    public partial class Form1 : Form
    {
        private EReaderMonitorSignal signal;
        private IBClient ibClient;


        public Form1()
        {
            InitializeComponent();

            signal = new EReaderMonitorSignal();
            ibClient = new IBClient(signal);

            ibClient.Error += ibClient_Error;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                string host = txtHost.Text;
                int port = Int32.Parse(txtPort.Text);
                int clientId = Int32.Parse(txtClientId.Text);

                ibClient.ClientSocket.eConnect(host, port, ibClient.ClientId);

                // The EReader Thread
                var reader = new EReader(ibClient.ClientSocket, signal);
                reader.Start();
                new Thread(() =>
                {
                    while (ibClient.ClientSocket.IsConnected())
                    {
                        signal.waitForSignal();
                        reader.processMsgs();
                    }
                })
                { IsBackground = true }
                .Start();
            }
            catch (Exception ex )
            {
                txtMessage.Text += Environment.NewLine + ex.ToString();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        void ibClient_Error(int id, int errorCode, string str, Exception ex)
        {
            txtMessage.Text += Environment.NewLine + str;
            
            //if (ex != null)
            //{
            //    addTextToBox("Error: " + ex);

            //    return;
            //}

            //if (id == 0 || errorCode == 0)
            //{
            //    addTextToBox("Error: " + str + "\n");

            //    return;
            //}

            //ErrorMessage error = new ErrorMessage(id, errorCode, str);

            //HandleErrorMessage(error);
        }
    }
}
