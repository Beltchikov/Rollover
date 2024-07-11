namespace SignalAdvisor.Commands
{
    class DeactivateAlert
    {
        public static void Run(IPositionsVisitor visitor)
        {
            visitor.AlertDeactivated = true;
        }
    }
}
