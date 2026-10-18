// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.Abstractions.Callback;
using Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

namespace Mud.Wechat.OfficialAccount.Demo.Handlers;

/// <summary>
/// 文本消息处理器（<c>MsgType = text</c>）：演示「消息族」与「事件族」在同一个分发面上按
/// <see cref="MpCallbackEnvelope.EventTypeKey"/> 统一寻址（事件优先取 <c>Event</c>，无 <c>Event</c> 时取 <c>MsgType</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>语音消息会带识别结果</b>：开放语音识别的公众号，语音消息的 <c>Recognition</c> 字段有值，
/// 其事件键是 <c>voice</c> 而非 <c>text</c> —— 想「文本+语音统一进关键词流程」须各注册一个处理器，
/// 不能只挂 text。
/// </para>
/// </remarks>
public sealed class TextMessageHandler(ILogger<TextMessageHandler> logger)
    : MpCallbackPayloadHandler<MpTextMessagePayload>
{
    public override string SupportedEventType => MpCallbackMessageTypes.Text;

    public override Task HandleAsync(
        MpCallbackEnvelope envelope,
        MpTextMessagePayload payload,
        CancellationToken cancellationToken = default)
    {
        // MsgId 是官方排重推荐键（普通消息用 MsgId，事件用 FromUserName + CreateTime）。
        logger.LogInformation(
            "收到文本消息：FromUser={FromUser}, MsgId={MsgId}, 内容={Content}",
            envelope.FromUserName, envelope.MsgId, payload.Content);
        return Task.CompletedTask;
    }
}
