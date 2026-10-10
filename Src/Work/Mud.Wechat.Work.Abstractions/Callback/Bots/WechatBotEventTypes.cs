// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback.Bots;

/// <summary>
/// 智能机器人回调事件键 / 消息键常量（对齐 XML 侧 <c>WechatCallbackEventTypes</c> 的用法）。
/// </summary>
/// <remarks>
/// <para>
/// <b>键空间与 XML 侧完全隔离</b>（ADR-2）：智能机器人报文无 <c>ToUserName</c>/<c>InfoType</c>/<c>ChangeType</c>，
/// 无法承载 XML 事件键推导；强行并表还会与既有 <c>template_card_event</c>（应用消息卡片回调）键冲突，
/// 故本键集独立登记、<b>不得</b>混入 <c>OfficialPayloadContracts</c>。
/// </para>
/// <para>
/// 键的推导规则（<c>WechatBotCallbackEvent.EventTypeKey</c>）：事件回调取 <c>event.eventtype</c>，
/// 消息回调取顶层 <c>msgtype</c>；两者皆空返回空串（仅兜底处理器可见）。
/// </para>
/// <para>官方出处：接收事件 101027、接收消息 100719、长连接 101463。</para>
/// </remarks>
public static class WechatBotEventTypes
{
    // ——— 事件键（event.eventtype）———

    /// <summary>进入会话事件：用户当天首次进入智能机器人单聊会话；<b>若未回复消息，当天再次进入不再推送</b>（101027）。</summary>
    /// <remarks>官方仅该事件支持被动回复 <c>text</c> 欢迎语；须在 5 秒内回复（长连接 <c>aibot_respond_welcome_msg</c>）。</remarks>
    public const string EnterChat = "enter_chat";

    /// <summary>模板卡片事件：用户点击模板卡片按钮等；<b>官方只推送一次、5 秒内未响应即丢弃</b>（101027）。</summary>
    public const string TemplateCardEvent = "template_card_event";

    /// <summary>用户反馈事件：用户对机器人回复进行反馈；<b>仅支持回复空包</b>（101027）。</summary>
    public const string FeedbackEvent = "feedback_event";

    /// <summary>
    /// 连接断开事件：<b>仅长连接模式</b>（101463）——同一机器人新连接完成订阅后会踢掉旧连接并推送本事件。
    /// </summary>
    /// <remarks>收到本事件<b>不得立即抢占重连</b>（防乒乓），须退避 + 先夺连接租约。</remarks>
    public const string DisconnectedEvent = "disconnected_event";

    // ——— 消息键（顶层 msgtype）———

    /// <summary>文本消息（群 @ 或单聊）。</summary>
    public const string Text = "text";

    /// <summary>图片消息（仅单聊）。</summary>
    public const string Image = "image";

    /// <summary>图文混排消息（群 @ 或单聊）。</summary>
    public const string Mixed = "mixed";

    /// <summary>语音消息（仅单聊；官方只提供转换后的文本）。</summary>
    public const string Voice = "voice";

    /// <summary>本地文件消息（仅单聊；官方仅支持 100M 以内）。</summary>
    public const string File = "file";

    /// <summary>视频消息（仅单聊；官方仅支持 100M 以内）。</summary>
    public const string Video = "video";

    /// <summary>
    /// 流式消息刷新（回调地址模式专用）。
    /// </summary>
    /// <remarks>
    /// 官方 100719：机器人在 <c>finish=true</c> 前企微反复推送本消息类型（自用户发消息起最多 6 分钟），
    /// 机器人对<b>每次推送</b>经应答写回当前累计内容；本类型<b>不携带 <c>response_url</c></b>。
    /// 长连接模式（101463）无刷新回调，改为开发者主动推送同 id 的流式刷新。
    /// </remarks>
    public const string Stream = "stream";
}
