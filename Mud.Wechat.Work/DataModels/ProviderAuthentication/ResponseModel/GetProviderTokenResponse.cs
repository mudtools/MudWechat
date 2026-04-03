namespace Mud.Wechat.Work.DataModels.ProviderAuthentication;

/// <summary>
/// 获取服务商凭证响应体
/// </summary>
public class GetProviderTokenResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置服务商凭证。
    /// </summary>
    [JsonPropertyName("provider_access_token")]
    public string ProviderAccessToken { get; set; } = default!;

    /// <summary>
    /// 获取或设置服务商凭证有效期（单位：秒）。
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
