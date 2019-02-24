using LinqToWikiTest1.Domain;
using System.Collections.Generic;

namespace LinqToWikiTest1.Interface
{
    public interface IDataSetPreparer<TOutFormat>
    {
        IEnumerable<TOutFormat> Prepare(string jsonResponse);
    }
}
