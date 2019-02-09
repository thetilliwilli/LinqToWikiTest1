using LinqToWiki.Generated;
using Newtonsoft.Json;
using RestSharp;
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
            //query
            //var jsonResult = File.ReadAllText(@"query (3).json");
            //var query = @"https://query.wikidata.org/sparql?format=json&query=SELECT%20%3Fvideo_game%20%3Fvideo_gameLabel%20%3Fpublisher%20%3FpublisherLabel%20%3Fpublication_date%20%3Fplatform%20%3FplatformLabel%20%3Fgenre%20%3FgenreLabel%20WHERE%20%7B%0A%20%20SERVICE%20wikibase%3Alabel%20%7B%20bd%3AserviceParam%20wikibase%3Alanguage%20%22en%22.%20%7D%0A%20%20%3Fvideo_game%20wdt%3AP31%20wd%3AQ7889.%0A%20%20OPTIONAL%20%7B%20%3Fvideo_game%20wdt%3AP123%20%3Fpublisher.%20%7D%0A%20%20OPTIONAL%20%7B%20%3Fvideo_game%20wdt%3AP577%20%3Fpublication_date.%20%7D%0A%20%20OPTIONAL%20%7B%20%3Fvideo_game%20wdt%3AP400%20%3Fplatform.%20%7D%0A%20%20OPTIONAL%20%7B%20%3Fvideo_game%20wdt%3AP136%20%3Fgenre.%20%7D%0A%7D%0ALIMIT%2020";
            var query = "SELECT ?video_game ?video_gameLabel ?publisher ?publisherLabel ?publication_date ?platform ?platformLabel ?genre ?genreLabel WHERE { SERVICE wikibase:label { bd:serviceParam wikibase:language \"en\". } ?video_game wdt:P31 wd:Q7889. OPTIONAL { ?video_game wdt:P123 ?publisher. } OPTIONAL { ?video_game wdt:P577 ?publication_date. } OPTIONAL { ?video_game wdt:P400 ?platform. } OPTIONAL { ?video_game wdt:P136 ?genre. } } LIMIT 20";
            var jsonResult = GetResult(query);
            var gamesDtoResult = JsonConvert.DeserializeObject<ResponseDto>(jsonResult);
            var games = Program.ToGameInfos(gamesDtoResult);

            //processing
            var resultSet = games
                    .Where(g => g.publication_date > DateTimeOffset.Parse("2000-01-01T00:00:00Z"))
                    .Take(15)
                    ;

            //output
            resultSet
                .ToList()
                .ForEach(GameInfo.Print)
                ;
            ReadKey();
        }

        static RestClient _client = PrepareClient();

        static RestClient PrepareClient()
        {
            var client = new RestClient();
            client.BaseUrl = new Uri("https://query.wikidata.org/sparql");
            return client;
        }

        static string GetResult(string query)
        {
            //var uri = Program.ConvertQueryToUrl(query);
            var request = new RestRequest(Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddQueryParameter("format", "json");
            request.AddQueryParameter("query", query);
            //request.
            WriteLine(_client.BuildUri(request));
            var result = _client.Execute(request);
            if (result.ErrorException != null)
                Environment.FailFast("Error while request", result.ErrorException);
            return result.Content;
        }

        static string ConvertQueryToUrl(string query)
        {
            return query;
        }

        static IEnumerable<GameInfo> ToGameInfos(ResponseDto resultDto)
        {
            return resultDto.results.bindings
                .Select(binding => new GameInfo {
                    video_gameLabel = binding[nameof(GameInfo.video_gameLabel)].value,
                    publisherLabel = binding[nameof(GameInfo.publisherLabel)].value,
                    publication_date = DateTimeOffset.Parse(binding[nameof(GameInfo.publication_date)].value),
                    platformLabel = binding[nameof(GameInfo.platformLabel)].value,
                    genreLabel = binding[nameof(GameInfo.genreLabel)].value,
                    //video_game = binding[nameof(GameInfo.video_game)].value,
                    //publisher = binding[nameof(GameInfo.publisher)].value,
                    //platform = binding[nameof(GameInfo.platform)].value,
                    //genre = binding[nameof(GameInfo.genre)].value,
                });
        }
    }

    struct GameInfo
    {
        public string video_gameLabel;
        public string publisherLabel;
        public DateTimeOffset publication_date;
        public string platformLabel;
        public string genreLabel;
        //public string video_game;
        //public string publisher;
        //public string platform;
        //public string genre;

        public override string ToString()
        {
            return $"Game: {video_gameLabel,-40} published {publication_date.ToString("yyyy-MM-dd")}";
        }

        public static void Print(GameInfo gameInfo)
        {
            WriteLine(gameInfo.ToString());
        }
    }


    struct ResponseDto
    {
        public Results results;
    }

    struct Results
    {
        public List<Dictionary<string, ItemDto>> bindings;
    }

    struct ItemDto
    {
        public string type;
        public string value;
    }
}