// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Agent;

/// <summary>
/// 获取应用权限响应体（<c>/cgi-bin/agent/get_permissions</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方响应体的权限列表字段名为 <c>app_permissions</c>（数组，非单值字符串）；
/// 权限字符串的取值字典在该页附录「应用权限说明」中按分类列表给出，SDK 不做枚举建模（附录随官方扩充）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Agent")]
public class GetAgentPermissionsResponse : WechatWorkResponse
{
    /// <summary>获取或设置应用权限列表（官方 app_permissions，元素为「分类:子分类:权限」形态字符串，详见官方附录）。</summary>
    [JsonPropertyName("app_permissions")]
    public List<string>? AppPermissionList { get; set; }

    /// <summary>
    /// 获取或设置应用权限的扩展信息列表（官方 app_permissions_ext）。
    /// <para>官方口径：「若存在应用权限为二次授权并且有时效时，返回该字段列表」——即二次授权有时效时，否则不返回。</para>
    /// </summary>
    [JsonPropertyName("app_permissions_ext")]
    public List<AgentPermissionExtItem>? AppPermissionExtList { get; set; }
}
