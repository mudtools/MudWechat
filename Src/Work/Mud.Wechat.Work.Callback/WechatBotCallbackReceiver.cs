// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback.Bots;

#if NET8_0_OR_GREATER
using System.Globalization;
using System.Text.Json;
using Mud.Wechat.Work.DataModels.Aibot;
#endif

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 智能机器人回调接收接口（JSON 报文：<c>{"encrypt":"..."}</c>，官方 101033）。
/// </summary>
/// <remarks>
/// <para>
/// 与 XML 回调接收器的差异仅在<b>报文格式与信封</b>：验签（<c>msg_signature</c>）、时效窗口（±300s）、
/// AES 解密（同 <c>EncodingAESKey</c>）、一次性指纹闸（解密成功之后消费）全部<b>同族同算法</b>。
/// </para>
/// <para>
/// GET URL 验证与 XML 侧完全一致（101033 与 90968 同协议），故 <b>GET echo 复用
/// <see cref="IWechatCallbackReceiver.EchoAsync"/></b>，本接口只承载 POST 接收。
/// </para>
/// <para>
/// <b>目标框架门控</b>：报文反序列化依赖源生成 JSON 上下文，而本仓生成上下文位于
/// <c>#if NET8_0_OR_GREATER</c>（<c>DataModels/Generated/</c>）⇒ 智能机器人回调通道仅
/// <c>net8.0</c> / <c>net10.0</c> 可用；低目标框架经 <see cref="IsSupported"/> 显式上报不可用，
/// 中间件据此对 JSON 请求回 415（<b>不静默降级</b>）。
/// </para>
/// </remarks>
public interface IWechatBotCallbackReceiver
{
    /// <summary>当前目标框架是否支持智能机器人 JSON 回调。</summary>
    bool IsSupported { get; }

    /// <summary>
    /// 接收并校验智能机器人回调（验签 → 时效窗口 → AES 解密 → 空 receiveid 校验 → 一次性指纹 → JSON 反序列化）。
    /// </summary>
    /// <param name="botKey">回调配置键（路由路径段；凭据按其解析，精确键优先回退通配）。</param>
    /// <param name="urlQuery">回调 URL 查询串（含 <c>msg_signature</c> / <c>timestamp</c> / <c>nonce</c>）。</param>
    /// <param name="body">回调请求体（JSON：<c>{"encrypt":"..."}</c>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>智能机器人回调信封（含强类型载荷）。</returns>
    /// <exception cref="WechatCallbackException">验签 / 时效 / 解密 / receiveid 校验失败或报文非法时抛出
    /// （该类型继承 <see cref="InvalidOperationException"/>，中间件统一映射 403）。</exception>
    Task<WechatBotCallbackEvent> ReceiveAsync(
        string botKey, string urlQuery, string body, CancellationToken cancellationToken = default);
}

#if NET8_0_OR_GREATER

/// <summary>
/// 智能机器人回调接收器默认实现（时序与失败面完全对齐 XML 侧接收器）。
/// </summary>
/// <remarks>
/// <b>指纹闸位置</b>：位于「AES 解密 + receiveid 校验成功」之后、信封返回之前 —— 解密成功即证明报文
/// 经仅企微与我方共知的 AESKey 验证可信；解密失败不消耗指纹，官方重试可重新进入管线
/// （与 XML 侧同一条 fail-closed 语义，见 AGENTS.md §5.5）。
/// </remarks>
internal sealed class WechatBotCallbackReceiver : IWechatBotCallbackReceiver
{
    private readonly IOptionsMonitor<WechatCallbackOptions> _optionsMonitor;
    private readonly IWechatCallbackReplayGuard _replayGuard;
    private readonly Func<DateTimeOffset> _utcNow;
    private readonly ILogger<WechatBotCallbackReceiver>? _logger;

    /// <summary>创建智能机器人回调接收器。</summary>
    /// <param name="optionsMonitor">回调配置监视器（多机器人凭据热更新）。</param>
    /// <param name="replayGuard">一次性去重守卫（可选；缺省为进程内实现）。</param>
    /// <param name="utcNow">时钟源（可选，默认 <see cref="DateTimeOffset.UtcNow"/>；便于测试）。</param>
    /// <param name="logger">日志器（可选）。</param>
    public WechatBotCallbackReceiver(
        IOptionsMonitor<WechatCallbackOptions> optionsMonitor,
        IWechatCallbackReplayGuard? replayGuard = null,
        Func<DateTimeOffset>? utcNow = null,
        ILogger<WechatBotCallbackReceiver>? logger = null)
    {
        _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        _replayGuard = replayGuard ?? new InMemoryWechatCallbackReplayGuard();
        _utcNow = utcNow ?? (() => DateTimeOffset.UtcNow);
        _logger = logger;
    }

