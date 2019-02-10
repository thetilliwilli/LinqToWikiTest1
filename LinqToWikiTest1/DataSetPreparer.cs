using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToWikiTest1
{
    /// <summary>
    /// Duty - prepare data for ready-to-use form for consumer (processor)
    /// </summary>
    internal class DataSetPreparer
    {
        public IEnumerable<GameInfo> Prepare(string jsonResult)
        {
            var gamesDtoResult = JsonConvert.DeserializeObject<ResponseDto>(jsonResult);
            var games = gamesDtoResult.results.bindings.Select(b => b.ToGameInfo());
            return games;
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

    internal class GameInfoDto
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
    }

    internal class ItemDto
    {
        public string type;
        public string value;
    }

    internal static class DtoExtensions
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

    internal class GameInfo : IPrintable
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
            return $"Game: {video_gameLabel.Substring(0, Math.Min(video_gameLabel.Length, 40)),-40} published {publication_date?.ToString("yyyy-MM-dd")}";
        }

        public void Print()
        {
            Console.WriteLine(this.ToString());
        }
    }
}
