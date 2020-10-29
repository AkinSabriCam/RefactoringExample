using System;

namespace RefactoringExample
{
    public class ComedyCalculator : PerformanceCalculator
    {
        public ComedyCalculator(Invoice.Performance performance)
            : base(performance)
        {
        }

        public override decimal GetAmount()
        {
            var result = 30000;
            if (Performance.Audience > 20)
            {
                result += 10000 + 500 * (Performance.Audience - 20);
            }

            result += 300 * Performance.Audience;
            return result;
        }

        public override decimal GetVolumeCredits()
        {
            return base.GetVolumeCredits() + Math.Floor((decimal) Performance.Audience / 5);
        }
    }
}