namespace Mud.Wechat.Work.DataModels.ProviderAuthentication;

/// <summary>
/// 设置授权配置请求体
/// </summary>
public class SetSessionInfoRequest
{
    /// <summary>
    /// 获取或设置预授权码。
    /// </summary>
    [JsonPropertyName("pre_auth_code")]
    public string PreAuthCode { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置本次授权过程中需要用到的会话信息。
    /// </summary>
    [JsonPropertyName("session_info")]
    public SessionInfo Session { get; set; } = new SessionInfo();
}

/// <summary>
/// 授权过程中需要用到的会话信息
/// </summary>
public class SessionInfo
{
    /// <summary>
    /// 获取或设置允许授权的 AppId。
    /// </summary>
    [JsonPropertyName("appid")]
    public IList<int>? AppIdList { get; set; }

    /// <summary>
    /// 获取或设置授权类型。
    /// </summary>
    [JsonPropertyName("auth_type")]
    public int? AuthType { get; set; }
}