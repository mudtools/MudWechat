// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup.ChainContacts;

/// <summary>
/// 获取企业上下游通讯录分组下的企业详情列表请求体（<c>/cgi-bin/corpgroup/corp/get_chain_corpinfo_list</c>）。
/// </summary>
/// <remarks>如需获取某分组及其子分组的所有企业详情，需先获取该分组下的所有子分组，再逐层递归获取子分组下的企业。</remarks>
public class GetChainCorpInfoListRequest
{
    /// <summary>
    /// 获取或设置上下游 id。
    /// </summary>
    [JsonPropertyName("chain_id")]
    public string ChainId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置分组 id（不填表示根目录；不填时不出网）。
    /// </summary>
    [JsonPropertyName("groupid")]
    public int? GroupId { get; set; }

    /// <summary>
    /// 获取或设置是否需要返回未加入的企业（官方默认不返回；不填时不出网）。
    /// </summary>
    [JsonPropertyName("need_pending")]
    public bool? NeedPending { get; set; }

    /// <summary>
    /// 获取或设置分页游标（传入返回值 next_cursor；不填时不出网）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置分页大小（&gt; 0 时开启分页功能；不填时不出网）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
