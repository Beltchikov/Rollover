using SsbHedger2.Abstractions;

namespace SsbHedger2.RegistryManager
{
    internal class RegistryManagerBuilder : IRegistryManagerBuilder
    {
        public IRegistryManager Build()
        {
            return new RegistryManager(new RegistryCurrentUserAbstraction());
        }
    }
}
