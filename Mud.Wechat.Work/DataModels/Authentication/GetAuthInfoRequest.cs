namespace Mud.Wechat.Work.DataModels.Authentication;

/// <summary>
/// 获取企业授权信息请求体
/// </summary>
public class GetAuthInfoRequest
{
    /// <summary>
    /// 获取或设置授权方 CorpId。
    /// </summary>
    [JsonPropertyName("auth_corpid")]
    public string AuthorizerCorpId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置永久授权码。
    /// </summary>
    [JsonPropertyName("permanent_code")]
    public string PermanentAuthCode { get; set; } = string.Empty;
}