    /// <inheritdoc />
    public bool IsSupported => true;

    /// <inheritdoc />
    public async Task<WechatBotCallbackEvent> ReceiveAsync(
        string botKey, string urlQuery, string body, CancellationToken cancellationToken = default)
    {
        var app = ResolveApp(botKey);
        var (signature, timestamp, nonce) = WechatCallbackCrypto.ParseSignatureQuery(urlQuery);
        if (string.IsNullOrEmpty(signature))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingSignature, "智能机器人回调验签失败：缺少 msg_signature 参数。");
        }

        var encrypt = ExtractEncrypt(body)
            ?? throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingEncrypt, "智能机器人回调报文非法：未找到 encrypt 字段。");

        // 验签比对与后续指纹共用同一次 SHA1 计算结果（与 XML 侧 P2-3 同款）。
        var fingerprint = WechatCallbackCrypto
            .ComputeSignature(app.PushToken, timestamp ?? string.Empty, nonce ?? string.Empty, encrypt);
        if (!string.Equals(fingerprint, signature, StringComparison.OrdinalIgnoreCase))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.InvalidSignature,
                "智能机器人回调验签失败：msg_signature 不匹配（请检查 PushToken 配置）。");
        }

        ValidateTimestampWindow(timestamp);
        if (string.IsNullOrEmpty(nonce))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingNonce, "智能机器人回调验签失败：nonce 缺失。");
        }

        var decrypted = WechatCallbackCrypto.Decrypt(app.PushEncodingAESKey, encrypt, out var receiveId);

        // 官方 101033：企业内部智能机器人场景 receiveid 恒为空字符串 ⇒ 非空即拒绝（fail-closed）。
        if (!string.IsNullOrEmpty(receiveId))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.ReceiveIdMismatch,
                "智能机器人回调验签失败：明文 receiveid 必须为空字符串（官方 101033）。");
        }

        if (decrypted == null || decrypted.Length == 0)
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingEncrypt, "智能机器人回调报文非法：解密明文为空。");
        }

        if (!await _replayGuard
                .TryMarkAsync(fingerprint, TimeSpan.FromSeconds(WechatCallbackReceiver.ReplayWindowSeconds), cancellationToken)
                .ConfigureAwait(false))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.ReplaySuspected, "智能机器人回调验签失败：报文已处理过（疑似重放）。");
        }

        return ParseEvent(botKey, decrypted, message => _logger?.LogWarning(
            "智能机器人回调报文缺少 msgid（配置键 {BotKey}）：{Message}", botKey, message));
    }

    /// <summary>解析指定回调配置键的凭据（精确键优先，回退通配键）；未命中或凭据非法即抛出。</summary>
    private WechatAppCallbackOptions ResolveApp(string botKey)
    {
        var options = _optionsMonitor.CurrentValue;
        var app = options.ResolveApp(botKey)
            ?? throw new WechatCallbackException(
                WechatCallbackFailureKind.UnknownReceiver,
                $"回调配置未命中 \"{botKey}\"（既无精确键也无通配 \"{WechatCallbackOptions.WildcardAppKey}\" 键）。");

        app.Validate(botKey);

        // 智能机器人通道校验：JSON 报文只可能来自 Bot 条目（App/Suite 条目为加密 XML）。
        if (app.Channel != WechatCallbackChannel.Bot)
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.UnknownReceiver,
                $"回调配置 \"{botKey}\" 的 Channel 必须为 Bot 才能接收智能机器人 JSON 回调（当前 {app.Channel}）。");
        }

        return app;
    }

    /// <summary>从 JSON 报文体提取 <c>encrypt</c> 字段（官方 101033：<c>{"encrypt":"msg_encrypt"}</c>）。</summary>
    private static string? ExtractEncrypt(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            var envelope = JsonSerializer.Deserialize(body, AibotJsonContext.Default.AibotEncryptedEnvelope);
            return string.IsNullOrEmpty(envelope?.Encrypt) ? null : envelope!.Encrypt;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>校验时间戳时效窗口（fail-closed：缺失或非数字一律拒绝）。</summary>
    private void ValidateTimestampWindow(string? timestamp)
    {
        if (string.IsNullOrEmpty(timestamp) ||
            !long.TryParse(timestamp, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ts))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingTimestamp, "智能机器人回调验签失败：timestamp 缺失或非数字。");
        }

        var now = _utcNow().ToUnixTimeSeconds();
        if (Math.Abs(now - ts) > WechatCallbackReceiver.ReplayWindowSeconds)
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.TimestampOutOfRange,
                $"智能机器人回调验签失败：timestamp 超出时效窗口（±{WechatCallbackReceiver.ReplayWindowSeconds}s），疑似重放或时钟偏差。");
        }
    }

    /// <summary>
    /// 把解密明文 JSON 解析为回调信封：顶层 <c>msgtype</c> 为判别子（<c>event</c> ⇒ 事件族，其余 ⇒ 消息族）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>internal static</c> 供长连接侧复用（<see cref="LongConnection.WechatBotConnection"/> 收到
    /// <c>aibot_msg_callback</c> / <c>aibot_event_callback</c> 帧后，帧体即本方法消费的<b>明文</b>回调报文，
    /// 信封形态与回调模式完全一致 —— 官方 101463 原文）。诊断文本只带配置键，不回显报文。
    /// </para>
    /// </remarks>
    internal static WechatBotCallbackEvent ParseEvent(string botKey, string decryptedJson, Action<string>? logWarning = null)
    {
        var isEvent = IsEventPayload(decryptedJson);

        var evt = new WechatBotCallbackEvent
        {
            AppKey = botKey,
            DecryptedJson = decryptedJson,
        };

        if (isEvent)
        {
            var payload = Deserialize(decryptedJson, AibotJsonContext.Default.AibotEventCallback);
            if (payload == null)
            {
                throw InvalidPayload();
            }

            evt.AibotId = payload.AibotId;
            evt.MsgId = payload.MsgId;
            evt.ChatId = payload.ChatId;
            evt.ChatType = payload.ChatType;
            evt.FromCorpId = payload.From?.CorpId;
            evt.FromUserId = payload.From?.UserId;
            evt.MsgType = payload.MsgType;
            evt.EventType = payload.Event?.EventType;
            evt.ResponseUrl = payload.ResponseUrl;
            evt.Event = payload;
        }
        else
        {
            var payload = Deserialize(decryptedJson, AibotJsonContext.Default.AibotMessageCallback);
            if (payload == null)
            {
                throw InvalidPayload();
            }

            evt.AibotId = payload.AibotId;
            evt.MsgId = payload.MsgId;
            evt.ChatId = payload.ChatId;
            evt.ChatType = payload.ChatType;
            evt.FromCorpId = payload.From?.CorpId;
            evt.FromUserId = payload.From?.UserId;
            evt.MsgType = payload.MsgType;
            evt.ResponseUrl = payload.ResponseUrl;
            evt.Message = payload;
        }

        if (evt.MsgId == null || evt.MsgId.Length == 0)
        {
            // 官方以 msgid 作为排重唯一标志；缺失说明明文非官方报文结构，拒收而非放行。
            logWarning?.Invoke($"智能机器人报文缺少 msgid（配置键 {botKey}），已丢弃（官方以 msgid 排重）。");
            throw InvalidPayload();
        }

        return evt;
    }

    /// <summary>
    /// 读出顶层 <c>msgtype</c> 判别子（单层属性读取，不做多级手写解析）。
    /// </summary>
    private static bool IsEventPayload(string decryptedJson)
    {
        try
        {
            using var document = JsonDocument.Parse(decryptedJson);
            return document.RootElement.TryGetProperty("msgtype", out var msgType)
                   && msgType.ValueKind == JsonValueKind.String
                   && string.Equals(msgType.GetString(), "event", StringComparison.Ordinal);
        }
        catch (JsonException)
        {
            throw InvalidPayload();
        }
    }

    private static T? Deserialize<T>(string json, System.Text.Json.Serialization.Metadata.JsonTypeInfo<T> typeInfo)
    {
        try
        {
            return JsonSerializer.Deserialize(json, typeInfo);
        }
        catch (JsonException)
        {
            throw InvalidPayload();
        }
    }

    private static WechatCallbackException InvalidPayload()
        => new(WechatCallbackFailureKind.MissingEncrypt, "智能机器人回调报文非法：解密明文不是可识别的官方 JSON 报文。");
}

#else

/// <summary>
/// 智能机器人回调接收器降级实现（低目标框架：无源生成 JSON 上下文，<see cref="IsSupported"/> = false）。
/// </summary>
/// <remarks>
/// 中间件据 <see cref="IsSupported"/> 对 JSON 回调回 415 并告警，<b>不静默降级</b>；本类方法恒不可达。
/// </remarks>
internal sealed class WechatBotCallbackReceiver : IWechatBotCallbackReceiver
{
    /// <inheritdoc />
    public bool IsSupported => false;

    /// <inheritdoc />
    public Task<WechatBotCallbackEvent> ReceiveAsync(
        string botKey, string urlQuery, string body, CancellationToken cancellationToken = default)
        => throw new InvalidOperationException(
            "智能机器人回调通道需要 net8.0 及以上的源生成 JSON 上下文，请将宿主目标框架升级为 net8.0+。");
}

#endif
