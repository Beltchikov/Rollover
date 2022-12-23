namespace SsbHedger2.UnitTests
{
    using AutoFixture;
    using AutoFixture.AutoNSubstitute;
    using AutoFixture.Xunit2;

    public class AutoNSubstituteDataAttribute : AutoDataAttribute
    {
        public AutoNSubstituteDataAttribute()
            : base(() => new Fixture().Customize(new AutoNSubstituteCustomization { ConfigureMembers = true }))
        {
        }

    }
}
