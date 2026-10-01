// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Users;

/// <summary>
/// userid 转 openid 请求体（<c>/cgi-bin/user/convert_to_openid</c>，主要用于企业支付场景）。
/// </summary>
/// <remarks>成员须使用微信登录企业微信或关注微信插件（原企业号）才能转成 openid；外部联系人请使用外部联系人 openid 转换接口。</remarks>
public class ConvertUserIdToOpenIdRequest
{
    /// <summary>
    /// 获取或设置企业内的成员 UserID。
    /// </summary>
    [JsonPropertyName("userid")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置需要发送红包的应用 ID（若只是使用微信支付和企业转账，则无需该参数；不传时响应不返回 <see cref="ConvertUserIdToOpenIdResponse.AppId"/>）。
    /// </summary>
    [JsonPropertyName("agentid")]
    public int? AgentId { get; set; }
}
