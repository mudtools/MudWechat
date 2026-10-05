// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 删除成员（文件权限）请求体（<c>/cgi-bin/wedrive/file_acl_del</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 文件权限成员与空间成员共用同一结构（<see cref="WedriveSpaceAclMember"/>）；
/// 官方请求形态不携带 auth 字段，成员类型 type 与部门 departmentid 官方标注「后续将废弃」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class DelWedriveFileAclRequest
{
    /// <summary>获取或设置文件 fileid（官方必填）。</summary>
    [JsonPropertyName("fileid")]
    public string? Fileid { get; set; }

    /// <summary>获取或设置被移除的成员信息（官方必填，可一次性删除多个）。</summary>
    [JsonPropertyName("auth_info")]
    public List<WedriveSpaceAclMember>? AuthInfo { get; set; }
}
