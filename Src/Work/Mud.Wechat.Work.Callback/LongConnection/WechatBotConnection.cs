// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER

using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.DataModels.Aibot;
using Mud.Wechat.Work.DataModels.Message;

namespace Mud.Wechat.Work.Callback.LongConnection;

/// <summary>
/// 单个智能机器人的<b>长连接</b>（官方 101463：<c>wss://openws.work.weixin.qq.com</c>，
/// 建连后 <c>aibot_subscribe</c> 鉴权，随后收发 JSON 帧）。
/// </summary>
/// <remarks>
/// <para>
/// <b>生命周期</b>（<see cref="RunAsync"/> 主循环）：夺租约（若注册了 <see cref="IWechatBotConnectionLease"/>）→
/// 建连 → <c>aibot_subscribe</c>（<b>每条连接恰一次</b>，官方有频率保护）→ 读帧循环 →
/// 断开（含被新连接踢下线的 <c>disconnected_event</c>）→ <b>指数退避</b> → 回到夺租约。
/// </para>
/// <para>
/// <b>失败顺序即契约（对齐 ADS-B3 的先删后抛纪律）</b>：重连<b>不得</b>直接建连 —— 必须先退避、再夺租约、
/// 夺到才建连（防主备乒乓）；失去租约（续租失败）时立即放弃当前连接并回到夺取流程。
/// </para>
/// <para>
/// <b>无指纹闸</b>：长连接帧无 query 签名，<c>msgid</c> 去重与业务幂等归宿主（信封暴露 <see cref="WechatBotCallbackEvent.MsgId"/>，
/// R10）。应答经 <see cref="WechatBotReplySupport.ValidateLongConnectionReply"/> 发送前 fail-fast（R13）。
/// </para>
/// </remarks>
internal sealed class WechatBotConnection
{
    private const string LongConnectionHost = "wss://openws.work.weixin.qq.com";

    /// <summary>应答帧等待 ack 的超时（官方未给时长；取心跳周期量级，超时即视为本次发送失败——官方按 errcode 表达失败）。</summary>
    private static readonly TimeSpan AckTimeout = TimeSpan.FromSeconds(10);

    private readonly string _botKey;
    private readonly WechatBotAppOptions _app;
    private readonly WechatBotEventDispatcher _dispatcher;
    private readonly IWechatBotConnectionLease? _lease;
    private readonly Func<ClientWebSocket> _webSocketFactory;
    private readonly WechatBotStreamRegistry _streams;
    private readonly string _instanceId;
    private readonly ILogger _logger;

    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private readonly ConcurrentDictionary<string, TaskCompletionSource<AibotFrameAck>> _pendingAcks = new(StringComparer.Ordinal);

    private ClientWebSocket? _socket;
    private volatile bool _disposed;

    /// <summary>创建单机器人长连接。</summary>
    /// <param name="botKey">机器人键（配置面与租约的分槽键）。</param>
    /// <param name="app">该机器人的长连接配置（<c>BotId</c> / <c>BotSecret</c> / 保活参数）。</param>
    /// <param name="dispatcher">bot 事件分发内核（与 HTTP 回调共用同一实例，ADR-8）。</param>
    /// <param name="lease">连接租约（<c>null</c> = 单实例部署，不做主备；多实例部署必须提供）。</param>
    /// <param name="webSocketFactory">WebSocket 工厂缝（测试注入）。</param>
    /// <param name="instanceId">实例标识（租约持有者凭据）。</param>
    /// <param name="logger">日志器。</param>
    public WechatBotConnection(
        string botKey,
        WechatBotAppOptions app,
        WechatBotEventDispatcher dispatcher,
        IWechatBotConnectionLease? lease,
        Func<ClientWebSocket> webSocketFactory,
        string instanceId,
        ILogger logger)
    {
        _botKey = botKey ?? throw new ArgumentNullException(nameof(botKey));
        _app = app ?? throw new ArgumentNullException(nameof(app));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _lease = lease;
        _webSocketFactory = webSocketFactory ?? throw new ArgumentNullException(nameof(webSocketFactory));
        _instanceId = instanceId;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _streams = new WechatBotStreamRegistry();
    }

