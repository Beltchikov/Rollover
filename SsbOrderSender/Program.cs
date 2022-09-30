using IbClient;
using System.Diagnostics;

const string LOG_FILE = "SsbOrderSender.log";
const string HOST = "localhost";
const int PORT = 4001;
const int CLIENT_ID = 1;
IBClient _ibClient;

IBClient.CreateSignal();
_ibClient = IBClient.CreateClient();

_ibClient.Error -= _ibClient_Error;
_ibClient.Error += _ibClient_Error;

_ibClient.NextValidId -= _ibClient_NextValidId; 
_ibClient.NextValidId += _ibClient_NextValidId;

_ibClient.ConnectAndStartReaderThread(HOST, PORT, CLIENT_ID);

void _ibClient_Error(int id, int errorCode, string msg, Exception ex)
{
    string msgError = $"OnError: id={id} errorCode={errorCode} msg={msg} exception={ex}{Environment.NewLine}";
    WithRetry(() => File.AppendAllText(LOG_FILE, msgError), 10, 10);
}

void _ibClient_NextValidId(IbClient.messages.ConnectionStatusMessage statusMessage)
{
    string msgNextValidId = statusMessage.IsConnected
        ? $"Connected to TWS! HOST: {HOST} PORT: {PORT} CLIENT_ID: {CLIENT_ID} Next Order ID: {_ibClient.NextOrderId}"
        : $"Error while connecting to TWS! HOST: {HOST} PORT: {PORT} CLIENT_ID: {CLIENT_ID}";
    Console.WriteLine(msgNextValidId);
}

while (Console.ReadKey().KeyChar.ToString().ToUpper() != "Q") { };

void WithRetry(Action action, int timeout = 1000, int maxAttempts = 10)
{
    int attempt = 1;
    var time = Stopwatch.StartNew();
    while (time.ElapsedMilliseconds < timeout)
    {
        try
        {
            action();
            return;
        }
        catch (Exception ex)
        {
            attempt++;
            if(attempt == maxAttempts)
            {
                throw new Exception("Stopped retrying after {maxAttempts} attempts", ex);
            }
        }
    }
}


