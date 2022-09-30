using IbClient;

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
    string msgError = $"OnError: id={id} errorCode={errorCode} msg={msg} exception={ex}";
    Console.WriteLine(msgError);
}

void _ibClient_NextValidId(IbClient.messages.ConnectionStatusMessage statusMessage)
{
    string msgNextValidId = statusMessage.IsConnected
        ? $"Connected to TWS! HOST: {HOST} PORT: {PORT} CLIENT_ID: {CLIENT_ID} Next Order ID: {_ibClient.NextOrderId}"
        : $"Error while connecting to TWS! HOST: {HOST} PORT: {PORT} CLIENT_ID: {CLIENT_ID}";
    Console.WriteLine(msgNextValidId);
}

Console.WriteLine("SsbOrderSender: press Q to quit");
while (Console.ReadKey().KeyChar.ToString().ToUpper() != "Q") { };



