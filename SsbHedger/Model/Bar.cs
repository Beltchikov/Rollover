using System;

namespace SsbHedger.Model
{
    public record Bar(DateTime Time, double Open, double High, double Low, double Close);
}
