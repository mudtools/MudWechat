// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.CustomerAcquisition;

/// <summary>
/// 获客链接的使用范围（<c>range</c>）。
/// <para><see cref="UserList"/> 与 <see cref="DepartmentList"/> 不可同时为空；
/// 使用范围覆盖的总人数不超过 500（成员数 + 部门覆盖人数），且须在应用可见范围或客户可建联成员范围内。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "CustomerAcquisition")]
public class AcquisitionLinkRange
{
    /// <summary>
    /// 获取或设置获客链接关联的成员 userid 列表（最多 500 人）。
    /// </summary>
    [JsonPropertyName("user_list")]
    public List<string>? UserList { get; set; }

    /// <summary>
    /// 获取或设置获客链接关联的部门 id 列表（部门覆盖总人数最多 500 个）。
    /// </summary>
    [JsonPropertyName("department_list")]
    public List<long>? DepartmentList { get; set; }
}

/// <summary>
/// 获客链接的优先分配选项（<c>priority_option</c>，仅部分经营类目企业支持）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "CustomerAcquisition")]
public class AcquisitionPriorityOption
{
    /// <summary>
    /// 获取或设置优先分配类型：1 - 全企业范围内优先分配给有好友关系的成员，2 - 指定范围内优先分配。
    /// </summary>
    [JsonPropertyName("priority_type")]
    public int? PriorityType { get; set; }

    /// <summary>
    /// 获取或设置优先分配的成员 userid 列表（priority_type = 2 时必填，最多 1000 个）。
    /// </summary>
    [JsonPropertyName("priority_userid_list")]
    public List<string>? PriorityUseridList { get; set; }
}

/// <summary>
/// 获客链接详情（<c>link</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "CustomerAcquisition")]
public class AcquisitionLink
{
    /// <summary>
    /// 获取或设置获客链接 id。
    /// </summary>
    [JsonPropertyName("link_id")]
    public string? LinkId { get; set; }

    /// <summary>
    /// 获取或设置获客链接名称（最长 30 个字符）。
    /// </summary>
    [JsonPropertyName("link_name")]
    public string? LinkName { get; set; }

    /// <summary>
    /// 获取或设置获客链接的实际链接 url。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 获取或设置获客链接的创建时间（Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>
    /// 获取或设置添加客户时是否无需验证（默认为 true）。
    /// </summary>
    [JsonPropertyName("skip_verify")]
    public bool? SkipVerify { get; set; }

    /// <summary>
    /// 获取或设置是否标记客户添加来源（默认为 true；仅对「营销获客」应用生效）。
    /// </summary>
    [JsonPropertyName("mark_source")]
    public bool? MarkSource { get; set; }

    /// <summary>
    /// 获取或设置获客链接的使用范围。
    /// </summary>
    [JsonPropertyName("range")]
    public AcquisitionLinkRange? Range { get; set; }

    /// <summary>
    /// 获取或设置获客链接的优先分配选项。
    /// </summary>
    [JsonPropertyName("priority_option")]
    public AcquisitionPriorityOption? PriorityOption { get; set; }
}
