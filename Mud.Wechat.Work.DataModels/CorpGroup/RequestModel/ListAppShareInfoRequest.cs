// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup;

/// <summary>
/// 获取应用共享信息请求体（<c>/cgi-bin/corpgroup/corp/list_app_share_info</c>）。
/// </summary>
public class ListAppShareInfoRequest
{
    /// <summary>
    /// 获取或设置上级/上游企业应用 agentid。
    /// </summary>
    [JsonPropertyName("agentid")]
    public int AgentId { get; set; }

    /// <summary>
    /// 获取或设置业务类型：0 - 企业互联/局校互联，1 - 上下游企业（不填时不出网，由官方按默认处理）。
    /// </summary>
    [JsonPropertyName("business_type")]
    public int? BusinessType { get; set; }

    /// <summary>
    /// 获取或设置下级/下游企业 corpid（指定时仅拉取该企业的应用共享信息；不填时不出网）。
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? CorpId { get; set; }

    /// <summary>
    /// 获取或设置返回的最大记录数（整型，最大值 100，默认情况或者值为 0 表示拉取全量数据；
    /// 建议分页拉取或通过指定 corpid 参数拉取）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// 获取或设置分页查询游标（由上一次调用返回；首次调用可不填，不填时不出网）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }
}
