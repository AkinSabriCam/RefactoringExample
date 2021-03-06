﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace RefactoringExample
{
    class Program
    {
        private static Plays Plays { get; set; }
        private static List<Invoice> Invoices { get; set; }

        static void Main(string[] args)
        {
            SetPlaysAndInvoicesProps(args);
            var performances = GetPerformancesByCustomer("BigCo");
            var enrichedPerformances = EnrichPerformances(performances);

            var resultStatement = new ResultForStatement
            {
                Customer = "BigCo",
                Performances = enrichedPerformances,
                TotalAmount = CalculateTotalAmount(enrichedPerformances),
                TotalVolumeCredits = CalculateTotalVolumeCredits(enrichedPerformances),
            };

            var result = $"Statement for {resultStatement.Customer} \n";
            foreach (var perf in resultStatement.Performances)
            {
                result += $"{perf.Play.Name}: ${perf.Amount}  {perf.Audience} seats \n";
            }

            result += $"Amount owned is ${resultStatement.TotalAmount} \n";
            result += $"You earned {resultStatement.TotalVolumeCredits} credits \n";

            Console.WriteLine(result);
        }

        private static void SetPlaysAndInvoicesProps(string[] args)
        {
            var data = GetDataFromJson(args);
            Plays = data.Plays;
            Invoices = data.Invoices;
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
                var performanceCalculator = CreatePerformanceCalculator(perf, play);
                results.Add(new ResultForPerf
                {
                    Play = play,
                    Amount =performanceCalculator.GetAmount(),
                    VolumeCredits = performanceCalculator.GetVolumeCredits(),
                    Audience = perf.Audience
                });
            }

            return results;
        }

        static Plays.Play GetPlay(string playId)
        {
            return Plays.GetPlay(playId);
        }
        
        static PerformanceCalculator CreatePerformanceCalculator(Invoice.Performance performance, Plays.Play play)
        {
            return play.Type switch
            {
                "tragedy" => new TragedyCalculator(performance),
                "comedy" => new ComedyCalculator(performance),
                _ => throw new Exception($"Unknown type: {play.Type}")
            };
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