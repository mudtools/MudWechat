// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Agent;

/// <summary>
/// 批量设置应用在用户工作台展示的数据请求体（<c>/cgi-bin/agent/batch_set_workbench_data</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方限制：userid_list 最多 1000 个；userid 必须在应用可见范围内；
/// 频率限制：每个应用 100000 人次/分钟。
/// </para>
/// <para>
/// 形态差异：本接口的模版数据官方以 <c>data</c> 对象包裹（type + 对应模版数据），
/// 与单用户设置接口的平铺形态不同。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Agent")]
public class BatchSetAgentWorkbenchDataRequest
{
    /// <summary>获取或设置应用 id（官方必填）。</summary>
    [JsonPropertyName("agentid")]
    public int Agentid { get; set; }

    /// <summary>获取或设置用户 userid 列表（官方必填，最多 1000 个；须在应用可见范围内）。</summary>
    [JsonPropertyName("userid_list")]
    public List<string>? UseridList { get; set; }

    /// <summary>获取或设置用户设置的数据（官方必填：type + 对应类型的模版数据），详见 <see cref="AgentWorkbenchUserData"/>。</summary>
    [JsonPropertyName("data")]
    public AgentWorkbenchUserData? Data { get; set; }
}
