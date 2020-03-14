using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace wavegenerator.library
{
    public class BaseClassPropertiesFirstContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var props = base.CreateProperties(type, memberSerialization);
            var declaringTypes = props.Select(p => p.DeclaringType).Distinct().ToArray();
            var declaringTypesBaseClasses = declaringTypes.ToDictionary(t => t,
                t => declaringTypes.Count(t1 => t1 != t && t1.IsAssignableFrom(t)));
            var sortedProps = props.OrderBy(p => declaringTypesBaseClasses[p.DeclaringType]).ThenBy(p => props.IndexOf(p)).ToList();
            return sortedProps;
        }
    }
}
