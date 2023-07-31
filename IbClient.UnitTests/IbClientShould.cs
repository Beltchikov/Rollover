using System.Runtime.Serialization;

namespace IbClient.UnitTests
{
    public class IbClientShould
    {
        [Fact]
        public void CallClientSocketEConnect()
        {
            var sut = FormatterServices.GetUninitializedObject(typeof(IBClient));
            //throw new NotImplementedException();
            // TODO
        }
    }
}