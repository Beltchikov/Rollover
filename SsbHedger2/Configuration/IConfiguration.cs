namespace SsbHedger2.Configuration
{
    public interface IConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public int ClientId { get; set; }
    }
}