// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Agent;

/// <summary>
/// 工作台关键数据型模版/数据结构（<c>keydata</c> 字段）。
/// </summary>
/// <remarks>
/// <para>官方限制：items 数组不超过 4 个。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Agent")]
public class AgentWorkbenchKeyData
{
    /// <summary>获取或设置关键数据型条目数组（不超过 4 个），详见 <see cref="AgentWorkbenchKeyDataItem"/>。</summary>
    [JsonPropertyName("items")]
    public List<AgentWorkbenchKeyDataItem>? Items { get; set; }
}
