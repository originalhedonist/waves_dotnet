using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace wavegenerator
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum NamingConvention
    {
        RandomFemaleName = 1, RandomMaleName, RandomAnyName
    }
}
