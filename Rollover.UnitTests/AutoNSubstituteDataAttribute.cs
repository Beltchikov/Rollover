using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rollover.UnitTests
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
