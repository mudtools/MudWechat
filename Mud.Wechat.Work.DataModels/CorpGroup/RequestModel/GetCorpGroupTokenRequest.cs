// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup;

/// <summary>
/// 获取下级/下游企业的 access_token 请求体（<c>/cgi-bin/corpgroup/corp/gettoken</c>）。
/// </summary>
public class GetCorpGroupTokenRequest
{
    /// <summary>
    /// 获取或设置已授权的下级/下游企业 corpid。
    /// </summary>
    [JsonPropertyName("corpid")]
    public string CorpId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置已授权的下级/下游企业应用 ID（agentid）。
    /// </summary>
    [JsonPropertyName("agentid")]
    public int AgentId { get; set; }

    /// <summary>
    /// 获取或设置业务类型：0 - 企业互联/局校互联（默认），1 - 上下游企业（不填时不出网，由官方按默认 0 处理）。
    /// </summary>
    [JsonPropertyName("business_type")]
    public int? BusinessType { get; set; }
}
