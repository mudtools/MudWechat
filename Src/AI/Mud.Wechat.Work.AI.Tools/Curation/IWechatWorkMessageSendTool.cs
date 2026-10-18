// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>message.send</c>：向成员发送应用文本消息（受控写）。
/// </summary>
/// <remarks>
/// <para>
/// <b>外部可见、不可撤回（本工具不提供撤回）</b>：消息直达成员的企业微信客户端。标为 <c>IsWrite = true</c>：
/// 默认白名单关闭，宿主须显式 `Enable` + 授权器 + 审批门才可执行。
/// </para>
/// <para>
/// <b>群发语义要收敛</b>：官方支持 <c>touser</c> / <c>toparty</c> / <c>totag</c> 三种收件人轴，本工具只开放
/// <c>touser</c>——按部门/标签群发一旦打错参数就是全员打扰，且难以逐条核对；需要群发时由宿主在自己的审批流里做。
/// </para>
/// </remarks>
[AgentTool(
    name: "message.send",
    Description = "向指定成员发送企业微信应用文本消息（对外可见、不可撤回）。参数 touser 为成员 UserID，多个用 | 分隔（单次不超过 1000 个，建议先少量确认）；content 为文本内容（官方上限 2048 字节）；agentid 为应用 AgentId（管理后台应用详情页可查，必填且须与当前应用一致）；safe 可选（Public 可对外 / Confidential 保密，默认 Public）。返回 msgid，以及 invalid_user / invalid_party / invalid_tag / unlicensed_user（官方即使部分失败也可能返回 errcode 0，必须逐一检查）。发送前请让用户确认收件人与内容。",
    Source = "IWechatWorkMessageService.SendTextMessageAsync",
    RequiredScopes = ["message.send.write"],
    IsWrite = true)]
public interface IWechatWorkMessageSendTool
{
    /// <summary>发送应用文本消息。</summary>
    /// <param name="touser">成员 UserID（多个用 | 分隔）。</param>
    /// <param name="content">文本内容（≤ 2048 字节）。</param>
    /// <param name="agentid">应用 AgentId。</param>
    /// <param name="safe">保密模式（Public 可对外 / Confidential 保密；可空 ⇒ Public）。</param>
    void Send(
        [ToolParameter("touser", "成员 UserID（多个用 | 分隔）。", Required = true)] string touser,
        [ToolParameter("content", "文本内容（≤ 2048 字节）。", Required = true)] string content,
        [ToolParameter("agentid", "应用 AgentId。")] int agentid,
        [ToolParameter("safe", "保密模式（Public 可对外 / Confidential 保密；可空 ⇒ Public）。", EnumType = typeof(WechatMessageSafeModes))] int? safe);
}
