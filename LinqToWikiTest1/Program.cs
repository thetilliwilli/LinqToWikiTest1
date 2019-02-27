using LinqToWiki.Generated;
using LinqToWikiTest1.Domain;
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
        static void Main2(string[] args)
        {
            //MyClass.NotMain();return;
            //new App().Go(new AppEnviroment()).ConfigureAwait(false).GetAwaiter().GetResult();

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
            Console.WriteLine(processor.EntityTotalCount(games));
            Console.WriteLine(processor.GroupedEntityTotalCount(games));

            //output
            metricsCollector.LogAndInvoke(() => resultPrinter.Print(resultSet), "printer");

            WriteLine("\nTiming");
            WriteLine(string.Join(", ", (metricsCollector as IEnumerable<string>).Select(x=>$"[{x}]")));
            ReadKey();
        }


        static void Main(string[] args)
        {
            //init services
            var metricsCollector = new MetricsCollector();
            var dataFetcher = new DataFetcher2();
            var dataSetPreparer = new DataSetPreparer2<GameInfo2>(new WdtResponseParser());
            //var processor = new Processor();
            var resultPrinter = new ResultPrinter<GameInfo2>();

            //query
            var query = new SparqlQuery("SELECT ?game ?gameLabel ?platform ?platformLabel ?developer ?developerLabel ?publisher ?publisherLabel ?genre ?genreLabel ?game_mode ?game_modeLabel ?official_website ?official_websiteLabel ?publication_date ?publication_dateLabel ?part_of_the_series ?part_of_the_seriesLabel ?software_engine ?software_engineLabel ?pegi_rating ?pegi_ratingLabel ?review_score ?review_scoreLabel ?title ?titleLabel ?country_of_origin ?country_of_originLabel WHERE { SERVICE wikibase:label { bd:serviceParam wikibase:language \"en\". } ?game wdt:P31 wd:Q7889. OPTIONAL { ?game wdt:P400 ?platform. } OPTIONAL { ?game wdt:P178 ?developer. } OPTIONAL { ?game wdt:P123 ?publisher. } OPTIONAL { ?game wdt:P136 ?genre. } OPTIONAL { ?game wdt:P404 ?game_mode. } OPTIONAL { ?game wdt:P856 ?official_website. } OPTIONAL { ?game wdt:P577 ?publication_date. } OPTIONAL { ?game wdt:P179 ?part_of_the_series. } OPTIONAL { ?game wdt:P408 ?software_engine. } OPTIONAL { ?game wdt:P908 ?pegi_rating. } OPTIONAL { ?game wdt:P444 ?review_score. } OPTIONAL { ?game wdt:P1476 ?title. } OPTIONAL { ?game wdt:P495 ?country_of_origin. } }");

            var jsonResult = metricsCollector.LogAndInvoke(()=> dataFetcher.Fetch(query), "query");

            //preparing
            var games = metricsCollector.LogAndInvoke(() => dataSetPreparer.Prepare(jsonResult), "preparing");

            //processing
            //Func<IEnumerable<GameInfo2>, IEnumerable<GameInfo2>> FindSinglePlayerGames = gs => gs.Where(g=>g.GameMode?.Contains("single")==true);
            //var resultSet = metricsCollector.LogAndInvoke(()=> FindSinglePlayerGames(games.Take(1000)), "FindSinglePlayerGames");

            Func<IEnumerable<GameInfo2>, int> FindTotalCount = gs => gs.Count();
            var resultCount = metricsCollector.LogAndInvoke(()=>FindTotalCount(games), "FindTotalCount");

            //output
            //metricsCollector.LogAndInvoke(() => ResultPrinter2.Print<GameInfo2>(resultSet), "ResultPrinter2.Print<GameInfo2>");

            WriteLine("\nTiming");
            WriteLine(string.Join(", ", (metricsCollector as IEnumerable<string>).Select(x => $"[{x}]")));
            ReadKey();
        }
    }
}