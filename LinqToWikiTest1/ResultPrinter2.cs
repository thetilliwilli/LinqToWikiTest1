using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToWikiTest1
{
    /// <summary>
    /// Duty - output the result of research in convinient way
    /// </summary>
    internal static class ResultPrinter2
    {
        
        public static void Print<T>(IEnumerable<T> resultSet)
            where T : IPrintable
        {
            resultSet
                .ToList()
                .ForEach(x=>x.Print())
                ;
        }
    }
}
