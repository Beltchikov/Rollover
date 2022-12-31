namespace SsbHedger.Configuration
{
    public interface IConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public int ClientId { get; set; }
    }
}