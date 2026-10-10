// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 微盘文件父路径继承权限（获取文件权限信息响应的 inherit_father_auth 对象）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：参数表列出 member_list（文件夹、文档成员）、返回示例实为 auth_list 数组，
/// 本 SDK 两个成员列表均承载；成员元素与空间成员权限结构同构（<see cref="WedriveSpaceAclMember"/>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class WedriveFileInheritFatherAuth
{
    /// <summary>获取或设置是否开启父路径权限继承。</summary>
    [JsonPropertyName("inherit")]
    public bool? Inherit { get; set; }

    /// <summary>获取或设置从文件父路径继承的成员授权列表（返回示例形态）。</summary>
    [JsonPropertyName("auth_list")]
    public List<WedriveSpaceAclMember>? AuthList { get; set; }

    /// <summary>获取或设置文件夹、文档成员列表（官方参数表形态，返回示例未出现）。</summary>
    [JsonPropertyName("member_list")]
    public List<WedriveSpaceAclMember>? MemberList { get; set; }
}