    /// <summary>本连接的机器人键。</summary>
    public string BotKey => _botKey;

    /// <summary>
    /// 长连接主循环：夺租约 → 建连 → 订阅 → 读帧 → 退避重连，直到 <paramref name="lifetime"/> 取消。
    /// </summary>
    /// <remarks>
    /// 任何一轮失败都不抛出（长连接的语义是「尽力维持」）；取消令牌触发后<b>先释放租约再退出</b>
    /// （优雅停机即让位，避免等 TTL 自然过期）。
    /// </remarks>
    public async Task RunAsync(CancellationToken lifetime)
    {
        var attempt = 0;
        while (!lifetime.IsCancellationRequested && !_disposed)
        {
            var connected = false;
            try
            {
                if (_lease != null)
                {
                    if (!await _lease.TryAcquireAsync(_botKey, _instanceId, LeaseTtl(), lifetime).ConfigureAwait(false))
                    {
                        // 未夺得租约（另一实例持有）：按退避节奏重试，不得建连（否则与持有者乒乓）。
                        await DelayBeforeRetry(attempt++, lifetime).ConfigureAwait(false);
                        continue;
                    }
                }

                await RunSessionAsync(lifetime).ConfigureAwait(false);
                connected = true;
            }
            catch (OperationCanceledException) when (lifetime.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "智能机器人 {BotKey} 长连接会话异常，将退避重连。", _botKey);
            }
            finally
            {
                if (!connected)
                {
                    attempt++;
                }
            }

            // 会话结束（含被踢）：退避后重走夺租约（disconnected_event 语义 —— 不得立即抢占）。
            await DelayBeforeRetry(attempt++, lifetime).ConfigureAwait(false);
        }

