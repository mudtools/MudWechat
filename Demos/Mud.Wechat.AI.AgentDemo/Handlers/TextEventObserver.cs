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
/// 文本消息的<b>观察用</b>处理器：与 SDK 内置的 <c>WechatBotAgentHandler</c> 同键并存，只记账、恒回空包。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么要多这一个</b>：分发器对同一事件键执行<b>全部</b>命中处理器（「全量执行 + 首个非 null 应答」），
/// 而 SDK 内置处理器不认识本 Demo 的台账 —— 只装内置处理器时，<c>text</c> 事件在 HTTP 应答上是空包、
/// 在台账里也不留痕，「回合到底有没有排进去」就只剩日志可查。补一个零副作用的记录器，
/// 才让 /api/bot-events 成为判定依据。返回 <see langword="null"/> 保证它不参与应答竞争。
/// </para>
/// <para>
/// <b>可观测的边界（本 Demo 想显式承认的一点）</b>：这里记到的是「处理器被调用」，
/// 不是「Agent 回合已完成」—— 回合在后台调度器里跑，主动回复走 <c>response_url</c>，
/// 其结果只出现在日志与企微会话里。台账与回合是两个独立的事实，不要混为一谈。
/// </para>
/// </remarks>
internal sealed class TextEventObserver : IWechatBotCallbackEventHandler
{
    private readonly BotEventLedger _ledger;

    // 构造函数必须是 public：容器在 ValidateOnBuild 下只认公共构造函数（类型本身 internal 不影响解析）。
    public TextEventObserver(BotEventLedger ledger) => _ledger = ledger;

    /// <inheritdoc />
    public string SupportedEventType => WechatBotEventTypes.Text;

    /// <inheritdoc />
    public Task<AibotMessage?> HandleAsync(
        WechatBotCallbackEvent eventData,
        CancellationToken cancellationToken = default)
    {
        // 正文（text.content）一律不落账：台账要能安全回显，就必须只放非敏感字段。
        _ledger.Record(new BotEventRecord(
            WechatBotEventTypes.Text,
            eventData.AppKey,
            eventData.MsgId,
            eventData.ChatType,
            "HTTP 空包（回合已排后台）"));

        return Task.FromResult<AibotMessage?>(null);
    }
}
