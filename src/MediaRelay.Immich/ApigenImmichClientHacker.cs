using Apigen.Immich.Models;
using MediaRelay.Immich.Json;
using System.Reflection;
using System.Text.Json;

namespace MediaRelay.Immich;

internal sealed class ApigenImmichClientHacker
{
    private static int _isHacked;

    public static void Hack()
    {
        if (Interlocked.CompareExchange(ref _isHacked, 1, 0) == 1)
            return;

        try
        {
            var options = Get_JsonConfig_Default_JsonSerializerOptions();
            options.Converters.Clear();
            options.Converters.Add(new NumberStringConverter());
            options.TypeInfoResolver = ImmichJsonSerializerContext.Default;

            // Test
            Test_JsonConfig_Default_JsonSerializerOptions(options);
        }
        catch (Exception)
        {
            Interlocked.Exchange(ref _isHacked, 0);
            throw;
        }
    }


    private static JsonSerializerOptions Get_JsonConfig_Default_JsonSerializerOptions()
    {
        try
        {
            var type = Type.GetType("Apigen.Immich.Client.JsonConfig, Apigen.Immich.Client")
                ?? throw new TypeLoadException("找不到 Apigen.Immich.Client.JsonConfig 类型");

            var field = type.GetField("Default", BindingFlags.Static | BindingFlags.NonPublic)
                ?? throw new MissingFieldException("Apigen.Immich.Client.JsonConfig", "Default");

            return (JsonSerializerOptions?)field.GetValue(null)
                ?? throw new ArgumentNullException("Apigen.Immich.Client.JsonConfig.Default");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("无法获取 Apigen.Immich.Client.JsonConfig.Default", ex);
        }
    }

    private static void Test_JsonConfig_Default_JsonSerializerOptions(JsonSerializerOptions options)
    {
        var dto = new AssetMediaResponseDto()
        {
            Id = "123456789",
            Status = AssetMediaStatus.Created
        };

        try
        {
            // 序列化
#pragma warning disable IL2026, IL3050 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            var jsonString = JsonSerializer.Serialize(dto, options);
#pragma warning restore IL2026, Il3050 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            ArgumentException.ThrowIfNullOrWhiteSpace(jsonString);

            // 反序列化
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            var obj = JsonSerializer.Deserialize<AssetMediaResponseDto>(jsonString, options);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            ArgumentNullException.ThrowIfNull(obj);

            // 比较
            if (obj.Id != dto.Id || obj.Status != dto.Status)
                throw new InvalidOperationException("Json序列化前后比对结果不一致");
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Json序列化测试失败", ex);
        }
    }
}