using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToWikiTest1
{
    /// <summary>
    /// Duty - process data according to demand
    /// </summary>
    internal class Processor
    {
        public IEnumerable<GameInfo> Process(IEnumerable<GameInfo> games)
        {
            var resultSet = games
                    .Where(g => g.publication_date > DateTimeOffset.Parse("2000-01-01T00:00:00Z") && g.publication_date < DateTimeOffset.Parse("2010-01-01T00:00:00Z"))
                    .GroupBy(g => g.video_game)
                    .Select(group => group.First())
                    .OrderBy(x=>x.publication_date)
                    //.Take(15)
                    ;
            return resultSet;
        }

        public int EntityTotalCount(IEnumerable<GameInfo> games)
        {
            var result = games
                    .Count()
                    ;
            return result;
        }

        public int GroupedEntityTotalCount(IEnumerable<GameInfo> games)
        {
            var result = games
                    //.Where(g => g.publication_date > DateTimeOffset.Parse("2000-01-01T00:00:00Z") && g.publication_date < DateTimeOffset.Parse("2010-01-01T00:00:00Z"))
                    .GroupBy(g => g.video_game)
                    .Count()
                    //.Select(group => group.First())
                    //.OrderBy(x => x.publication_date)
                    //.Take(15)
                    ;
            return result;
        }

        public IEnumerable<GameInfo> FindPostalGame(IEnumerable<GameInfo> games)
        {
            var result =
                from game in games
                where game.video_gameLabel.ToLower().Contains("pos")
                //group game.
                select game
                ;

            return result;
        }
    }
}
