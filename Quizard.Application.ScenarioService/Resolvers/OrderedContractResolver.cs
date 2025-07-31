using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Quizard.Application.ScenarioService.Resolvers;

internal class OrderedContractResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        return base.CreateProperties(type, memberSerialization)
            .OrderBy(p => p.PropertyName)
            .ToList();
    }
}