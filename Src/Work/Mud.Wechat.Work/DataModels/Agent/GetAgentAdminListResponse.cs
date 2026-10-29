// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Agent;

/// <summary>
/// 获取应用管理员列表响应体（<c>/cgi-bin/agent/get_admin_list</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方响应体的管理员列表字段名为 <c>admin</c>（不带 <c>_list</c> 后缀），元素结构见 <see cref="AgentAdminItem"/>。
/// </para>
/// <para>
/// 旧的服务商形态 <c>/cgi-bin/service/get_admin_list</c>（以 <c>suite_access_token</c> 调用）列表字段名同为
/// <c>admin</c>，但元素多回传 <c>open_userid</c>（第三方成员唯一标识）；官方现行页已注明该方式「不再维护」，
/// 其文档页亦已脱离目录树 ⇒ 本仓不声明该路由（零声明守卫 RD1 锁定），两侧不共用元素类型。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Agent")]
public class GetAgentAdminListResponse : WechatWorkResponse
{
    /// <summary>获取或设置管理员列表，每项含成员账号与管理权限类型，详见 <see cref="AgentAdminItem"/>。</summary>
    [JsonPropertyName("admin")]
    public List<AgentAdminItem>? AdministratorList { get; set; }
}
