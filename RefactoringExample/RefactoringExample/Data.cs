using System.Collections.Generic;

namespace RefactoringExample
{
    public class Data
    {
        public const string MainSectionName = "data";
        public Plays Plays { get; set; }

        public List<Invoice> Invoices { get; set; }
    }

    public class Invoice
    {
        public string Customer { get; set; }
        public List<Performance> Performances { get; set; }

        public class Performance
        {
            public string PlayId { get; set; }
            public int Audience { get; set; }
        }
    }

    public class Plays
    {
        public Play Hamlet { get; set; }
        public Play Aslike { get; set; }
        public Play Othello { get; set; }

        public class Play
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }

        public Play GetPlay(string perfPlayId)
        {
            return perfPlayId.ToLower() switch
            {
                "hamlet" => Hamlet,
                "as-like" => Aslike,
                "othello" => Othello,
                _ => null
            };
        }
    }
}