using System.Threading;
using System.Windows;

namespace NpvManager.Commands
{
    class AnalystsPrices
    {
        public static async Task RunAsync(IPositionsVisitor visitor)
        {
            MessageBox.Show("AnalystsPrices!");

            //string fundamentalData = await _ibHost.RequestFundamentalDataAsync(contract, reportType, timeout);

            //public async Task<List<DataStringWithTicker>> FundamentalDataFromContractStrings(List<string> contractStringsTws, string reportType, int timeout)
            //{
            //    var result = new List<DataStringWithTicker>();

            //    int cnt = 1;
            //    foreach (string contractString in contractStringsTws)
            //    {
            //        if (string.IsNullOrWhiteSpace(contractString))
            //        {
            //            continue;
            //        }

            //        var contractStringTrimmed = contractString.Trim();
            //        Contract contract = ContractFromString(contractStringTrimmed);
            //        TriggerStatus($"Retrieving fundamental data for {contractStringTrimmed}, report type: {reportType} {cnt++}/{contractStringsTws.Count}");
            //        string fundamentalDataString = await FundamentalDataFromContract(timeout, reportType, contract);
            //        result.Add(new DataStringWithTicker(contract.Symbol, fundamentalDataString));
            //    }

            //    return result;
            //}
        }
    }
}
