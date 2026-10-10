// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.JsSdk;

/// <summary>
/// 获取企业 jsapi_ticket 响应体（<c>/cgi-bin/get_jsapi_ticket</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "JsSdk")]
public class GetCorpJsapiTicketResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置生成签名所需的 jsapi_ticket（最长 512 字节）；
    /// 官方要求必须在自己的后台服务中对其进行缓存，有效期 7200 秒（2 小时）。
    /// </summary>
    [JsonPropertyName("ticket")]
    public string Ticket { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置凭证的有效时间（秒），正常情况下为 7200 秒（2 小时），
    /// 具体过期时间以本属性为准。
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
