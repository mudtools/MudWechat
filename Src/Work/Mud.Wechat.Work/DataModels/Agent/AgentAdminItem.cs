// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Agent;

/// <summary>
/// 应用管理员列表项（获取应用管理员列表响应 <c>admin</c> 数组元素）。
/// <para>官方口径：该列表「不包括外部管理员」；<b>成员授权模式下不返回管理员列表</b>。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Agent")]
public class AgentAdminItem
{
    /// <summary>获取或设置成员账号（官方 userid）。</summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>获取或设置该管理员对应用的权限（官方 auth_type：0-发消息权限、1-管理权限）。</summary>
    [JsonPropertyName("auth_type")]
    public int? AuthType { get; set; }
}
