namespace RefactoringExample
{
    public class TragedyCalculator : PerformanceCalculator
    {
        public TragedyCalculator(Invoice.Performance performance) 
            : base(performance)
        {
        }

        public override decimal GetAmount()
        {
            var thisAmount = 40000;
            if (Performance.Audience > 30)
            {
                thisAmount += 1000 * (Performance.Audience - 30);
            }

            return thisAmount;
        }
    }
}