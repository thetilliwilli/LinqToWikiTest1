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
            var queryStartTime = DateTime.Now;
                var query = "SELECT ?video_game ?video_gameLabel ?publisher ?publisherLabel ?publication_date ?platform ?platformLabel ?genre ?genreLabel WHERE { SERVICE wikibase:label { bd:serviceParam wikibase:language \"en\". } ?video_game wdt:P31 wd:Q7889. OPTIONAL { ?video_game wdt:P123 ?publisher. } OPTIONAL { ?video_game wdt:P577 ?publication_date. } OPTIONAL { ?video_game wdt:P400 ?platform. } OPTIONAL { ?video_game wdt:P136 ?genre. } }"
                        + " LIMIT 9999"
                        ;
                var jsonResult = TryGetFromCache(query, ()=> GetResult(query));
            var queryEndTime = DateTime.Now;

            var preparingStartTime = DateTime.Now;
                var gamesDtoResult = JsonConvert.DeserializeObject<ResponseDto>(jsonResult);
                var games = Program.ToGameInfos(gamesDtoResult);
            var preparingEndTime = DateTime.Now;


            //processing
            var processingStartTime = DateTime.Now;
                var resultSet = games
                        .Where(g => g.publication_date > DateTimeOffset.Parse("2000-01-01T00:00:00Z") && g.publication_date < DateTimeOffset.Parse("2010-01-01T00:00:00Z"))
                        .GroupBy(g => g.video_game)
                        .Select(group => group.First())
                        .Take(15)
                        ;
            var processingEndTime = DateTime.Now;

            //output
            resultSet
                .ToList()
                .ForEach(GameInfo.Print)
                ;

            WriteLine($"Timing: [Query: {(queryEndTime - queryStartTime).TotalMilliseconds}] [Porcessing: {(processingEndTime-processingStartTime).TotalMilliseconds}]");
            ReadKey();
        }

        private static string TryGetFromCache(string query, Func<string> retrieveRemoteContent)
        {
            var contentByteArray = System.Text.Encoding.Unicode.GetBytes(query);
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var cacheKey = sha256.ComputeHash(contentByteArray);
                //var cacheKeyBase64 = System.Convert.ToBase64String(cacheKey);
                //var cacheKeyBase64 = System.Text.Encoding.ASCII.GetString(cacheKey);
                //2.ToString()
                var cacheKeyBase64 = new string(cacheKey.Select(@byte => @byte.ToString("X")[0]).ToArray());
                if (File.Exists(cacheKeyBase64))
                    return File.ReadAllText(cacheKeyBase64);

                var newContent = retrieveRemoteContent();
                File.WriteAllText(cacheKeyBase64, newContent);
                return newContent;
            }
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
            //WriteLine(_client.BuildUri(request));
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
                    video_game = binding[nameof(GameInfo.video_game)].value,
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
        public string video_game;
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