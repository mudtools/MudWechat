// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 添加成员/部门请求体（<c>/cgi-bin/wedrive/space_acl_add</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class AddWedriveSpaceAclRequest
{
    /// <summary>获取或设置空间 spaceid（官方必填）。</summary>
    [JsonPropertyName("spaceid")]
    public string? Spaceid { get; set; }

    /// <summary>
    /// 获取或设置被添加的空间成员信息（官方必填，可一次性添加多个）。
    /// <para>应用空间管理员（auth = 7）连同已设置的管理员最多可指定 3 个，且不支持将部门设为管理员。</para>
    /// </summary>
    [JsonPropertyName("auth_info")]
    public List<WedriveSpaceAclMember>? AuthInfo { get; set; }
}
