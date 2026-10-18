// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>kf.message.send</c>：向客户发送微信客服文本消息（受控写）。
/// </summary>
/// <remarks>
/// <para>
/// <b>对外可见、不可撤回</b>：消息一旦发出即到达客户微信，撤回须另行调用官方撤回接口（本工具不做）。
/// 故标为 <c>IsWrite = true</c>：默认白名单关闭、需宿主显式 `Enable` + 授权器 + 审批门三件套齐备才可执行
/// （缺任一即 fail-closed）。
/// </para>
/// <para>
/// <b>会话窗口限制</b>：微信客服只能在客户主动发起会话后的官方窗口期内回复，超窗发送会失败；
/// 描述里写明这条，避免模型把「发送失败」误判为权限问题。
/// </para>
/// </remarks>
[AgentTool(
    name: "kf.message.send",
    Description = "向指定客户发送微信客服文本消息（对外可见、不可撤回）。参数 open_kfid 为客服账号 ID（先用 kf.account.list 获取）；touser 为客户 external_userid；content 为文本内容（官方单条长度有限制，建议 500 字以内）。返回 msgid（发送成功时非空）。只能在客户主动发起会话后的官方窗口期内发送，超窗会被官方拒绝；发送前应确认收件人身份，避免发错人。",
    Source = "IWechatWorkKfSessionService.SendTextMessageAsync",
    RequiredScopes = ["kf.message.write"],
    IsWrite = true)]
public interface IWechatWorkKfMessageSendTool
{
    /// <summary>发送客服文本消息。</summary>
    /// <param name="open_kfid">客服账号 ID。</param>
    /// <param name="touser">客户 external_userid。</param>
    /// <param name="content">文本内容。</param>
    void Send(
        [ToolParameter("open_kfid", "客服账号 ID。", Required = true)] string open_kfid,
        [ToolParameter("touser", "客户 external_userid。", Required = true)] string touser,
        [ToolParameter("content", "文本内容。", Required = true)] string content);
}
