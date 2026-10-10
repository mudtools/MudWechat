// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.GroupMsg;

/// <summary>
/// 创建企业群发的客户标签过滤结构（<c>tag_filter</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class GroupMsgTagFilter
{
    /// <summary>
    /// 获取或设置群发标签分组列表（不同组之间为「且」关系）。
    /// </summary>
    [JsonPropertyName("group_list")]
    public List<GroupMsgTagGroup>? GroupList { get; set; }
}

/// <summary>
/// 群发标签分组（<c>tag_filter.group_list[]</c> 元素；同组标签之间为「或」关系，每组最多 100 个标签，支持规则组标签）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class GroupMsgTagGroup
{
    /// <summary>
    /// 获取或设置该组内的客户标签 id 列表（同组标签为「或」关系，每组最多 100 个）。
    /// </summary>
    [JsonPropertyName("tag_list")]
    public List<string>? TagList { get; set; }
}
