namespace Mud.Wechat.Work.DataModels.ProviderAuthentication;

/// <summary>
/// 获取预授权码响应体
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ProviderAuthentication")]
public class GetPreAuthCodeResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置预授权码，预授权码,最长为512字节。
    /// </summary>
    [JsonPropertyName("pre_auth_code")]
    public string PreAuthCode { get; set; } = default!;

    /// <summary>
    /// 获取或设置预授权码有效期（单位：秒）。
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
