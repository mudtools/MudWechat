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

    /// <summary>
    /// 校验应答可用于<b>长连接回复</b>（<c>aibot_respond_msg</c> / <c>aibot_respond_welcome_msg</c> /
    /// <c>aibot_respond_update_msg</c>，官方 101463），不合法即抛 <see cref="InvalidOperationException"/>。
    /// </summary>
    /// <param name="message">处理器返回的应答消息。</param>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> 为 <c>null</c>。</exception>
    /// <exception cref="InvalidOperationException">应答形态不被长连接支持，或结构不自洽。</exception>
    /// <remarks>
    /// <para>
    /// 官方 101463 支持（<c>msgtype</c> 级）：<c>text</c> / <c>markdown</c> / <c>template_card</c> /
    /// <c>stream</c> / <c>file</c> / <c>image</c> / <c>voice</c> / <c>video</c>（媒体走 <c>media_id</c>，须先上传）；
    /// <c>response_type=update_template_card</c> 走 <c>aibot_respond_update_msg</c>（仅模板卡片点击事件、5 秒内）。
    /// </para>
    /// <para>
    /// <b>两处长连接特有拒绝</b>：① <c>stream_with_template_card</c> —— 官方原文「不支持『流式 + 模板卡片』组合」；
    /// ② <c>stream.msg_item</c> —— 官方原文「暂不支持 <c>msg_item</c> 字段」（回调地址模式才支持）。
    /// 流式的 <b>10 分钟窗口</b>与 <c>finish</c> 终结语义由 <c>WechatBotStreamRegistry</c> 在发送侧记账（P3）。
    /// </para>
    /// </remarks>
    public static void ValidateLongConnectionReply(AibotMessage message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // 更新模板卡片应答（aibot_respond_update_msg）：以 response_type 表达。
        if (message.ResponseType != null && message.ResponseType.Length > 0)
        {
            if (!string.Equals(message.ResponseType, WechatBotReplyTypes.UpdateTemplateCard, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "智能机器人长连接不支持的 response_type：" + message.ResponseType +
                    "（官方 101463 仅支持 " + WechatBotReplyTypes.UpdateTemplateCard + "）。");
            }

            if (message.MsgType != null && message.MsgType.Length > 0)
            {
                throw new InvalidOperationException(
                    "更新模板卡片应答以 response_type 表达，不得同时声明 msgtype（当前 msgtype=" + message.MsgType + "）。");
            }

            if (message.TemplateCard == null)
            {
                throw new InvalidOperationException("更新模板卡片应答必须携带 template_card（官方 101463 必填）。");
            }

            return;
        }

        if (string.IsNullOrEmpty(message.MsgType))
        {
            throw new InvalidOperationException(
                "智能机器人长连接回复必须声明 msgtype 或 response_type（空应答请让处理器返回 null，由连接统一跳过发送）。");
        }

        switch (message.MsgType)
        {
            case WechatBotReplyTypes.Text:
                RequireBody(message.Text != null, "text");
                return;

            case WechatBotReplyTypes.Markdown:
                RequireBody(message.Markdown != null, "markdown");
                return;

            case WechatBotReplyTypes.TemplateCard:
                RequireBody(message.TemplateCard != null, "template_card");
                return;

            case WechatBotReplyTypes.Stream:
                RequireBody(message.Stream != null, "stream");
                if (string.IsNullOrEmpty(message.Stream!.Id))
                {
                    throw new InvalidOperationException("流式应答必须携带 stream.id（官方：首次回复时必须设置自定义唯一 id）。");
                }

                if (message.Stream.MsgItem is { Count: > 0 })
                {
                    throw new InvalidOperationException(
                        "长连接流式应答暂不支持 msg_item 字段（官方 101463 原文）；该形态仅回调地址模式支持。");
                }

                return;

            case WechatBotReplyTypes.StreamWithTemplateCard:
                throw new InvalidOperationException(
                    "长连接不支持「流式 + 模板卡片」组合（官方 101463 原文）；请改用纯 stream 或纯 template_card。");

            case WechatBotReplyTypes.File:
            case WechatBotReplyTypes.Image:
            case WechatBotReplyTypes.Voice:
                RequireMediaBody(message);
                return;

            case WechatBotReplyTypes.Video:
                RequireBody(message.Video?.MediaId != null, "video");
                return;

            default:
                throw new InvalidOperationException(
                    "智能机器人长连接不支持的 msgtype：" + message.MsgType +
                    "（官方 101463 支持 text / markdown / template_card / stream / file / image / voice / video）。");
        }
    }

    /// <summary>媒体三分支（file/image/voice）按声明的 msgtype 校验对应结构体与 <c>media_id</c>（官方必填）。</summary>
    private static void RequireMediaBody(AibotMessage message)
    {
        var body = message.MsgType switch
        {
            WechatBotReplyTypes.File => message.File,
            WechatBotReplyTypes.Image => message.Image,
            _ => message.Voice,
        };

        if (body == null || body.MediaId == null || body.MediaId.Length == 0)
        {
            throw new InvalidOperationException(
                "智能机器人应答声明 msgtype=" + message.MsgType + " 但未携带对应结构体或 media_id（官方必填；media_id 须先经素材上传获得，3 天内有效）。");
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
