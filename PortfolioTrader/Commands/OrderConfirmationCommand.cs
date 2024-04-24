using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PortfolioTrader.Commands
{
    internal class OrderConfirmationCommand
    {
        public static void Run(IBuyModelVisitor visitor)
        {
            // TODO
            var buyConfirmationWindow = new BuyConfirmationWindow();
            buyConfirmationWindow.ShowDialog(); 
        }
    }
}
