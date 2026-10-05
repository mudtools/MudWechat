// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Aibot;

namespace Mud.Wechat.Work.Abstractions.Callback.Bots;

/// <summary>
/// 智能机器人回调处理器契约（<b>返回式应答</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何是返回式而非 scope 内 ambient 通道</b>：本 SDK 的两条传输（HTTP 回调应答 / 长连接帧）
/// 都要用同一分发内核，而长连接场景<b>没有 HTTP 请求 scope</b>，ambient scoped 通道不可移植；
/// 返回式契约在两种传输下形态完全一致，且应答是编译期可见的显式返回值（无隐藏状态、无双写点仲裁）。
/// </para>
/// <para>
/// <b><see cref="SupportedEventType"/> 语义</b>（= <see cref="WechatBotCallbackEvent.EventTypeKey"/>）：
/// 填 <see cref="WechatBotEventTypes"/> 常量之一做<b>精确匹配</b>（如 <c>text</c> / <c>enter_chat</c> /
/// <c>template_card_event</c>）；返回<b>空串/null 表示兜底处理器</b>（仅当事件未被任何精确处理器命中时调用）。
/// </para>
/// <para>
/// <b>必须快速返回</b>：分发软超时（默认 4500ms，<b>必须小于企微 5s 应答契约</b>）触发即丢弃/重推；
/// 官方对模板卡片事件与欢迎语/卡片更新更有 <b>5 秒硬约束</b>且<b>只推一次</b>。
/// 长耗时业务（如 LLM 生成）必须「先返回 <c>null</c>（空包）→ 再用 <c>response_url</c> 主动回复或长连接流式补发」。
/// </para>
/// <para>
/// <b>幂等要求</b>：回调地址模式下抗重放指纹在<b>分发前</b>已消费（fail-closed 优先于 at-least-once），
/// 但官方明确 <c>msgid</c> 可能重复推送，宿主仍应按 <see cref="WechatBotCallbackEvent.MsgId"/> 自证幂等。
/// </para>
/// </remarks>
public interface IWechatBotCallbackEventHandler
{
    /// <summary>支持的事件键（= <see cref="WechatBotCallbackEvent.EventTypeKey"/>）；空串/null = 兜底处理器。</summary>
    string SupportedEventType { get; }

    /// <summary>
    /// 处理智能机器人回调并返回应答。
    /// </summary>
    /// <param name="eventData">回调事件信封（含强类型载荷 <see cref="WechatBotCallbackEvent.Message"/> /
    /// <see cref="WechatBotCallbackEvent.Event"/>）。</param>
    /// <param name="cancellationToken">取消令牌（链接请求中止与分发软超时）。</param>
    /// <returns>
    /// 应答消息；<c>null</c> = 无应答（分发器按「加密空包」应答）。
    /// </returns>
    /// <remarks>
    /// <para>
    /// 应答<b>支持面按传输而异</b>：HTTP 被动回复仅接受 <c>text</c>（进入会话欢迎语）/ <c>template_card</c> /
    /// <c>stream</c> / <c>stream_with_template_card</c> / <c>response_type=update_template_card</c>；
    /// <c>markdown</c> 与媒体消息为长连接专用。返回当前传输不支持的形态将被适配层<b>拒绝并 fail-fast</b>。
    /// </para>
    /// <para>
    /// <c>feedback_event</c> <b>仅支持回复空包</b>（官方 101027）：该事件处理器应返回 <c>null</c>。
    /// </para>
    /// </remarks>
    Task<AibotMessage?> HandleAsync(
        WechatBotCallbackEvent eventData, CancellationToken cancellationToken = default);
}
