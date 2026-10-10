
namespace Mud.Wechat.Work.DataModels.ProviderAuthentication;

/// <summary>
/// 获取服务商凭证的请求体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ProviderAuthentication")]
public class GetProviderTokenRequest
{
    /// <summary>
    /// 获取或设置企业微信 CorpId。对应 <c>CorpId</c>/> 参数。
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? CorpId { get; set; }

    /// <summary>
    /// 获取或设置企业微信服务商 Secret。对应 <c>ProviderSecret</c> 参数。
    /// </summary>
    [JsonPropertyName("provider_secret")]
    public string? ProviderSecret { get; set; }
}
