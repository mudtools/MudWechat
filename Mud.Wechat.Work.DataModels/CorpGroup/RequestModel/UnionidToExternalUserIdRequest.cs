// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup;

/// <summary>
/// 上下游关联客户信息（已添加客户）请求体（<c>/cgi-bin/corpgroup/unionid_to_external_userid</c>）。
/// </summary>
/// <remarks>
/// 调用频率：10 万次/小时、48 万次/天、750 万次/月；传入有效 <see cref="MassCallTicket"/>
/// 可不受此限制（但仍受基础频率限制），适用于数据初始化场景。
/// </remarks>
public class UnionidToExternalUserIdRequest
{
    /// <summary>
    /// 获取或设置微信客户的 unionid。
    /// </summary>
    [JsonPropertyName("unionid")]
    public string UnionId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置微信客户的 openid。
    /// </summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置需要换取的企业 corpid（不填则拉取所有企业；不填时不出网）。
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? CorpId { get; set; }

    /// <summary>
    /// 获取或设置大批量调用凭据（适用于数据初始化场景，有获取及使用限制；不填时不出网）。
    /// </summary>
    [JsonPropertyName("mass_call_ticket")]
    public string? MassCallTicket { get; set; }
}
