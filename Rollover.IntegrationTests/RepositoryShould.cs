using System;
using System.Diagnostics;
using Xunit;

namespace Rollover.IntegrationTests
{
    public class RepositoryShould
    {
        [Fact]
        public void ConnectFastToTestAccount()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday
                || DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                return;
            }

            var repository = Tests.Shared.Helper.RepositoryFactory();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var connnectionTuple = repository.Connect(
                Tests.Shared.Helper.HOST, 
                Tests.Shared.Helper.PORT, 
                Tests.Shared.Helper.RandomClientId());
            repository.Disconnect();


            Assert.True(connnectionTuple.Item1);
            Assert.Contains(connnectionTuple.Item2, m => m.Contains("DU4798064"));
            Assert.InRange(stopwatch.Elapsed.TotalMilliseconds, 0, 3000);
        }
    }
}
