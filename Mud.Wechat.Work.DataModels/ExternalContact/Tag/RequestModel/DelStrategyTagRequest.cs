// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Tag;

/// <summary>
/// 删除指定规则组下的企业客户标签请求体（<c>/cgi-bin/externalcontact/del_strategy_tag</c>）。
/// <para><see cref="TagId"/> 与 <see cref="GroupId"/> 不可同时为空；
/// 标签组内的所有标签被删除后，标签组自动删除。应用仅能删除由本应用创建的规则组标签。</para>
/// </summary>
public class DelStrategyTagRequest
{
    /// <summary>
    /// 获取或设置要删除的标签 id 列表。
    /// </summary>
    [JsonPropertyName("tag_id")]
    public List<string>? TagId { get; set; }

    /// <summary>
    /// 获取或设置要删除的标签组 id 列表（连同组下所有标签一并删除）。
    /// </summary>
    [JsonPropertyName("group_id")]
    public List<string>? GroupId { get; set; }
}
