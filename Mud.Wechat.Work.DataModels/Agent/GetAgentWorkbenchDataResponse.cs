// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Agent;

/// <summary>
/// 获取应用在用户工作台展示的数据响应体（<c>/cgi-bin/agent/get_workbench_data</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方限制：若设置了应用模版且配置 replace_user_data 为 true，应用数据会覆盖个人数据，此时返回应用设置的数据。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Agent")]
public class GetAgentWorkbenchDataResponse : WechatWorkResponse
{
    /// <summary>获取或设置用户工作台展示数据（type + 对应类型的模版数据），详见 <see cref="AgentWorkbenchUserData"/>。</summary>
    [JsonPropertyName("data")]
    public AgentWorkbenchUserData? Data { get; set; }
}
