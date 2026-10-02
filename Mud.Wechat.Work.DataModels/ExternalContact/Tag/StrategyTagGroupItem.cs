// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Tag;

/// <summary>
/// 规则组下的企业标签组（获取 / 添加指定规则组下的企业客户标签响应中的 <c>tag_group</c> 结构；
/// 与 <see cref="CorpTagGroupItem"/> 的差异是携带所属规则组 id，且不含删除标记）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Tag")]
public class StrategyTagGroupItem
{
    /// <summary>
    /// 获取或设置标签组 id。
    /// </summary>
    [JsonPropertyName("group_id")]
    public string? GroupId { get; set; }

    /// <summary>
    /// 获取或设置标签组名称。
    /// </summary>
    [JsonPropertyName("group_name")]
    public string? GroupName { get; set; }

    /// <summary>
    /// 获取或设置标签组创建时间。
    /// </summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>
    /// 获取或设置标签组排序的次序值，官方有效范围 [0, 2^32)，值大的排序靠前。
    /// </summary>
    [JsonPropertyName("order")]
    public long? Order { get; set; }

    /// <summary>
    /// 获取或设置标签组所属的规则组 id。
    /// </summary>
    [JsonPropertyName("strategy_id")]
    public long? StrategyId { get; set; }

    /// <summary>
    /// 获取或设置标签组内的标签列表。
    /// </summary>
    [JsonPropertyName("tag")]
    public List<CorpTagItem>? Tag { get; set; }
}
