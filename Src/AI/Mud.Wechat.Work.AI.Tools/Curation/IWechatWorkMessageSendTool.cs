// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>message.send</c>：向成员发送应用消息，形态由 <c>msg_type</c> 闭集选择（纯文本 / markdown / 文本卡片）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么一个工具三种形态，而不是三个工具</b>：三者的官方端点、收件人轴、响应结构完全一致，只有消息体不同。
/// 拆成 `message.send.text` / `.markdown` / `.textcard` 会让模型在三个近似描述之间随机选（描述长度还 ×3），
/// 而它本来只该决定「用什么形式表达」。形态用闭集参数表达，选择成本压到最低。
/// </para>
/// <para>
/// <b>外部可见，但 24 小时内可撤回</b>：配套 <c>message.recall</c>。因此本工具是「可补救的受控写」（L2），
/// 不是不可逆高危写；描述里必须写清撤回窗口，否则模型会在写错后不敢修。
/// </para>
/// <para>
/// <b>只开放 <c>touser</c> 轴</b>：官方支持 <c>touser</c> / <c>toparty</c> / <c>totag</c>，本工具只给 <c>touser</c>
/// ——按部门/标签群发一旦参数打错就是全员打扰，且难以逐条核对；需要群发时由宿主在自己的审批流里做。
/// </para>
/// <para>
/// <b>Source 为什么仍指文本端点</b>：三个形态共用 <c>POST /cgi-bin/message/send</c> 与同一响应信封，
/// <c>Source</c> 决定路由与输出 Schema——指文本端点得到的是正确路由与同一信封；形态差异在执行器内分派。
/// </para>
/// </remarks>
[AgentTool(
    name: "message.send",
    Description = "向指定成员发送企业微信应用消息（对外可见，24 小时内可用 message.recall 撤回）。参数 touser 为成员 UserID，多个用 | 分隔（单次不超过 1000 个，建议先少量确认）；msg_type 选择形态（Text 纯文本默认 / Markdown 富文本 / TextCard 可点击卡片）；content 为正文（Text/Markdown ≤ 2048 字节；TextCard 时作为描述，≤ 512 字符）；agentid 为应用 AgentId（必填且须与当前应用一致）；title、url 仅 TextCard 必填（title ≤ 128 字符，url ≤ 2048 字节且含 http/https）；btn_txt 可选（卡片按钮文字，≤ 4 字，默认「详情」）；safe 可选（Public 可对外 / Confidential 保密；仅 Text 形态生效——官方 markdown/textcard 请求体无此字段）。返回 msgid 与 invalid_user / invalid_party / invalid_tag / unlicensed_user（官方即使部分失败也可能返回 errcode 0，必须逐一检查）。发送前请让用户确认收件人与内容。",
    Source = "IWechatWorkMessageService.SendTextMessageAsync",
    RequiredScopes = ["message.send.write"],
    IsWrite = true)]
public interface IWechatWorkMessageSendTool
{
    /// <summary>发送应用消息（文本 / markdown / 文本卡片）。</summary>
    /// <returns>白名单投影后的 JSON 文本（ok、msg_type、msgid、invalid_user、invalid_party、invalid_tag、unlicensed_user）。</returns>
    Task<string> SendMessageAsync(
        [ToolParameter("touser", "成员 UserID（多个用 | 分隔）。", Required = true)] string touser,
        [ToolParameter("content", "正文（Text/Markdown ≤ 2048 字节；TextCard 时作为描述，≤ 512 字符）。", Required = true)] string content,
        [ToolParameter("agentid", "应用 AgentId。")] int agentid,
        [ToolParameter("msg_type", "消息形态（Text 纯文本 / Markdown 富文本 / TextCard 卡片；可空 ⇒ Text）。", EnumType = typeof(WechatMessageTypes))] int? msg_type,
        [ToolParameter("title", "卡片标题（仅 TextCard 必填，≤ 128 字符）。")] string? title,
        [ToolParameter("url", "卡片跳转链接（仅 TextCard 必填，≤ 2048 字节，须含 http/https）。")] string? url,
        [ToolParameter("btn_txt", "卡片按钮文字（仅 TextCard，≤ 4 字，可空 ⇒ 详情）。")] string? btn_txt,
        [ToolParameter("safe", "保密模式（Public 可对外 / Confidential 保密；可空 ⇒ Public）。", EnumType = typeof(WechatMessageSafeModes))] int? safe,
        CancellationToken cancellationToken = default);
}
