// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>message.recall</c>：撤回本应用 24 小时内发送的应用消息（纠错动作，受控写 L2）。
/// </summary>
/// <remarks>
/// <para>
/// <b>它是 <c>message.send</c> 的配对，不是可选装饰</b>：没有撤回时，「发错人 / 发错内容」的唯一补救是
/// 再发一条更正消息（对客户侧是二次打扰，对内部是信息污染）。有了撤回，模型才敢在核对后立刻纠错。
/// </para>
/// <para>
/// <b>为什么不是 L3</b>：撤回是<b>恢复</b>动作而非破坏动作——它把一个错误的外部副作用消除。
/// 剖面危险词表已移除 <c>recall</c>（补救词，不是破坏词），故按 <c>IsWrite</c> 落到 L2（需显式启用 + 授权 + 确认门）。
/// </para>
/// <para>
/// <b>官方边界必须写进描述</b>：仅 24 小时内、仅本应用发送的消息、仅企业微信端（微信插件端数据不支持撤回）。
/// 模型据此在超窗时如实告知，而不是反复重试。
/// </para>
/// </remarks>
[AgentTool(
    name: "message.recall",
    Description = "撤回本应用发送的应用消息（仅限 24 小时内、仅本应用发送、仅企业微信端；微信插件端数据不支持撤回）。参数 msgid 为发送接口返回的消息 ID。返回 ok。用于发现发错人/发错内容后立即纠错；超窗或非本应用消息会返回官方错误码，此时应如实告知用户并改用发送更正消息。",
    Source = "IWechatWorkMessageService.RecallMessageAsync",
    RequiredScopes = ["message.send.write"],
    IsWrite = true,
    OutputSchema = "{\"type\":\"object\",\"properties\":{\"ok\":{\"type\":\"boolean\"},\"msgid\":{\"type\":\"string\"}},\"required\":[\"ok\"]}")]
public interface IWechatWorkMessageRecallTool
{
    /// <summary>撤回应用消息。</summary>
    /// <returns>白名单投影后的 JSON 文本（ok、msgid）。</returns>
    Task<string> RecallMessageAsync(
        [ToolParameter("msgid", "消息 ID（发送接口返回的 msgid）。", Required = true)] string msgid,
        CancellationToken cancellationToken = default);
}
