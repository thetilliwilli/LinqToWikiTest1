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
    internal class ResultPrinter<T> where T : IPrintable
    {
        
        public void Print(IEnumerable<T> resultSet)
        {
            resultSet
                .ToList()
                .ForEach(x=>x.Print())
                ;
        }
    }
}
