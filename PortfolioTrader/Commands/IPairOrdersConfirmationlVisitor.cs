using IbClient.IbHost;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalAdvisor.Commands
{
    interface IPairOrdersConfirmationlVisitor : ITwsVisitor
    {
        string PairOrdersAsString { get; set; }
        public int InvestmentAmount { get; set; }
        bool PositionsCalculated { get; set; }
        string StocksWithoutPrice { get; set; }
        string StocksWithoutMargin { get; set; }
        string OrdersLongWithError { get; set; }
        string OrdersShortWithError { get; set; }

        public void CalculateWeights();
        void ClearQueueOrderOpenMessage();
    }
}
