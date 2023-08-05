using System;
using System.Windows;

namespace EventTrader.Requests
{
    public class RequestLoop : IInfiniteLoop
    {
        public void Start(Action action, object[] parameters)
        {
            MessageBox.Show("NotImplementedException");
        }
    }
}
