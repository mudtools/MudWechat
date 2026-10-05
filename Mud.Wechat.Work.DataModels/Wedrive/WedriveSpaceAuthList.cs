// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 微盘空间成员及部门权限列表（获取空间信息 auth_list，新旧两版共用同一结构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class WedriveSpaceAuthList
{
    /// <summary>获取或设置空间成员信息列表。</summary>
    [JsonPropertyName("auth_info")]
    public List<WedriveSpaceAclMember>? AuthInfo { get; set; }

    /// <summary>
    /// 获取或设置已退出空间的成员 userid 列表
    /// （成员在一个有权限的部门中自己退出空间或被移除权限的情况）。
    /// </summary>
    [JsonPropertyName("quit_userid")]
    public List<string>? QuitUserid { get; set; }
}
