using System.Collections;
using Ta;

namespace SignalsTest
{
    public class SignalsShould
    {
        public class InsideUpDown
        {

            [Theory]
            [ClassData(typeof(NoSignalTestData))]
            public void Return0IfNoSignal(List<Bar> bars)
            {
                Assert.Equal(0, Signals.InsideUpDown(bars));
            }

            public class NoSignalTestData : IEnumerable<object[]>
            {
                public NoSignalTestData()
                {

                }

                private readonly List<object[]> _data = new List<object[]>
                {
                    new []{
                        new List<Bar>(){
                            new Bar(10d, 15d, 9d, 11d, DateTime.Now)
                        }
                } };

                public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            }
        }

    }
}