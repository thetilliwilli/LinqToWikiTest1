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


            //query
            var queryStartTime = DateTime.Now;
                var query = "SELECT ?video_game ?video_gameLabel ?publisher ?publisherLabel ?publication_date ?platform ?platformLabel ?genre ?genreLabel WHERE { SERVICE wikibase:label { bd:serviceParam wikibase:language \"en\". } ?video_game wdt:P31 wd:Q7889. OPTIONAL { ?video_game wdt:P123 ?publisher. } OPTIONAL { ?video_game wdt:P577 ?publication_date. } OPTIONAL { ?video_game wdt:P400 ?platform. } OPTIONAL { ?video_game wdt:P136 ?genre. } }"
                        + " LIMIT 9999"
                        ;
                var jsonResult = TryGetFromCache(query, ()=> GetResult(query));
            var queryEndTime = DateTime.Now;

            //JsConfig.IncludeNullValues = true;
            var preparingStartTime = DateTime.Now;
            var gamesDtoResult = JsonConvert.DeserializeObject<ResponseDto>(jsonResult);
            //var gamesDtoResult = ServiceStack.Text.JsonSerializer.DeserializeFromString<ResponseDto>(jsonResult);
                var games = gamesDtoResult.results.bindings.Select(b => b.ToGameInfo());
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

            WriteLine($"Timing: [Query: {(queryEndTime - queryStartTime).TotalMilliseconds:F2}ms] [Preparing: {(preparingEndTime - preparingStartTime).TotalMilliseconds:F2}ms] [Porcessing: {(processingEndTime-processingStartTime).TotalMilliseconds:F2}ms]");
            ReadKey();
        }

        private static string TryGetFromCache(string query, Func<string> retrieveRemoteContent)
        {
            var contentByteArray = System.Text.Encoding.Unicode.GetBytes(query);
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var cacheKey = sha256.ComputeHash(contentByteArray);
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
            var request = new RestRequest(Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddQueryParameter("format", "json");
            request.AddQueryParameter("query", query);
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
                .Select(binding => binding.ToGameInfo());
        }
    }

    struct GameInfo
    {
        public string video_gameLabel;
        public string publisherLabel;
        public DateTimeOffset? publication_date;
        public string platformLabel;
        public string genreLabel;
        public string video_game;
        //public string publisher;
        //public string platform;
        //public string genre;

        public override string ToString()
        {
            return $"Game: {video_gameLabel,-40} published {publication_date?.ToString("yyyy-MM-dd")}";
        }

        public static void Print(GameInfo gameInfo)
        {
            WriteLine(gameInfo.ToString());
        }
    }


    class ResponseDto
    {
        public Results results;
    }

    class Results
    {
        public List<GameInfoDto> bindings;
    }

    class GameInfoDto
    {
        public ItemDto video_gameLabel;
        public ItemDto publisherLabel;
        public ItemDto publication_date;
        public ItemDto platformLabel;
        public ItemDto genreLabel;
        public ItemDto video_game;
        //public ItemDto publisher;
        //public ItemDto platform;
        //public ItemDto genre;

        public static void Print(GameInfo gameInfo)
        {
            WriteLine(gameInfo.ToString());
        }
    }

    static class DtoExtensions
    {
        public static GameInfo ToGameInfo(this GameInfoDto gameInfoDto) => new GameInfo
        {
            video_gameLabel = gameInfoDto.video_gameLabel?.value,
            publisherLabel = gameInfoDto.publisherLabel?.value,
            publication_date = gameInfoDto.publication_date?.value == null
                ? (DateTimeOffset?)null
                : DateTimeOffset.Parse(gameInfoDto.publication_date.value),
            platformLabel = gameInfoDto.platformLabel?.value,
            genreLabel = gameInfoDto.genreLabel?.value,
            video_game = gameInfoDto.video_game?.value,
            //publisher = binding[nameof(GameInfo.publisher)].value,
            //platform = binding[nameof(GameInfo.platform)].value,
            //genre = binding[nameof(GameInfo.genre)].value,
        };
    }

    class ItemDto
    {
        public string type;
        public string value;
    }
}