using Newtonsoft.Json.Linq;

namespace MAU.Models;

/// <summary>
/// Struct contains response information
/// </summary>
internal readonly struct ResponseInfoModel
{
    public int RequestId { get; init; }
    public string MauId { get; init; }
    public RequestType RequestType { get; init; }
    public JObject Data { get; init; }
}
