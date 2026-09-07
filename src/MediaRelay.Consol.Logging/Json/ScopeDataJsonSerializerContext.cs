using MediaRelay.Publish;
using MediaRelay.Resources;
using MediaRelay.Source;
using MediaRelay.Storage;
using MediaRelay.Twitter.Image;
using MediaRelay.Twitter.Tweet;
using System.Text.Json.Serialization;

namespace MediaRelay.Console.Logging;


[JsonSerializable(typeof(Dictionary<string, object>))]
[JsonSerializable(typeof(Uri))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(Guid))]
[JsonSerializable(typeof(TimeSpan))]
[JsonSerializable(typeof(SourceId))]
[JsonSerializable(typeof(ResourceId))]
[JsonSerializable(typeof(ImageSize))]
[JsonSerializable(typeof(ImageUrlResource))]
[JsonSerializable(typeof(StorageInfo))]
[JsonSerializable(typeof(TweetContent))]
[JsonSerializable(typeof(PublishContent))]
internal partial class ScopeDataJsonSerializerContext : JsonSerializerContext;