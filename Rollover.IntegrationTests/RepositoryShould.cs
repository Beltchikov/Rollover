using System;
using System.Diagnostics;
using Xunit;

namespace Rollover.IntegrationTests
{
    public class RepositoryShould
    {
        [Fact]
        public void ConnectFast()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday
                || DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                return;
            }

            var repository = Helper.RepositoryFactory();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var connnectionTuple = repository.Connect(Helper.HOST, Helper.PORT, Helper.RandomClientId());
            repository.Disconnect();


            Assert.True(connnectionTuple.Item1);
            Assert.InRange(stopwatch.Elapsed.TotalMilliseconds, 0, 3000);
        }
    }
}
