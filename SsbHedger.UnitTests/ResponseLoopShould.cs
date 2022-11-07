namespace SsbHedger.UnitTests
{
    public class ResponseLoopShould
    {
        [Fact]
        public void CallActionIfStartedOnNewThtread()
        {
            var counter = 1;
            var millisecondsToRun = 10;
            var startTime = DateTime.Now;
            
            var sut = new ResponseLoop();
            sut.BreakCondition = () => 1==0;
            sut.Actions = () => counter++;

            new Thread(() =>
            {
                sut.Start();
            })
            { IsBackground = true }
           .Start();

            Thread.Sleep(millisecondsToRun);
            Assert.True(counter > 2);
        }
    }
}
