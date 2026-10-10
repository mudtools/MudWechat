// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Hr;

/// <summary>
/// 人事助手增加字段组项（<c>/cgi-bin/hr/update_staff_info</c> 的 insert_items 内嵌结构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Hr")]
public class HrInsertFieldGroup
{
    /// <summary>
    /// 获取或设置需要增加的字段组类型（官方 group_type，必填，取可重复字段组列表：1 教育经历 / 2 工作经历 /
    /// 3 家庭成员 / 4 紧急联系人 / 5 合同信息）。
    /// </summary>
    [JsonPropertyName("group_type")]
    public long? GroupType { get; set; }

    /// <summary>
    /// 获取或设置需要增加的字段内容列表（官方 item，填写要求与 update_items 相同，但 sub_idx 的内容将被忽略；
    /// 没有找到对应字段 id 的字段内容将被忽略）。
    /// </summary>
    [JsonPropertyName("item")]
    public List<HrUpdateFieldItem>? Item { get; set; }
}
