using RestSharp;
using System;
using System.IO;
using System.Linq;

namespace LinqToWikiTest1
{
    /// <summary>
    /// Ответственность - вернуть актуальные данные с сервера или закэшированные
    /// </summary>
    internal class DataFetcher
    {
        public string Fetch()
        {
            var query = "SELECT ?video_game ?video_gameLabel ?publisher ?publisherLabel ?publication_date ?platform ?platformLabel ?genre ?genreLabel WHERE { SERVICE wikibase:label { bd:serviceParam wikibase:language \"en\". } ?video_game wdt:P31 wd:Q7889. OPTIONAL { ?video_game wdt:P123 ?publisher. } OPTIONAL { ?video_game wdt:P577 ?publication_date. } OPTIONAL { ?video_game wdt:P400 ?platform. } OPTIONAL { ?video_game wdt:P136 ?genre. } }"
                    + " LIMIT 9999"
                    ;
            var jsonResult = TryGetFromCache(query, () => GetResult(query));
            return jsonResult;
        }

        string TryGetFromCache(string query, Func<string> retrieveRemoteContent)
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


        string GetResult(string query)
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


        static RestClient _client = PrepareClient();

        static RestClient PrepareClient()
        {
            var client = new RestClient();
            client.BaseUrl = new Uri("https://query.wikidata.org/sparql");
            return client;
        }
    }
}
