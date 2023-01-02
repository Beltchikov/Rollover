namespace SsbHedger.Model
{
    public interface IConfiguration
    {
        object GetValue(string name);
        void SetValue(string name, object value);
    }
}