using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PassageIdentity;

public class CustomJsonStringEnumConverter : JsonConverterFactory
{
    private readonly bool _allowIntegerValues;
    private readonly JsonStringEnumConverter _baseConverter;
    private readonly JsonNamingPolicy? _namingPolicy;

    public CustomJsonStringEnumConverter() : this(null, true)
    {
    }

    public CustomJsonStringEnumConverter(JsonNamingPolicy? namingPolicy = null, bool allowIntegerValues = true)
    {
        _namingPolicy = namingPolicy;
        _allowIntegerValues = allowIntegerValues;
        _baseConverter = new JsonStringEnumConverter(namingPolicy, allowIntegerValues);
    }

    public override bool CanConvert(Type typeToConvert) => _baseConverter.CanConvert(typeToConvert);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert is null)
        {
            throw new ArgumentNullException(nameof(typeToConvert));
        }

        var query = from field in typeToConvert.GetFields(BindingFlags.Public | BindingFlags.Static)
                    let attr = field.GetCustomAttribute<EnumMemberAttribute>()
                    where attr != null
                    select (field.Name, attr.Value);
        var dictionary = query.ToDictionary(p => p.Name, p => p.Value);
        if (dictionary.Count > 0)
        {
            return new JsonStringEnumConverter(new DictionaryLookupNamingPolicy(dictionary, _namingPolicy ?? new SnakeCaseNamingPolicy()), _allowIntegerValues).CreateConverter(typeToConvert, options);
        }
        else
        {
            return _baseConverter.CreateConverter(typeToConvert, options);
        }
    }
}

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public static SnakeCaseNamingPolicy Instance { get; } = new SnakeCaseNamingPolicy();

    public override string ConvertName(string name)
    {
        // Conversion to other naming convention goes here. Like SnakeCase, KebabCase etc.
        return name.ToSnakeCase();
    }
}

internal sealed class DictionaryLookupNamingPolicy : JsonNamingPolicyDecorator
{
    private readonly Dictionary<string, string> _dictionary;

    public DictionaryLookupNamingPolicy(Dictionary<string, string> dictionary, JsonNamingPolicy underlyingNamingPolicy) : base(underlyingNamingPolicy) => this._dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

    public override string ConvertName(string name) => _dictionary.TryGetValue(name, out var value) ? value : base.ConvertName(name);
}

internal class JsonNamingPolicyDecorator : JsonNamingPolicy
{
    private readonly JsonNamingPolicy _underlyingNamingPolicy;

    public JsonNamingPolicyDecorator(JsonNamingPolicy underlyingNamingPolicy) => this._underlyingNamingPolicy = underlyingNamingPolicy;

    public override string ConvertName(string name) => _underlyingNamingPolicy == null ? name : _underlyingNamingPolicy.ConvertName(name);
}
