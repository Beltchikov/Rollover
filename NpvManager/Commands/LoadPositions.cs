using System.Windows;

namespace NpvManager.Commands
{
    class LoadPositions
    { 
        public static async Task RunAsync(IPositionsVisitor visitor)
        {
            bool positionsRequested = false;
            visitor.Positions.Clear();
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
            
            visitor.Input = "";
            visitor.Input = visitor.Positions
                .Select(p => p.Contract.ToString())
                .Aggregate((r,n)=>r + Environment.NewLine +n);    
            MessageBox.Show("Positions loaded!");
        }
    }
}
