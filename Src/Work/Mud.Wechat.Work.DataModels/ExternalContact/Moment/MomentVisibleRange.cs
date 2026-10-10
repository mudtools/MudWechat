// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Moment;

/// <summary>
/// 发表范围的成员 / 部门执行者列表（<c>visible_range.sender_list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class MomentSenderList
{
    /// <summary>
    /// 获取或设置发表任务的执行者用户列表（最多支持 10 万个）。
    /// </summary>
    [JsonPropertyName("user_list")]
    public List<string>? UserList { get; set; }

    /// <summary>
    /// 获取或设置发表任务的执行者部门列表。
    /// </summary>
    [JsonPropertyName("department_list")]
    public List<long>? DepartmentList { get; set; }
}

/// <summary>
/// 发表范围可见的客户列表条件（<c>visible_range.external_contact_list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class MomentExternalContactList
{
    /// <summary>
    /// 获取或设置客户标签列表（仅支持企业客户标签，不支持规则组标签）。
    /// </summary>
    [JsonPropertyName("tag_list")]
    public List<string>? TagList { get; set; }
}

/// <summary>
/// 创建发表朋友圈任务的发表范围（<c>visible_range</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class MomentVisibleRange
{
    /// <summary>
    /// 获取或设置发表任务的执行者列表。
    /// </summary>
    [JsonPropertyName("sender_list")]
    public MomentSenderList? SenderList { get; set; }

    /// <summary>
    /// 获取或设置可见到该朋友圈的客户列表条件。
    /// </summary>
    [JsonPropertyName("external_contact_list")]
    public MomentExternalContactList? ExternalContactList { get; set; }
}
