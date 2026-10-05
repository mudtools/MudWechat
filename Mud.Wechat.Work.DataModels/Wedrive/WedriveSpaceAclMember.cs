// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 微盘空间成员/部门权限信息（新建空间 auth_info、添加/移除空间成员 auth_info 与
/// 获取空间信息 auth_list.auth_info 共用的嵌套对象）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约差异：新建空间（space_create）的 auth 取值 1/4/7 且各项选填；
/// 添加成员/部门（space_acl_add）的 auth 取值 1/7 且各字段必填；
/// 移除成员/部门（space_acl_del）不携带 auth 字段（未填充时不会被序列化）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class WedriveSpaceAclMember
{
    /// <summary>获取或设置成员类型：1 - 个人；2 - 部门。</summary>
    [JsonPropertyName("type")]
    public ulong? Type { get; set; }

    /// <summary>获取或设置成员 userid（字符串；type 为 1 时填写）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置部门 departmentid（32 位整型，取值范围 [0, 2^32)；type 为 2 时填写）。</summary>
    [JsonPropertyName("departmentid")]
    public ulong? Departmentid { get; set; }

    /// <summary>
    /// 获取或设置成员权限：1 - 仅下载；4 - 可预览（仅专业版微盘企业可设置）；7 - 应用空间管理员。
    /// <para>应用空间管理员（auth = 7）最多可指定 3 个，且不支持设置部门。</para>
    /// </summary>
    [JsonPropertyName("auth")]
    public ulong? Auth { get; set; }
}
