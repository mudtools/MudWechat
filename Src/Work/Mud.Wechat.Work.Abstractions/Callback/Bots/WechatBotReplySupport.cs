// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Aibot;

namespace Mud.Wechat.Work.Abstractions.Callback.Bots;

/// <summary>
/// 智能机器人应答的<b>传输支持面</b>校验（同一超集模型，各传输支持面不同 ⇒ 发送前 fail-fast）。
/// </summary>
/// <remarks>
/// <para>
/// 让「不合法的应答组合」在 SDK 边界即失败，而不是静默发给企微后被丢弃
/// （官方对模板卡片事件与欢迎语只推一次、超时即弃，静默失败无法补救）。
/// </para>
/// <para>
/// 支持面依据：官方 101031（HTTP 被动回复）、101138（HTTP 主动回复）、101463（长连接回复）。
/// </para>
/// </remarks>
public static class WechatBotReplySupport
{
    /// <summary>
    /// 校验应答可用于<b>HTTP 被动回复</b>（回调请求的加密应答明文），不合法即抛 <see cref="InvalidOperationException"/>。
    /// </summary>
    /// <param name="message">处理器返回的应答消息。</param>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> 为 <c>null</c>。</exception>
    /// <exception cref="InvalidOperationException">应答形态不被 HTTP 被动回复支持，或结构不自洽。</exception>
    /// <remarks>
    /// 官方 101031 支持：<c>text</c>（仅进入会话回调事件）/ <c>template_card</c> / <c>stream</c> /
    /// <c>stream_with_template_card</c> / <c>response_type=update_template_card</c>。
    /// <c>markdown</c> 与媒体消息（<c>file</c>/<c>image</c>/<c>voice</c>/<c>video</c>）为长连接专用，此处拒绝。
    /// </remarks>
    public static void ValidateHttpPassiveReply(AibotMessage message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // 更新模板卡片应答：以 response_type 表达，不得同时声明 msgtype。
        if (message.ResponseType != null && message.ResponseType.Length > 0)
        {
            if (!string.Equals(message.ResponseType, WechatBotReplyTypes.UpdateTemplateCard, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "智能机器人被动回复不支持的 response_type：" + message.ResponseType +
                    "（官方 101031 仅支持 " + WechatBotReplyTypes.UpdateTemplateCard + "）。");
            }

            if (message.MsgType != null && message.MsgType.Length > 0)
            {
                throw new InvalidOperationException(
                    "更新模板卡片应答以 response_type 表达，不得同时声明 msgtype（当前 msgtype=" + message.MsgType + "）。");
            }

            if (message.TemplateCard == null)
            {
                throw new InvalidOperationException("更新模板卡片应答必须携带 template_card（官方 101031 必填）。");
            }

            return;
        }

        if (string.IsNullOrEmpty(message.MsgType))
        {
            throw new InvalidOperationException(
                "智能机器人被动回复必须声明 msgtype 或 response_type（空应答请让处理器返回 null，由分发器统一回空包）。");
        }

        switch (message.MsgType)
        {
            case WechatBotReplyTypes.Text:
                RequireBody(message.Text != null, "text");
                return;

            case WechatBotReplyTypes.TemplateCard:
                RequireBody(message.TemplateCard != null, "template_card");
                return;

            case WechatBotReplyTypes.Stream:
                RequireStream(message, requireTemplateCard: false);
                return;

            case WechatBotReplyTypes.StreamWithTemplateCard:
                RequireStream(message, requireTemplateCard: true);
                return;

            case WechatBotReplyTypes.Markdown:
                throw new InvalidOperationException(
                    "markdown 应答不被 HTTP 被动回复支持（官方 101031 无 markdown 类型）；" +
                    "请改用主动回复（response_url）或长连接回复。");

            default:
                throw new InvalidOperationException(
                    "智能机器人被动回复不支持的 msgtype：" + message.MsgType +
                    "（官方 101031 支持 text / template_card / stream / stream_with_template_card）。");
        }
    }

    private static void RequireBody(bool present, string msgType)
    {
        if (!present)
        {
            throw new InvalidOperationException(
                "智能机器人应答声明 msgtype=" + msgType + " 但未携带对应结构体（官方必填）。");
        }
    }

    private static void RequireStream(AibotMessage message, bool requireTemplateCard)
    {
        if (message.Stream == null)
        {
            throw new InvalidOperationException("流式应答声明 msgtype=" + message.MsgType + " 但未携带 stream 结构体（官方必填）。");
        }

        if (string.IsNullOrEmpty(message.Stream.Id))
        {
            throw new InvalidOperationException("流式应答必须携带 stream.id（官方：首次回复时必须设置自定义唯一 id）。");
        }

        if (requireTemplateCard && message.TemplateCard == null)
        {
            throw new InvalidOperationException(
                "stream_with_template_card 应答必须携带 template_card（官方 101031 必填）。");
        }
    }
}
