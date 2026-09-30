namespace Mud.Wechat.Work.DataModels.ProviderAuthentication;

/// <summary>
/// 获取代开发自建应用带参授权链接请求体（get_customized_auth_url）。
/// </summary>
public class GetCustomizedAuthUrlRequest
{
    /// <summary>
    /// 获取或设置授权方标识（state）。
    /// 仅允许 a-zA-Z0-9，最长 32 字节；授权成功后原样回传。
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// 获取或设置代开发模板 id 列表（最多 9 个）。
    /// </summary>
    [JsonPropertyName("templateid_list")]
    public List<string> TemplateIdList { get; set; } = new();
}