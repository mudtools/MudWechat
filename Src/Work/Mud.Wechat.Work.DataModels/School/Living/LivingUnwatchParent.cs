// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School.Living;

/// <summary>
/// 未观看直播的家长统计 V2（<c>get_unwatch_stat_v2</c> 响应 <c>stat_info.parents</c> 元素结构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Living")]
public class LivingUnwatchParent
{
    /// <summary>
    /// 获取或设置家长的 userid。
    /// </summary>
    [JsonPropertyName("parent_userid")]
    public string? ParentUserId { get; set; }

    /// <summary>
    /// 获取或设置家长对应学生的 userid。
    /// </summary>
    [JsonPropertyName("student_userid")]
    public string? StudentUserId { get; set; }

    /// <summary>
    /// 获取或设置家长对应学生所在的班级 id 列表。
    /// </summary>
    [JsonPropertyName("partyids")]
    public List<long>? PartyIds { get; set; }
}
