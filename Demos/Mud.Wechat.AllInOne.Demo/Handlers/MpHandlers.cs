// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.Abstractions.Callback;
using Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

namespace Mud.Wechat.AllInOne.Demo.Handlers;

/// <summary>
/// 公众号被动回复处理器：关注 → 欢迎语；文本含「帮助」→ 关键字回复；其余返回 <see langword="null"/>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么共存演示里偏偏要挂回复处理器</b>：公众号线是本宿主里唯一「应答内容取决于宿主代码」的通道
/// （企微/小店应答恒为明文 <c>success</c>、机器人应答是加密外壳）。挂上它，
/// <c>SelfTest --channel mp</c> 才能在与其他线同进程时依然断言到「密文进 → 密文出」这条完整回路，
/// 而不只是断言「闸门没炸」。
/// </para>
/// <para>
/// <b>正文经 <see cref="IMpPayloadReader"/> 读，而不是自己解析 XML</b>：分发器与回复器共用同一份解密明文
/// 与同一套契约登记表，不存在「两套解析逻辑」的分叉面。
/// </para>
/// <para>
/// <b>5 秒契约</b>：回复必须在 5 秒内产出，超时官方断连重推（共 3 次）⇒ 本方法不得内嵌远程调用。
/// 刻意<b>不</b>做 DM-04 那样的排重器（<c>MpReplyDeduper</c>）：本 Demo 不产生对外副作用，
/// 官方重推导致的重复回包在这里无害；真实宿主发的是客服消息时，排重是必须的。
/// </para>
/// </remarks>
internal sealed class MpKeywordReplyHandler(AllInOneLedger ledger, IMpPayloadReader payloadReader)
    : IMpCallbackReplyHandler
{
    public Task<MpCallbackReply?> TryReplyAsync(MpCallbackEnvelope envelope, CancellationToken cancellationToken = default)
    {
        ledger.Record("mp", envelope.AppKey, Describe(envelope), "(回复判定)");

        if (string.Equals(envelope.Event, MpCallbackEventTypes.Subscribe, StringComparison.Ordinal))
        {
            return Task.FromResult<MpCallbackReply?>(MpCallbackReply.Text("欢迎关注（多线共存 Demo）：回复「帮助」看关键字回复。"));
        }

        if (string.Equals(envelope.MsgType, MpCallbackMessageTypes.Text, StringComparison.Ordinal))
        {
            var read = payloadReader.Read<MpTextMessagePayload>(envelope);
            if (read.Status == MpPayloadReadStatus.Matched
                && read.Payload?.Content?.Contains("帮助", StringComparison.Ordinal) == true)
            {
                return Task.FromResult<MpCallbackReply?>(MpCallbackReply.Text(
                    "本宿主同时挂载企微 / 公众号 / 支付 / 小店 / 开放平台五条线，"
                    + "这条是公众号 XML 通道的关键字回复。"));
            }
        }

        // 不回复：SDK 以明文 success 应答（官方视为「已接收」，不再重推）。
        return Task.FromResult<MpCallbackReply?>(null);
    }

    /// <summary>台账事件键：事件族取 <c>Event</c>，消息族取 <c>MsgType</c>（与分发器同口径）。</summary>
    private static string Describe(MpCallbackEnvelope envelope)
        => !string.IsNullOrWhiteSpace(envelope.Event)
            ? envelope.Event!
            : envelope.MsgType ?? "message";
}
