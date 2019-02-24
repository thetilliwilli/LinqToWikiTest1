using LinqToWikiTest1.Domain;
using Newtonsoft.Json;

namespace LinqToWikiTest1
{
    public class WdtResponseParser
    {
        public WdtResponseDto ParseResponse(string response)
        {
            return JsonConvert.DeserializeObject<WdtResponseDto>(response);
        }
    }
}
