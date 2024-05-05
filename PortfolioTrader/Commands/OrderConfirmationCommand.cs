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
            if (MessageBox.Show("Do you want to use pair orders?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var pairOrdersConfirmationWindow = new PairOrdersConfirmationWindow(visitor);
                pairOrdersConfirmationWindow.ShowDialog();
            }
            else
            {
                var buyConfirmationWindow = new BuyConfirmationWindow(visitor);
                buyConfirmationWindow.ShowDialog();
            }

           
        }
    }
}
