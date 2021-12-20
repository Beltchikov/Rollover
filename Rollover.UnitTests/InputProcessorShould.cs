using Rollover.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rollover.UnitTests
{
    public class InputProcessorShould
    {
        [Fact]
        public void ReturnInputIfStateIsConnecting()
        {
            var testInput = "TEST";
            var sut = new InputProcessor();

            var resultList = sut.Convert(testInput, "Connecting", null, null);

            Assert.True(resultList.Count() == 1);
            Assert.True(resultList.First() == testInput);
        }

        [Fact]
        public void ReturnInputIfStateIsConnected()
        {
            var testInput = "TEST";
            var sut = new InputProcessor();

            var resultList = sut.Convert(testInput, "Connected", null, null);

            Assert.True(resultList.Count() == 1);
            Assert.True(resultList.First() == testInput);
        }

        [Fact]
        public void ReturnEmptyArrayIfStateIsWaitingForSymbolAndInputIsNull()
        {
            string testInput = null;
            var sut = new InputProcessor();

            var resultList = sut.Convert(testInput, "WaitingForSymbol", null, null);

            Assert.True(!resultList.Any());
        }
    }
}
