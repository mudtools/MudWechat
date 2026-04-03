using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Mud.Wechat.Work;

internal class WechatWorkHttpClient : EnhancedHttpClient, IEnhancedHttpClient
{
    private readonly ILogger<WechatWorkHttpClient> _logger;
    private readonly IOptionsMonitor<JsonSerializerOptions> _jsonSerializerOptionsMonitor;

    public WechatWorkHttpClient(
        HttpClient httpClient,
        ILogger<WechatWorkHttpClient> logger,
        bool? enableLogging,
        IOptionsMonitor<JsonSerializerOptions> serializerOptions) : base(httpClient, logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _jsonSerializerOptionsMonitor = serializerOptions ?? throw new ArgumentNullException(nameof(serializerOptions));

    }

    protected override JsonSerializerOptions? GetJsonSerializerOptions()
    {
        return _jsonSerializerOptionsMonitor.CurrentValue;
    }

    public override string EncryptContent(object content, string propertyName = "data", SerializeType serializeType = SerializeType.Json)
    {
        _logger.LogWarning("当前类库无须使用请求体加密功能。");
        throw new NotImplementedException("当前类库无须使用请求体加密功能。");
    }
}
