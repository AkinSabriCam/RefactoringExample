using System;

namespace RefactoringExample
{
    public abstract class PerformanceCalculator
    {
        protected Invoice.Performance Performance { get; }

        protected PerformanceCalculator(Invoice.Performance performance)
        {
            Performance = performance;
        }

        public abstract decimal GetAmount();

        public virtual decimal GetVolumeCredits()
        {
            return Math.Max(Performance.Audience - 30, 0);
        }
    }
}