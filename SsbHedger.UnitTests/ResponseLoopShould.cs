namespace SsbHedger.UnitTests
{
    public class ResponseLoopShould
    {
        [Fact]
        public void CallActionAndExitOnExitCondition()
        {
            var counter = 1;
            var millisecondsToRun = 10;
            var startTime = DateTime.Now;
            
            var sut = new ResponseLoop();
            sut.BreakCondition = () => (DateTime.Now - startTime).Milliseconds > millisecondsToRun;
            sut.Actions = () => counter++;

            sut.Start();
            var endTime = DateTime.Now;

            Assert.True(counter > 2);
            Assert.True((endTime-startTime).Milliseconds >= millisecondsToRun);
        }
    }
}
