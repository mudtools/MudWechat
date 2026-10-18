// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.DataModels.Aibot;

namespace Mud.Wechat.AI.AgentDemo.Handlers;

/// <summary>
/// <c>enter_chat</c> 欢迎事件处理器：演示机器人通道里<b>唯一允许被动回复</b>的形态。
/// </summary>
/// <remarks>
/// <para>
/// 官方被动回复支持面极窄：<c>text</c> 只在 <c>eventType = enter_chat</c> 时被接受，
/// <c>markdown</c> 只能走主动回复（<c>response_url</c>），<c>feedback_event</c> 回复非空即被拒
/// —— 三条都在接收链里硬校验（<c>WechatBotReplySupport</c>），写错不是「不生效」而是<b>整包被拒</b>。
/// 因此本处理器刻意使用 <see cref="WechatBotReplies.Text"/> 而不是文本消息那套 markdown 主动回复。
/// </para>
/// <para>
/// <b>与 Agent 的分工</b>：欢迎语是一次固定应答，不进会话、不排回合、不调模型
/// —— 把「进会话」交给 Agent 只会平白多一次生成与一段历史。
/// </para>
/// </remarks>
internal sealed class EnterChatWelcomeHandler : IWechatBotCallbackEventHandler
{
    private readonly BotEventLedger _ledger;

    // 构造函数必须是 public：容器在 ValidateOnBuild 下只认公共构造函数（类型本身 internal 不影响解析）。
    public EnterChatWelcomeHandler(BotEventLedger ledger) => _ledger = ledger;

    /// <inheritdoc />
    public string SupportedEventType => WechatBotEventTypes.EnterChat;

    /// <inheritdoc />
    public Task<AibotMessage?> HandleAsync(
        WechatBotCallbackEvent eventData,
        CancellationToken cancellationToken = default)
    {
        _ledger.Record(new BotEventRecord(
            WechatBotEventTypes.EnterChat,
            eventData.AppKey,
            eventData.MsgId,
            eventData.ChatType,
            WechatBotReplyTypes.Text));

        // 被动回复只带一行文案：欢迎语出现在会话里，任何用户内容都不经过这里，无敏感面。
        return Task.FromResult<AibotMessage?>(WechatBotReplies.Text(
            "这里是企业微信智能机器人 Demo（Mud.Wechat）：发一条文本消息，它会经 Agent 回合处理后再回复你。"));
    }
}
