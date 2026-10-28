// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Agent;

/// <summary>
/// 获取 access_token 对应的应用列表响应体（<c>/cgi-bin/agent/list</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方权限口径：企业仅可获取当前凭证对应的应用；第三方仅可获取被授权的应用。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Agent")]
public class GetAgentListResponse : WechatWorkResponse
{
    /// <summary>获取或设置应用列表，每项含应用 id、名称与方形头像 url，详见 <see cref="AgentItem"/>。</summary>
    [JsonPropertyName("agentlist")]
    public List<AgentItem>? Agentlist { get; set; }
}
