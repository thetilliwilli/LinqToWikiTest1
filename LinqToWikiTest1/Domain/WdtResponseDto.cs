using Newtonsoft.Json;
using System.Collections.Generic;

namespace LinqToWikiTest1.Domain
{
    /// <summary>
    /// WikidataDataSet dto
    /// </summary>
    public class WdtResponseDto
    {
        public WdtHeadDto Head;
        public WdtResultsDto Results;

    }

    public class WdtHeadDto
    {
        public List<string> Vars;
    }

    public class WdtResultsDto
    {
        public List<WdtBindingItemDto> Bindings;
    }

    public class WdtBindingItemDto : Dictionary<string, WdtBindingItemPropertyDto>
    {
    }

    public class WdtBindingItemPropertyDto
    {
        public string Type;
        public string Value;
        public string Datatype;
        [JsonProperty("xml:lang")]
        public string XmlLang;
    }
}
