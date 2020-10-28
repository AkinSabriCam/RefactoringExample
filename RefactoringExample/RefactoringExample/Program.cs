using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace ExamOnRefacttoring
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = GetDataFromJson(args);
            var plays = data.Plays;
            var invoices = data.Invoices;
            var performances = invoices.FirstOrDefault()?.Performances;

            var totalAmount = 0m;
            var volumeCredits = 0m;
            var result = $"Statement for {invoices.FirstOrDefault()?.Customer} \n";

            foreach (var perf in performances)
            {
                var play = plays.GetPlay(perf.PlayId);
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

                // add volume credits
                volumeCredits += Convert.ToDecimal(Math.Max(perf.Audience - 30, 0).ToString());
                // add extra credit for every ten comedy attendees
                if ("comedy" == play.Type)
                    volumeCredits += Math.Floor((decimal) (perf.Audience / 5));


                // print line for this order
                result += $"{play.Name}: {thisAmount} {perf.Audience} seats \n";
                totalAmount += thisAmount;
            }

            result += $"Amount owned is {totalAmount} \n";
            result += $"You earned {volumeCredits} credits \n";

            Console.WriteLine("Hello World!");
            Console.WriteLine(result);
        }

        private static AppSettings GetDataFromJson(string[] args)
        {
            var configuration = ReadDataFromJson(args);
            var appSettings = new AppSettings();
            configuration.GetSection(AppSettings.Settings).Bind(appSettings);
            return appSettings;
        }

        private static IConfiguration ReadDataFromJson(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            return configuration;
        }
    }
}