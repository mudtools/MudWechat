// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup.ChainContacts;

/// <summary>
/// 上下游通讯录分组信息（获取上下游通讯录分组响应中 <c>groups[]</c> 的元素）。
/// </summary>
public class ChainGroupInfo
{
    /// <summary>
    /// 获取或设置分组 id。
    /// </summary>
    [JsonPropertyName("groupid")]
    public int? GroupId { get; set; }

    /// <summary>
    /// 获取或设置分组名称。
    /// </summary>
    [JsonPropertyName("group_name")]
    public string? GroupName { get; set; }

    /// <summary>
    /// 获取或设置父分组 id（根分组 id 为 1）。
    /// </summary>
    [JsonPropertyName("parentid")]
    public int? ParentId { get; set; }

    /// <summary>
    /// 获取或设置父分组中的次序值（order 值大的排序靠前；官方范围 [0, 2^32)，以 long 承载）。
    /// </summary>
    [JsonPropertyName("order")]
    public long? Order { get; set; }
}
