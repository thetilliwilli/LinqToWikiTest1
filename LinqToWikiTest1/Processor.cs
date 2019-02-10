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
    }
}
