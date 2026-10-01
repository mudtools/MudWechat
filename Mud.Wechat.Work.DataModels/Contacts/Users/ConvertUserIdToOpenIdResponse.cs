// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Users;

/// <summary>
/// userid 转 openid 响应体（<c>/cgi-bin/user/convert_to_openid</c>）。
/// </summary>
public class ConvertUserIdToOpenIdResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置企业微信成员 userid 对应的 openid。
    /// </summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置应用的 appid（请求包中不包含 agentid 时不返回；该 appid 在使用微信红包时会用到）。
    /// </summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}
