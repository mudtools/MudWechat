namespace Mud.Wechat.Work.DataModels.InternalAppAuthentication;

/// <summary>
/// 获取access_token响应体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "InternalAppAuthentication")]
public class GetTokenResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置凭证。
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = default!;

    /// <summary>
    /// 获取或设置凭证有效期（单位：秒）。
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
