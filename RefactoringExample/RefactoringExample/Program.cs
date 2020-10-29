using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace ExamOnRefacttoring
{
    class Program
    {
        private static Plays Plays { get; set; }
        private static List<Invoice> Invoices { get; set; }

        static void Main(string[] args)
        {
            var data = GetDataFromJson(args);
            Plays = data.Plays;
            Invoices = data.Invoices;
            var performances = GetPerformancesByCustomer("BigCo");
            var enrichedPerformances = EnrichPerformances(performances);
            
            var resultStatement = new ResultForStatement
            {
                Customer =  "BigCo",
                Performances = enrichedPerformances,
                TotalAmount = CalculateTotalAmount(enrichedPerformances),
                TotalVolumeCredits = CalculateTotalVolumeCredits(enrichedPerformances),
            };

            var result = $"Statement for {resultStatement.Customer} \n";
            foreach(var perf in resultStatement.Performances)
            {
                result += $"{perf.Play.Name}: ${perf.Amount}  {perf.Audience} seats \n";
            }

            result += $"Amount owned is ${resultStatement.TotalAmount} \n";
            result += $"You earned ${resultStatement.TotalVolumeCredits} credits \n";

            Console.WriteLine(result);
        }

        static decimal CalculateTotalAmount(IEnumerable<ResultForPerf> performances)
        {
            return performances.Sum(x => x.Amount);
        }

        static decimal CalculateTotalVolumeCredits(IEnumerable<ResultForPerf> performances)
        {
            return performances.Sum(x => x.VolumeCredits);
        }

        static List<Invoice.Performance> GetPerformancesByCustomer(string customer)
        {
            return Invoices.FirstOrDefault(x => x.Customer == customer)?.Performances;
        }

        static List<ResultForPerf> EnrichPerformances(List<Invoice.Performance> performances)
        {
            var results = new List<ResultForPerf>();
            foreach (var perf in performances)
            {
                var play = GetPlay(perf.PlayId);
                results.Add(new ResultForPerf
                {
                    Play = play,
                    Amount = CalculateAmount(perf, play),
                    VolumeCredits = CalculateVolumeCredits(perf, play),
                    Audience = perf.Audience
                });
            }

            return results;
        }

        static decimal CalculateVolumeCredits(Invoice.Performance perf, Plays.Play play)
        {
            // add volume credits
            var volumeCredits = Convert.ToDecimal(Math.Max(perf.Audience - 30, 0).ToString());
            // add extra credit for every ten comedy attendees
            if ("comedy" == play.Type)
                volumeCredits += Math.Floor((decimal) (perf.Audience / 5));
            return volumeCredits;
        }

        static int CalculateAmount(Invoice.Performance perf, Plays.Play play)
        {
            var thisAmount = 0;

            switch (play.Type)
            {
                case "tragedy":
                    thisAmount = 40000;
                    if (perf.Audience > 30)
                    {
                        thisAmount += 1000 * (perf.Audience - 30);
                    }

                    break;
                case "comedy":
                    thisAmount = 30000;
                    if (perf.Audience > 20)
                    {
                        thisAmount += 1000 + 500 * (perf.Audience - 20);
                    }

                    thisAmount += 300 * perf.Audience;
                    break;
                default:
                    throw new Exception($"unkown type {play.Type}");
            }

            return thisAmount;
        }

        static Plays.Play GetPlay(string playId)
        {
            return Plays.GetPlay(playId);
        }
        
        
        class ResultForPerf
        {
            public Plays.Play Play { get; set; }
            public decimal Amount { get; set; }
            public decimal VolumeCredits { get; set; }
            
            public int Audience { get; set; }
        }

        class ResultForStatement
        {
            public string Customer { get; set; }
            public List<ResultForPerf> Performances { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal TotalVolumeCredits { get; set; }
        }
        
        static Data GetDataFromJson(string[] args)
        {
            var configuration = ReadDataFromJson(args);
            var data = new Data();
            configuration.GetSection(Data.MainSectionName).Bind(data);
            return data;
        }

        static IConfiguration ReadDataFromJson(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("PlaysAndInvoices.json", true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            return configuration;
        }
    }
}