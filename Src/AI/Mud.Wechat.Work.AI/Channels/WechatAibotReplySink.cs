// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;
using Microsoft.Extensions.Logging;
using Mud.AI.Abstractions.Conversations;
using Mud.AI.Agent.Events;
using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.DataModels.Aibot;

namespace Mud.Wechat.Work.AI.Channels;

/// <summary>
/// 企微智能机器人主动回复通道：把回合回复文本经 <c>response_url</c> 的一次性凭据以
/// <c>markdown</c> 消息写回用户（官方 101138）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么走主动回复而不是被动回复</b>：分发软超时（默认 4500ms）远小于 LLM 生成耗时；官方对同一交互
/// 只推一次且被动回复支持面窄（<c>markdown</c> 仅主动回复 / 长连接可用）。故处理器先回空包，
/// 回合完成后由本通道写回 —— 这是官方推荐的长耗时业务形态。
/// </para>
/// <para>
/// <b>不可重试</b>：每个 <c>response_url</c> 仅可调用一次、有效期 1 小时；失败后无处补发（outbox 补发同样走本通道，
/// 每次补发消耗一个当轮凭据）。故 errcode <b>只记录不抛</b> —— 抛异常无法带来任何补偿，反而会污染回合完成路径的日志级别。
/// </para>
/// <para>
/// <b>长度上限</b>：<c>markdown.content</c> 官方上限 20480 <b>字节</b>（UTF-8），超限在此按字节安全截断并附提示，
/// 避免整条回复因超限被官方拒收。
/// </para>
/// </remarks>
public sealed class WechatAibotReplySink : IConversationReplySink
{
    /// <summary>官方 markdown 内容上限（字节，UTF-8）。</summary>
    public const int MaxMarkdownBytes = 20_480;

    private const string TruncationMarker = "\n\n…（内容过长，已截断）";

    private readonly IWechatWorkInternalAibotService _aibot;
    private readonly string _responseCode;
    private readonly ILogger? _logger;

    /// <summary>初始化回复通道。</summary>
    /// <param name="aibot">智能机器人端点（主动回复）。</param>
    /// <param name="responseCode">当轮回调的一次性应答凭据（<c>response_url</c> 的 <c>response_code</c>）。</param>
    /// <param name="logger">日志（可空；失败与截断在此可观测）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="aibot"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="ArgumentException"><paramref name="responseCode"/> 为空。</exception>
    public WechatAibotReplySink(
        IWechatWorkInternalAibotService aibot,
        string responseCode,
        ILogger? logger = null)
    {
        _aibot = aibot ?? throw new ArgumentNullException(nameof(aibot));
        if (string.IsNullOrWhiteSpace(responseCode))
        {
            throw new ArgumentException("主动回复凭据 response_code 不能为空。", nameof(responseCode));
        }

        _responseCode = responseCode;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task DeliverAsync(
        ConversationRequest request,
        string responseText,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(responseText))
        {
            return;
        }

        var content = FitMarkdown(responseText, MaxMarkdownBytes, out var truncated);
        if (truncated)
        {
            _logger?.LogWarning(
                "回复文本超过 {Max} 字节已截断（appKey={AppKey}, messageId={MessageId}）",
                MaxMarkdownBytes,
                request.AppKey,
                request.MessageId);
        }

        var message = new AibotMessage
        {
            MsgType = WechatBotReplyTypes.Markdown,
            Markdown = new AibotMarkdownBody { Content = content },
        };

        var response = await _aibot.ReplyAsync(_responseCode, message, cancellationToken).ConfigureAwait(false);

        // 官方未列响应字段（可能为空响应体）：仅在有 errcode 且非 0 时告警。
        if (response is { IsSuccess: false })
        {
            _logger?.LogError(
                "主动回复失败（不可重试）：errcode={ErrorCode}, errmsg={ErrorMessage}, appKey={AppKey}, messageId={MessageId}",
                response.ErrorCode,
                response.ErrorMessage,
                request.AppKey,
                request.MessageId);
        }
    }

    /// <summary>
    /// 按 UTF-8 字节上限裁剪 markdown 文本（不切坏代理对；截断时预算里预留提示文案的字节）。
    /// </summary>
    /// <param name="text">原文（非空）。</param>
    /// <param name="maxBytes">字节上限。</param>
    /// <param name="truncated">是否发生截断。</param>
    /// <returns>不超上限的文本。</returns>
    public static string FitMarkdown(string text, int maxBytes, out bool truncated)
    {
        truncated = false;
        if (text is null || maxBytes <= 0)
        {
            return text ?? string.Empty;
        }

        if (Encoding.UTF8.GetByteCount(text) <= maxBytes)
        {
            return text;
        }

        truncated = true;
        var markerBytes = Encoding.UTF8.GetByteCount(TruncationMarker);
        var budget = maxBytes - markerBytes;
        if (budget <= 0)
        {
            // 上限过小：仅保留提示文案的可裁剪前缀（保守退化，不为负数长度分配缓冲）。
            return TruncationMarker.Length <= maxBytes ? TruncationMarker : string.Empty;
        }

        // 二分求「字节数 ≤ budget」的最大字符长度（按 char 计，落在代理对中间时回退一位）。
        var low = 0;
        var high = text.Length;
        while (low < high)
        {
            var mid = low + ((high - low + 1) >> 1);
            // 注：Encoding.GetByteCount(string, int, int) 在 netstandard2.0 不存在，退化为切片重载。
#if NETSTANDARD2_0
            if (Encoding.UTF8.GetByteCount(text.Substring(0, mid)) <= budget)
#else
            if (Encoding.UTF8.GetByteCount(text, 0, mid) <= budget)
#endif
            {
                low = mid;
            }
            else
            {
                high = mid - 1;
            }
        }

        var length = low;
        if (length > 0 && char.IsHighSurrogate(text[length - 1]))
        {
            length--;
        }

        return string.Concat(text.Substring(0, length).TrimEnd(), TruncationMarker);
    }
}
