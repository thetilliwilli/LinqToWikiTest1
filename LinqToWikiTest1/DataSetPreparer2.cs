using LinqToWikiTest1.Domain;
using LinqToWikiTest1.Interface;
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
    internal class DataSetPreparer2<TOutFormat> : IDataSetPreparer<TOutFormat>
        where TOutFormat : class, new()
    {
        private WdtResponseParser wdtResponseParser;

        public DataSetPreparer2(WdtResponseParser wdtResponseParser)
        {
            this.wdtResponseParser = wdtResponseParser;
        }

        public IEnumerable<TOutFormat> Prepare(string jsonResponse)
        {
            var wdtResponseDto = wdtResponseParser.ParseResponse(jsonResponse);
            var entities = DataSetFactory(wdtResponseDto);
            return entities;
        }

        private IEnumerable<TOutFormat> DataSetFactory(WdtResponseDto wdtResponseDto)
        {
            foreach (var bindingItem in wdtResponseDto.Results.Bindings)
                yield return ToDataSetEntity(bindingItem);
            //return wdtResponseDto.Results.Bindings.Select(ToDataSetEntity);
        }

        private TOutFormat ToDataSetEntity(WdtBindingItemDto bindingItem)
        {
            var result = new TOutFormat();
            foreach(var property in typeof(TOutFormat).GetFields())
            {
                var x = property.GetCustomAttributesData()
                    .Where(t=>t.AttributeType.Equals(typeof(JsonPropertyAttribute)))
                    .SingleOrDefault()?.ConstructorArguments[0].Value.ToString();
                if(bindingItem.TryGetValue(x, out var propertyValue))
                    property.SetValue(result, propertyValue.Value);
                //var propertyValue = bindingItem[property.Name].Value;
            }
            return result;
        }
    }
}
