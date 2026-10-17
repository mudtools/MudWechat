// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.Abstractions.Callback;
using Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;
using Mud.Wechat.OfficialAccount.Demo.Services;

namespace Mud.Wechat.OfficialAccount.Demo.Handlers;

/// <summary>
/// 被动回复处理器：关注 → 欢迎语；文本含「帮助」→ 指引；其余返回 <see langword="null"/>（SDK 回明文 <c>success</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>回复处理器的入面只有信封</b>（<see cref="IMpCallbackReplyHandler.TryReplyAsync"/>），
/// 需要消息正文时经 <see cref="IMpPayloadReader"/> 读同一份解密明文 ——
/// 分发器与回复器共用读取器，不存在「两套解析逻辑」的分叉面。
/// </para>
/// <para>
/// <b>5 秒契约</b>：回复必须在 5 秒内产出，超时官方断连重推（共 3 次）。
/// 故本方法<b>不得</b>内嵌远程调用、发消息、写慢存储；需要重活时回 <c>success</c> 后走客服消息/模板消息。
/// 另：被动回复不受「48 小时客服会话窗口」限制，这是它与客服消息的本质区别。
/// </para>
/// </remarks>
public sealed class WelcomeReplyHandler(
    ILogger<WelcomeReplyHandler> logger,
    IMpPayloadReader payloadReader,
    MpReplyDeduper deduper) : IMpCallbackReplyHandler
{
    public Task<MpCallbackReply?> TryReplyAsync(
        MpCallbackEnvelope envelope,
        CancellationToken cancellationToken = default)
    {
        var key = MpReplyDeduper.BuildKey(envelope);
        if (!deduper.TryBegin(key))
        {
            // 官方重推（同事件换个 nonce/时间戳重新投递）会穿过 SDK 指纹闸到达这里，静默即可，勿重复回包。
            logger.LogInformation("重复推送已忽略（被动回复排重）：{Key}", key);
            return Task.FromResult<MpCallbackReply?>(null);
        }

        if (string.Equals(envelope.Event, MpCallbackEventTypes.Subscribe, StringComparison.Ordinal))
        {
            return Task.FromResult<MpCallbackReply?>(
                MpCallbackReply.Text("欢迎关注！回复「帮助」查看指引，或直接留言。"));
        }

        if (string.Equals(envelope.MsgType, MpCallbackMessageTypes.Text, StringComparison.Ordinal))
        {
            var read = payloadReader.Read<MpTextMessagePayload>(envelope);
            if (read.Status == MpPayloadReadStatus.Matched
                && read.Payload?.Content?.Contains("帮助", StringComparison.Ordinal) == true)
            {
                return Task.FromResult<MpCallbackReply?>(
                    MpCallbackReply.Text(
                        "本 Demo 演示：关注欢迎语、关键字回复、OAuth 网页授权、JS-SDK 签名、模板消息。" +
                        "详见 /jssdk-demo.html 与 README。"));
            }
        }

        // 不回复：SDK 以明文 success 应答（官方视为「已接收」，不再重推）。
        return Task.FromResult<MpCallbackReply?>(null);
    }
}
