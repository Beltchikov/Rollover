using System.Windows;

namespace NpvManager.Commands
{
    class LoadPositions
    { 
        public static async Task RunAsync(IPositionsVisitor visitor)
        {
            bool positionsRequested = false;
            await visitor.IbHost.RequestPositions(
               (p) =>
               {
                   visitor.Positions.Add(p);
               },
               () =>
               {
                   positionsRequested = true;

               });

            await Task.Run(() => { while (!positionsRequested) { }; });
            MessageBox.Show("Positions loaded!");
        }
    }
}
