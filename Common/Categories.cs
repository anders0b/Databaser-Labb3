using System.Text.Json.Serialization;

namespace Common;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Categories
{
    Music,
    Sports,
    History,
    Politics,
    Entertainment,
    Geography
}