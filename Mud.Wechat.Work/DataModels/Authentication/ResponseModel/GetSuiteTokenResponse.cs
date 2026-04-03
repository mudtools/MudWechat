namespace Mud.Wechat.Work.DataModels.Authentication;

/// <summary>
/// 获取第三方应用凭证响应体
/// </summary>
public class GetSuiteTokenResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置第三方或者代开发应用模板access_token,最长为512字节。
    /// </summary>
    [JsonPropertyName("suite_access_token")]
    public string SuiteAccessToken { get; set; } = default!;

    /// <summary>
    /// 获取或设置第三方应用凭证有效期（单位：秒）。
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}