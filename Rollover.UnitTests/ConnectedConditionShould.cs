using Rollover.Ib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rollover.UnitTests
{
    public class ConnectedConditionShould
    {
        [Theory]
        [InlineData("id=-1 errorCode=2104 msg=Market data farm connection is OK:eufarm Exception=")]
        void ReturnTrueIfAllConditionsMet(string input)
        {
            IConnectedCondition sut = new ConnectedCondition();
            sut.AddInput(input);

            var result = sut.IsConnected();

            Assert.True(result);    
        }

        //      id = -1 errorCode = 2104 msg = Market data farm connection is OK:usfarm.nj Exception =

        //Accounts found: U7292073
        //id = -1 errorCode = 2104 msg = Market data farm connection is OK:eufarm Exception =
        //      id = -1 errorCode = 2104 msg = Market data farm connection is OK:usfuture Exception =
        //            id = -1 errorCode = 2104 msg = Market data farm connection is OK:cashfarm Exception =
        //                  id = -1 errorCode = 2104 msg = Market data farm connection is OK:usopt Exception =
        //                        id = -1 errorCode = 2104 msg = Market data farm connection is OK:usfarm Exception =
        //                              id = -1 errorCode = 2106 msg = HMDS data farm connection is OK:euhmds Exception =
        //                                    id = -1 errorCode = 2106 msg = HMDS data farm connection is OK:ushmds Exception =
        //                                          id = -1 errorCode = 2158 msg = Sec - def data farm connection is OK:secdefil Exception =

    }
}
