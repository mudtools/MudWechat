namespace Mud.Wechat.Work.DataModels.ProviderAuthentication;

/// <summary>
/// 获取代开发自建应用带参授权链接响应体（get_customized_auth_url）。
/// </summary>
public class GetCustomizedAuthUrlResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置带参授权二维码链接（宿主自行渲染为二维码图片）。
    /// </summary>
    [JsonPropertyName("qrcode_url")]
    public string? QrcodeUrl { get; set; }

    /// <summary>
    /// 获取或设置二维码有效期（单位：秒），默认 864000 秒（10 天）。
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}