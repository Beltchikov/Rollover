using SsbHedger.WpfIbClient.ResponseObservers;

namespace SsbHedger.UnitTests.WpfIbClient.ResponseObserver
{
    public class UnsubscriberShould
    {
        [Theory, AutoNSubstituteData]
        public void RemoveObservers(Unsubscriber<string> sut)
        {
            var countObservers = sut._observers.Count();

            sut._observer = sut._observers.First();
            sut.Dispose();
            sut._observer = sut._observers.First();
            sut.Dispose();

            Assert.Single(sut._observers);
        }
    }
}
