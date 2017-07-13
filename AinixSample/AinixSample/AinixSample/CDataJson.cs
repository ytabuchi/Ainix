using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AinixSample
{
    public class Value
    {
        public string Lot { get; set; }
        public string StockDate { get; set; }
        public string Name { get; set; }
        public string Product { get; set; }
    }

    public class CDataJson
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string Context { get; set; }
        [JsonProperty(PropertyName = "value")]
        public List<Value> Values { get; set; }
    }
}
