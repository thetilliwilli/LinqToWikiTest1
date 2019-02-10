using LinqToWiki.Generated;
using Newtonsoft.Json;
using RestSharp;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace LinqToWikiTest1
{
    class Program
    {
        static void Main(string[] args)
        {
            new App().Go(new AppEnviroment()).ConfigureAwait(false).GetAwaiter().GetResult();

            //init services
            var metricsCollector = new MetricsCollector();
            var dataFetcher = new DataFetcher();
            var dataSetPreparer = new DataSetPreparer();
            var processor = new Processor();
            var resultPrinter = new ResultPrinter<GameInfo>();

            //query
            var jsonResult = metricsCollector.LogAndInvoke(dataFetcher.Fetch, "query");

            //preparing
            var games = metricsCollector.LogAndInvoke(()=>dataSetPreparer.Prepare(jsonResult), "preparing");

            //processing
            var resultSet = metricsCollector.LogAndInvoke(()=>processor.Process(games), "process");

            //output
            metricsCollector.LogAndInvoke(() => resultPrinter.Print(resultSet), "printer");

            WriteLine("\nTiming");
            WriteLine(string.Join(", ", metricsCollector.Select(x=>$"[{x}]")));
            ReadKey();
        }
    }
}