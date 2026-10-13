// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Agent;

/// <summary>
/// 应用权限扩展信息项（获取应用权限响应 <c>app_permissions_ext</c> 数组元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Agent")]
public class AgentPermissionExtItem
{
    /// <summary>获取或设置权限名称（官方 permission_name，与 app_permissions 元素同为「分类:子分类:权限」形态字符串）。</summary>
    [JsonPropertyName("permission_name")]
    public string? PermissionName { get; set; }

    /// <summary>
    /// 获取或设置权限过期时间（官方 expire_time，秒级时间戳）。
    /// <para>官方口径：「若权限不会过期则 expire_time 字段值为 0」，故 0 不是「已过期」而是「永久有效」，勿按时间戳大小直接判失效。</para>
    /// </summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }
}