        if (_lease != null)
        {
            try
            {
                await _lease.ReleaseAsync(_botKey, _instanceId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "智能机器人 {BotKey} 停机释放租约失败（等 TTL 自然过期即可）。", _botKey);
            }
        }
    }

    /// <summary>
    /// 主动推送消息（<c>aibot_send_msg</c>；官方要求用户在会话中先发过消息）。
    /// </summary>
    /// <param name="chatId">会话 id（单聊时填 userid）。</param>
    /// <param name="chatType">会话类型（1 单聊 / 2 群聊 / 0 或 <c>null</c> 兼容模式）。</param>
    /// <param name="message">推送内容（<c>msgtype</c> 支持 template_card / markdown / file / image / voice / video）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <exception cref="InvalidOperationException">连接未在线，或应答形态不被长连接支持（R13 fail-fast）。</exception>
    public async Task SendAsync(
        string chatId, int? chatType, AibotMessage message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(chatId))
        {
            throw new ArgumentException("会话 id 不能为空（官方 chatid；单聊时填 userid）。", nameof(chatId));
        }

        WechatBotReplySupport.ValidateLongConnectionReply(message);

        var body = new AibotSendMessageBody
        {
            ChatId = chatId,
            ChatType = chatType,
            MsgType = message.MsgType,
            Text = message.Text,
            Markdown = message.Markdown,
            TemplateCard = message.TemplateCard,
            File = message.File,
            Image = message.Image,
            Voice = message.Voice,
            Video = message.Video,
        };

        await SendWithAckAsync(
            WechatBotFrameCommands.SendMsg,
            WechatBotFrameCodec.NewReqId(),
            body,
            AibotJsonContext.Default.AibotSendMessageBody,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 宿主主动刷新流式应答（长连接模式：开发者主动推、同 <c>stream.id</c> 刷新、10 分钟窗口内 <c>finish</c>）。
    /// </summary>
    /// <param name="streamId">官方 <c>stream.id</c>（与首次回复一致）。</param>
    /// <param name="content">累计全量内容（官方：首次回 "1"、第二次回 "123"，展示 "123"）。</param>
    /// <param name="finish">是否终结本流（终结后不得再刷）。</param>
    /// <param name="feedbackId">反馈按钮 id（可选）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <exception cref="InvalidOperationException">流不存在（未从任何回调应答中首次发送）、已终结或超窗。</exception>
    /// <remarks>
    /// 本方法把刷新帧的 <c>req_id</c> 钉为<b>首次发送时</b>的 req_id（官方原文「同一次回调的所有流式回复
    /// 须用相同 req_id」）—— 记账在 <see cref="WechatBotStreamRegistry"/>。
    /// </remarks>
    public async Task RefreshStreamAsync(
        string streamId, string? content, bool finish, string? feedbackId = null, CancellationToken cancellationToken = default)
    {
        // 首次刷新若在册则复用首次 req_id；不在册（宿主跳过首次直接刷）按新流创建。
        var existing = _streams.TryGetReqId(streamId);
        var reqId = existing ?? WechatBotFrameCodec.NewReqId();

        var body = new AibotMessage
        {
            MsgType = WechatBotReplyTypes.Stream,
            Stream = new AibotStreamBody
            {
                Id = streamId,
                Finish = finish,
                Content = content,
                Feedback = feedbackId == null ? null : new MessageFeedbackBody { Id = feedbackId },
            },
        };

        var effectiveReqId = _streams.BeginOrRefresh(streamId, reqId, finish);
        await SendRespondCoreAsync(WechatBotFrameCommands.RespondMsg, effectiveReqId, body, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>上传临时素材（官方三步帧：<c>init</c> → <c>chunk</c>×N → <c>finish</c>），返回 3 天内有效的 <c>media_id</c>。</summary>
    /// <param name="mediaType">素材类型（<c>file</c> / <c>image</c> / <c>voice</c> / <c>video</c>）。</param>
    /// <param name="filename">文件名（官方 <c>filename</c>）。</param>
    /// <param name="content">素材内容流（<b>不</b>缓冲整体；分片按 512KB（base64 前）读取）。</param>
    /// <param name="md5">文件 MD5（官方选填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>官方 <c>media_id</c>（3 天内有效）。</returns>
    /// <remarks>
    /// 官方约束：单分片 ≤512KB（base64 前）、≤100 片、会话 30 分钟、分片可乱序（本实现按序发送）、
    /// 同分片重复上传幂等忽略；上传频控 30 次/分、1000 次/时。
    /// </remarks>
    public async Task<string> UploadMediaAsync(
        string mediaType, string filename, Stream content, string? md5 = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(mediaType))
        {
            throw new ArgumentException("素材类型不能为空。", nameof(mediaType));
        }

        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentException("文件名不能为空。", nameof(filename));
        }

        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        // ① init：总大小与分片数。
        var totalSize = content.Length - content.Position;
        const int chunkSize = 512 * 1024;
        var totalChunks = (int)((totalSize + chunkSize - 1) / chunkSize);
        if (totalSize == 0)
        {
            throw new InvalidOperationException("素材上传的 内容为空（官方 total_size 最少 5 字节）。");
        }

        var initAck = await SendWithAckAsync(
            WechatBotFrameCommands.UploadMediaInit,
            WechatBotFrameCodec.NewReqId(),
            new AibotUploadMediaInitBody
            {
                Type = mediaType,
                Filename = filename,
                TotalSize = totalSize,
                TotalChunks = totalChunks,
                Md5 = md5,
            },
            AibotJsonContext.Default.AibotUploadMediaInitBody,
            cancellationToken).ConfigureAwait(false);

        var uploadId = initAck.Body == null
            ? null
            : WechatBotFrameCodec.ParseBody(initAck.Body.Value, AibotJsonContext.Default.AibotUploadMediaInitAck)?.UploadId;
        if (string.IsNullOrEmpty(uploadId))
        {
            throw new InvalidOperationException($"智能机器人 {mediaType} 上传初始化应答缺少 upload_id（官方 101463 必回）。");
        }

        // ② chunk：按序分片（官方允许乱序与幂等重传；base64 由 Utf8JsonWriter 自动转义承载）。
        var buffer = new byte[chunkSize];
        long index = 0;
        while (content.Position < content.Length)
        {
            var read = 0;
            while (read < buffer.Length)
            {
                var n = await content.ReadAsync(buffer.AsMemory(read, buffer.Length - read), cancellationToken).ConfigureAwait(false);
                if (n == 0)
                {
                    break;
                }

                read += n;
            }

            if (read == 0)
            {
                break;
            }

            var chunkAck = await SendWithAckAsync(
                WechatBotFrameCommands.UploadMediaChunk,
                WechatBotFrameCodec.NewReqId(),
                new AibotUploadMediaChunkBody
                {
                    UploadId = uploadId,
                    ChunkIndex = index,
                    Base64Data = Convert.ToBase64String(buffer, 0, read),
                },
                AibotJsonContext.Default.AibotUploadMediaChunkBody,
                cancellationToken).ConfigureAwait(false);
            EnsureAckSuccess(WechatBotFrameCommands.UploadMediaChunk, index, chunkAck);
            index++;
        }

        // ③ finish：取 media_id。
        var finishAck = await SendWithAckAsync(
            WechatBotFrameCommands.UploadMediaFinish,
            WechatBotFrameCodec.NewReqId(),
            new AibotUploadMediaFinishBody { UploadId = uploadId },
            AibotJsonContext.Default.AibotUploadMediaFinishBody,
            cancellationToken).ConfigureAwait(false);

        var media = finishAck.Body == null
            ? null
            : WechatBotFrameCodec.ParseBody(finishAck.Body.Value, AibotJsonContext.Default.AibotUploadMediaFinishAck);
        if (string.IsNullOrEmpty(media?.MediaId))
        {
            throw new InvalidOperationException($"智能机器人 {mediaType} 上传完成应答缺少 media_id（官方 101463 必回）。");
        }

        return media.MediaId;
    }

    private async Task RunSessionAsync(CancellationToken lifetime)
    {
        using var socket = _webSocketFactory();
        _socket = socket;
        try
        {
            await socket.ConnectAsync(new Uri(LongConnectionHost), lifetime).ConfigureAwait(false);
            await SubscribeAsync(lifetime).ConfigureAwait(false);
            _logger.LogInformation("智能机器人 {BotKey} 长连接已订阅（{Host}）。", _botKey, LongConnectionHost);

            await ReadLoopAsync(socket, lifetime).ConfigureAwait(false);
        }
        finally
        {
            _socket = null;
        }
    }

    /// <summary>订阅帧（每条连接恰一次；官方有频率保护，成功后禁止反复订阅）。</summary>
    private async Task SubscribeAsync(CancellationToken lifetime)
    {
        var reqId = WechatBotFrameCodec.NewReqId();
        var ack = await SendWithAckCoreAsync(
            WechatBotFrameCommands.Subscribe,
            reqId,
            new AibotSubscribeBody { BotId = _app.BotId, Secret = _app.BotSecret },
            AibotJsonContext.Default.AibotSubscribeBody,
            lifetime).ConfigureAwait(false);

        if (!ack.IsSuccess)
        {
            // 凭据错误等失败：不吞——抛出让主循环退避重试（订阅失败即无回调整条链路）。
            throw new InvalidOperationException(
                $"智能机器人 {_botKey} 订阅失败：errcode={ack.ErrCode}, errmsg={ack.ErrMsg}。");
        }
    }

    /// <summary>读帧循环：ack 关联 / 回调分派；直到连接关闭或取消。</summary>
    private async Task ReadLoopAsync(ClientWebSocket socket, CancellationToken lifetime)
    {
        var buffer = new byte[64 * 1024];
        using var pingTimer = new CancellationTokenSource();
        var heartbeat = HeartbeatAsync(socket, pingTimer.Token);

        try
        {
            while (socket.State == WebSocketState.Open && !lifetime.IsCancellationRequested)
            {
                var message = await ReceiveTextAsync(socket, buffer, lifetime).ConfigureAwait(false);
                if (message == null)
                {
                    break;
                }

                pingTimer.CancelAfter(TimeSpan.FromSeconds(EffectivePingIntervalSeconds));

                var frame = WechatBotFrameCodec.ParseFrame(message);
                if (frame.Cmd == WechatBotFrameCommands.MsgCallback || frame.Cmd == WechatBotFrameCommands.EventCallback)
                {
                    await DispatchCallbackFrameAsync(frame).ConfigureAwait(false);
                }
                else if (frame.Headers?.ReqId is { Length: > 0 } reqId
                         && _pendingAcks.TryRemove(reqId, out var pending))
                {
                    var ack = WechatBotFrameCodec.ParseAck(message);
                    pending.TrySetResult(ack);
                }
                else
                {
                    _logger.LogDebug("智能机器人 {BotKey} 忽略未知帧 cmd={Cmd}。", _botKey, frame.Cmd);
                }
            }
        }
        finally
        {
            pingTimer.Cancel();
            await heartbeat.ConfigureAwait(false);
            foreach (var pending in _pendingAcks.Values)
            {
                pending.TrySetCanceled();
            }

            _pendingAcks.Clear();
        }
    }

    /// <summary>心跳：按配置周期发 <c>ping</c>（官方建议 30 秒；长时间未收到心跳服务端主动断开）。</summary>
    private async Task HeartbeatAsync(ClientWebSocket socket, CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested && socket.State == WebSocketState.Open)
            {
                await Task.Delay(TimeSpan.FromSeconds(EffectivePingIntervalSeconds), token).ConfigureAwait(false);
                if (socket.State != WebSocketState.Open)
                {
                    return;
                }

                await SendFrameAsync(
                    WechatBotFrameCommands.Ping,
                    WechatBotFrameCodec.NewReqId(),
                    bodyJson: null,
                    token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // 正常取消（读循环退出）。
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "智能机器人 {BotKey} 心跳发送失败（连接将被服务端或读循环关闭）。", _botKey);
        }
    }

    /// <summary>回调帧分派：两阶段解 body → 建信封 → 复用 HTTP 侧分发内核 → 应答帧回写。</summary>
    private async Task DispatchCallbackFrameAsync(AibotFrame frame)
    {
        var bodyJson = frame.Body?.GetRawText() ?? string.Empty;
        WechatBotCallbackEvent botEvent;
        try
        {
            botEvent = WechatBotCallbackReceiver.ParseEvent(_botKey, bodyJson);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "智能机器人 {BotKey} 长连接帧体解析失败，已丢弃该帧（无重推机制，宿主侧幂等自担）。", _botKey);
            return;
        }

        var reqId = frame.Headers?.ReqId ?? string.Empty;

        // 长连接的流式应答记账在「首次发送」时建立 —— 这里先把本次回调帧的 req_id 预登记给
        // 处理器可能返回的 stream.id（分发结果拿到 reply 后再按 msgtype 精确记账）。
        WechatBotDispatchResult result;
        try
        {
            result = await _dispatcher.DispatchAsync(_botKey, botEvent).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // 软超时（EventHandlingTimeoutMs）：长连接侧无 503 重推语义（官方只推一次）—— 记录后丢弃。
            _logger.LogError("智能机器人 {BotKey} 帧分派软超时（req_id={ReqId}, msgid={MsgId}），已丢弃（官方无重推）。", _botKey, reqId, botEvent.MsgId);
            return;
        }

        if (result.Outcome != WechatBotDispatchOutcome.Handled || result.Reply == null)
        {
            return;
        }

        var reply = result.Reply;
        var cmd = ResolveRespondCommand(botEvent, reply);
        if (cmd == null)
        {
            _logger.LogWarning(
                "智能机器人 {BotKey} 处理器返回的应答无法映射到任何 respond 帧（事件={Event}, msgtype={MsgType}），已丢弃。", _botKey, botEvent.EventType, reply.MsgType);
            return;
        }

        // 流式应答的「首次发送」在此记账（官方 101463：同一次回调的所有流式回复须用相同 req_id）——
        // 之后宿主经 RefreshStreamAsync 刷新时复用本 req_id。
        var effectiveReqId = reqId;
        if (reply.MsgType == WechatBotReplyTypes.Stream && !string.IsNullOrEmpty(reply.Stream?.Id))
        {
            effectiveReqId = _streams.BeginOrRefresh(reply.Stream.Id, reqId, reply.Stream.Finish == true);
        }

        try
        {
            await SendRespondCoreAsync(cmd, effectiveReqId, reply, CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // 应答失败不重试（官方 respond 帧按 errcode 表达失败、welcome/update 有 5s 窗口，重试必超窗）。
            _logger.LogError(ex, "智能机器人 {BotKey} 应答帧发送失败（cmd={Cmd}, req_id={ReqId}）。", _botKey, cmd, effectiveReqId);
        }
    }

    /// <summary>按事件与应答形态映射 respond 帧命令（官方 101463 三支应答帧各有事件限定）。</summary>
    private static string? ResolveRespondCommand(WechatBotCallbackEvent botEvent, AibotMessage reply)
    {
        if (reply.ResponseType == WechatBotReplyTypes.UpdateTemplateCard)
        {
            // 模板卡片更新仅限模板卡片点击事件（官方 5 秒内回复）。
            return botEvent.EventType == WechatBotEventTypes.TemplateCardEvent
                ? WechatBotFrameCommands.RespondUpdateMsg
                : null;
        }

        if (botEvent.EventType == WechatBotEventTypes.EnterChat && reply.MsgType == WechatBotReplyTypes.Text)
        {
            return WechatBotFrameCommands.RespondWelcomeMsg;
        }

        return WechatBotFrameCommands.RespondMsg;
    }

    private async Task SendRespondCoreAsync(string cmd, string reqId, AibotMessage reply, CancellationToken cancellationToken)
    {
        WechatBotReplySupport.ValidateLongConnectionReply(reply);

        var socket = _socket;
        if (socket == null || socket.State != WebSocketState.Open)
        {
            throw new InvalidOperationException($"智能机器人 {_botKey} 长连接未在线，无法发送 {cmd}。");
        }

        // 应答帧体即应答超集（welcome/update/msg 三支同形态）；ack 无承载语义（errcode 表达成败）。
        var ack = await SendWithAckCoreAsync(cmd, reqId, reply, AibotJsonContext.Default.AibotMessage, cancellationToken)
            .ConfigureAwait(false);
        EnsureAckSuccess(cmd, reqId, ack);
    }

    private async Task<AibotFrameAck> SendWithAckAsync<TBody>(
        string cmd, string reqId, TBody body,
        System.Text.Json.Serialization.Metadata.JsonTypeInfo<TBody> typeInfo,
        CancellationToken cancellationToken)
        where TBody : class
    {
        var ack = await SendWithAckCoreAsync(cmd, reqId, body, typeInfo, cancellationToken).ConfigureAwait(false);
        EnsureAckSuccess(cmd, reqId, ack);
        return ack;
    }

    /// <summary>发帧并等待关联 ack（req_id 关联；应答帧统一形态）。</summary>
    private async Task<AibotFrameAck> SendWithAckCoreAsync<TBody>(
        string cmd, string reqId, TBody body,
        System.Text.Json.Serialization.Metadata.JsonTypeInfo<TBody> typeInfo,
        CancellationToken cancellationToken)
        where TBody : class
    {
        var json = WechatBotFrameCodec.EncodeFrame(cmd, reqId, body, typeInfo);

        var pending = new TaskCompletionSource<AibotFrameAck>(TaskCreationOptions.RunContinuationsAsynchronously);
        _pendingAcks[reqId] = pending;
        try
        {
            await SendFrameAsync(cmd, reqId, json, cancellationToken).ConfigureAwait(false);

            var completed = await Task.WhenAny(pending.Task, Task.Delay(AckTimeout, cancellationToken)).ConfigureAwait(false);
            if (completed != pending.Task)
            {
                throw new TimeoutException($"智能机器人 {_botKey} 的 {cmd} 帧（req_id={reqId}）在 {AckTimeout.TotalSeconds}s 内未收到应答。");
            }

            return await pending.Task.ConfigureAwait(false);
        }
        finally
        {
            _pendingAcks.TryRemove(reqId, out _);
        }
    }

    private async Task SendFrameAsync(string cmd, string reqId, string? bodyJson, CancellationToken cancellationToken)
    {
        var socket = _socket;
        if (socket == null || socket.State != WebSocketState.Open)
        {
            throw new InvalidOperationException($"智能机器人 {_botKey} 长连接未在线（状态 {_socket?.State.ToString() ?? "无"}），无法发送 {cmd}。");
        }

        var bytes = Encoding.UTF8.GetBytes(bodyJson ?? $"{{\"cmd\":\"{cmd}\",\"headers\":{{\"req_id\":\"{reqId}\"}}}}");
        await _sendLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await socket.SendAsync(bytes, WebSocketMessageType.Text, endOfMessage: true, cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            _sendLock.Release();
        }
    }

    private static async Task<string?> ReceiveTextAsync(ClientWebSocket socket, byte[] buffer, CancellationToken cancellationToken)
    {
        using var message = new MemoryStream();
        while (true)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken).ConfigureAwait(false);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                return null;
            }

            message.Write(buffer, 0, result.Count);
            if (result.EndOfMessage)
            {
                return Encoding.UTF8.GetString(message.ToArray());
            }
        }
    }

    private async Task DelayBeforeRetry(int attempt, CancellationToken lifetime)
    {
        var baseMs = EffectiveReconnectBaseDelayMs;
        var maxMs = EffectiveReconnectMaxDelayMs;
        var delay = Math.Min((long)baseMs << Math.Min(attempt, 20), maxMs);
        _logger.LogInformation("智能机器人 {BotKey} 将在 {Delay}ms 后重试（第 {Attempt} 次退避）。", _botKey, delay, attempt + 1);
        await Task.Delay(TimeSpan.FromMilliseconds(delay), lifetime).ConfigureAwait(false);
    }

    /// <summary>实际生效的心跳间隔（秒）：机器人配置 > 0 取配置，否则官方建议值 30。</summary>
    private int EffectivePingIntervalSeconds
        => _app.PingIntervalSeconds > 0 ? _app.PingIntervalSeconds : WechatBotAppOptions.DefaultPingIntervalSeconds;

    /// <summary>实际生效的重连基础退避（毫秒）：机器人配置 > 0 取配置，否则默认 1000。</summary>
    private int EffectiveReconnectBaseDelayMs
        => _app.ReconnectBaseDelayMs > 0 ? _app.ReconnectBaseDelayMs : WechatBotAppOptions.DefaultReconnectBaseDelayMs;

    /// <summary>实际生效的重连退避上限（毫秒）：机器人配置 > 0 取配置，否则默认 30000。</summary>
    private int EffectiveReconnectMaxDelayMs
        => _app.ReconnectMaxDelayMs > 0 ? _app.ReconnectMaxDelayMs : WechatBotAppOptions.DefaultReconnectMaxDelayMs;

    private static TimeSpan LeaseTtl() => TimeSpan.FromSeconds(45);

    private static void EnsureAckSuccess(string cmd, object context, AibotFrameAck? ack)
    {
        if (ack != null && !ack.IsSuccess)
        {
            throw new InvalidOperationException($"智能机器人长连接 {cmd} 被官方拒绝：errcode={ack.ErrCode}, errmsg={ack.ErrMsg}。");
        }
    }
}

#endif
