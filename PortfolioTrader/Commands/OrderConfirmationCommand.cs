namespace SignalAdvisor.Commands
{
    internal class OrderConfirmationCommand
    {
        public static void Run(IBuyModelVisitor visitor)
        {
            var buyConfirmationWindow = new BuyConfirmationWindow(visitor);
            buyConfirmationWindow.ShowDialog();

            // Pair orders are not actual anymore. 
            //if (MessageBox.Show("Do you want to use pair orders?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            //{
            //    var pairOrdersConfirmationWindow = new PairOrdersConfirmationWindow(visitor);
            //    pairOrdersConfirmationWindow.ShowDialog();
            //}
        }
    }
}
