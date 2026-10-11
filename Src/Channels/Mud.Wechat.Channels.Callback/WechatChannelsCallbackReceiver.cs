// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using System.Text.Json;
using Mud.Wechat.Abstractions.Callback;
using Mud.Wechat.Channels.Abstractions.Callback;

namespace Mud.Wechat.Channels.Callback;

/// <summary>
/// 微信小店回调接收器（JSON 报文：官方强制安全模式，包体 <c>{"ToUserName":"gh_*","Encrypt":"..."}</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 接收时序（与企微 / 公众号 XML 侧同族同算法，仅报文格式为 JSON）：
/// 验签（<c>msg_signature</c>，四参 SHA1：token + timestamp + nonce + Encrypt）→ 时效窗口（±300s，
/// fail-closed：缺失 / 非数字即拒）→ AES-256-CBC 解密（PKCS#7，32 字节块）→ receiveid（= 小店 AppID）校验 →
/// 一次性指纹闸（解密成功之后消费；解密失败不消耗指纹，官方重试可重入）→ 明文 JSON 顶层字段解析。
/// </para>
/// <para>
/// GET URL 验证（echostr）与公众号完全同协议；<b>GET echo 只过时效闸、不消费指纹</b>
/// （同 echostr 二次保存配置必须成功）。
/// </para>
/// </remarks>
public interface IWechatChannelsCallbackReceiver
{
    /// <summary>
    /// GET URL 验证：验签通过即原样返回 <c>echostr</c>（不消费一次性指纹）。
    /// </summary>
    /// <param name="appKey">回调配置键（路由路径段；凭据按其解析，精确键优先回退通配）。</param>
    /// <param name="urlQuery">回调 URL 查询串（含 signature / timestamp / nonce / echostr）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>回包体（echostr 原文）。</returns>
    /// <exception cref="WechatCallbackException">验签 / 时效失败时抛出（中间件统一映射 403）。</exception>
    Task<string> EchoAsync(string appKey, string urlQuery, CancellationToken cancellationToken = default);

    /// <summary>
    /// 接收并校验小店回调（验签 → 时效窗口 → AES 解密 → AppId 校验 → 一次性指纹 → 顶层字段解析）。
    /// </summary>
    /// <param name="appKey">回调配置键（路由路径段）。</param>
    /// <param name="urlQuery">回调 URL 查询串（含 msg_signature / timestamp / nonce）。</param>
    /// <param name="body">回调请求体（JSON：<c>{"ToUserName":"gh_*","Encrypt":"..."}</c>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>回调事件信封（DecryptedXml 承载解密后明文 JSON）。</returns>
    /// <exception cref="WechatCallbackException">验签 / 时效 / 解密 / AppId 校验失败或报文非法时抛出
    /// （该类型继承 <see cref="InvalidOperationException"/>，中间件统一映射 403）。</exception>
    Task<WechatChannelsCallbackEvent> ReceiveAsync(
        string appKey, string urlQuery, string body, CancellationToken cancellationToken = default);
}

/// <summary>
/// 微信小店回调接收器默认实现（时序与失败面完全对齐企微 / 公众号 XML 侧接收器）。
/// </summary>
internal sealed class WechatChannelsCallbackReceiver : IWechatChannelsCallbackReceiver
{
    /// <summary>时间戳时效窗口（秒；与指纹窗口共用同一常量，对齐企微 / 公众号侧 ±300s）。</summary>
    internal const int ReplayWindowSeconds = 300;

    private readonly IOptionsMonitor<ChannelsCallbackOptions> _optionsMonitor;
    private readonly IWechatCallbackReplayGuard _replayGuard;
    private readonly Func<DateTimeOffset> _utcNow;
    private readonly ILogger<WechatChannelsCallbackReceiver>? _logger;

    /// <summary>创建小店回调接收器。</summary>
    /// <param name="optionsMonitor">回调配置监视器（多小店凭据热更新）。</param>
    /// <param name="replayGuard">一次性去重守卫（可选；缺省为进程内实现 —— 多实例部署须由宿主前置分布式实现）。</param>
    /// <param name="utcNow">时钟源（可选，默认 <see cref="DateTimeOffset.UtcNow"/>；便于测试）。</param>
    /// <param name="logger">日志器（可选）。</param>
    public WechatChannelsCallbackReceiver(
        IOptionsMonitor<ChannelsCallbackOptions> optionsMonitor,
        IWechatCallbackReplayGuard? replayGuard = null,
        Func<DateTimeOffset>? utcNow = null,
        ILogger<WechatChannelsCallbackReceiver>? logger = null)
    {
        _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        _replayGuard = replayGuard ?? new InMemoryWechatCallbackReplayGuard();
        _utcNow = utcNow ?? (() => DateTimeOffset.UtcNow);
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<string> EchoAsync(string appKey, string urlQuery, CancellationToken cancellationToken = default)
    {
        var app = ResolveApp(appKey);

        // 双验签（与公众号同构）：GET 用 3 参 signature（sort(token,timestamp,nonce)，不含 echostr）；
        // POST 用 4 参 msg_signature（含包体 Encrypt）。二者参数名不同，分别解析。
        var query = WechatCallbackQuery.Parse(urlQuery);
        query.TryGetValue("signature", out var signature);
        query.TryGetValue("timestamp", out var timestamp);
        query.TryGetValue("nonce", out var nonce);
        if (string.IsNullOrEmpty(signature))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingSignature, "小店回调 URL 验证失败：缺少 signature 参数。");
        }

        var expected = WechatCallbackCrypto.ComputeSignature(app.PushToken, timestamp ?? string.Empty, nonce ?? string.Empty);
        if (!string.Equals(expected, signature, StringComparison.OrdinalIgnoreCase))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.InvalidSignature,
                "小店回调 URL 验证失败：signature 不匹配（请检查 PushToken 配置）。");
        }

        ValidateTimestampWindow(timestamp);

        var echostr = WechatCallbackQuery.Get(urlQuery, "echostr");
        if (string.IsNullOrEmpty(echostr))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingEncrypt, "小店回调 URL 验证失败：缺少 echostr 参数。");
        }

        return Task.FromResult(echostr!);
    }

    /// <inheritdoc />
    public async Task<WechatChannelsCallbackEvent> ReceiveAsync(
        string appKey, string urlQuery, string body, CancellationToken cancellationToken = default)
    {
        var app = ResolveApp(appKey);
        var (signature, timestamp, nonce) = WechatCallbackCrypto.ParseSignatureQuery(urlQuery);
        if (string.IsNullOrEmpty(signature))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingSignature, "小店回调验签失败：缺少 msg_signature 参数。");
        }

        var encrypt = ExtractEncrypt(body)
            ?? throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingEncrypt, "小店回调报文非法：未找到 Encrypt 字段。");

        // 验签比对与后续指纹共用同一次 SHA1 计算结果（官方 msg_signature = SHA1(token, timestamp, nonce, Encrypt)）。
        var fingerprint = WechatCallbackCrypto
            .ComputeSignature(app.PushToken, timestamp ?? string.Empty, nonce ?? string.Empty, encrypt);
        if (!string.Equals(fingerprint, signature, StringComparison.OrdinalIgnoreCase))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.InvalidSignature,
                "小店回调验签失败：msg_signature 不匹配（请检查 PushToken 配置）。");
        }

        ValidateTimestampWindow(timestamp);
        if (string.IsNullOrEmpty(nonce))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingNonce, "小店回调验签失败：nonce 缺失。");
        }

        var decrypted = WechatCallbackCrypto.Decrypt(app.PushEncodingAESKey, encrypt, out var receiveId);

        // 官方：解密明文尾部的 appid 必须与自身微信小店相符（receiveid 即 AppID）⇒ 非一致即拒绝（fail-closed）。
        if (!string.Equals(receiveId, app.AppId, StringComparison.Ordinal))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.ReceiveIdMismatch,
                "小店回调验签失败：明文 appid 与回调配置 AppId 不一致（请检查回调凭据归属）。");
        }

        if (string.IsNullOrEmpty(decrypted))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingEncrypt, "小店回调报文非法：解密明文为空。");
        }

        // 指纹闸：位于「解密 + AppId 校验成功」之后、事件返回之前（解密失败不消耗指纹，官方重试可重入）。
        if (app.RequireReplayGuard
            && !await _replayGuard
                .TryMarkAsync(fingerprint, TimeSpan.FromSeconds(ReplayWindowSeconds), cancellationToken)
                .ConfigureAwait(false))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.ReplaySuspected, "小店回调验签失败：报文已处理过（疑似重放）。");
        }

        return ParseEvent(appKey, decrypted, timestamp, nonce, message => _logger?.LogWarning(
            "小店回调报文缺少事件键（配置键 {AppKey}）：{Message}", appKey, message));
    }

    /// <summary>解析指定回调配置键的凭据（精确键优先，回退通配键）；未命中或凭据非法即抛出。</summary>
    private ChannelsAppCallbackOptions ResolveApp(string appKey)
    {
        var app = _optionsMonitor.CurrentValue.ResolveApp(appKey)
            ?? throw new WechatCallbackException(
                WechatCallbackFailureKind.UnknownReceiver,
                $"回调配置未命中 \"{appKey}\"（既无精确键也无通配 \"{ChannelsCallbackOptions.WildcardAppKey}\" 键）。");

        app.Validate(appKey);
        return app;
    }

    /// <summary>从 JSON 报文体提取 <c>Encrypt</c> 字段（官方推送包体 <c>{"ToUserName":"...","Encrypt":"..."}</c>）。</summary>
    private static string? ExtractEncrypt(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(body);
            return document.RootElement.ValueKind == JsonValueKind.Object
                   && document.RootElement.TryGetProperty("Encrypt", out var encrypt)
                   && encrypt.ValueKind == JsonValueKind.String
                ? encrypt.GetString()
                : null;
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
                WechatCallbackFailureKind.MissingTimestamp, "小店回调验签失败：timestamp 缺失或非数字。");
        }

        var now = _utcNow().ToUnixTimeSeconds();
        if (Math.Abs(now - ts) > ReplayWindowSeconds)
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.TimestampOutOfRange,
                $"小店回调验签失败：timestamp 超出时效窗口（±{ReplayWindowSeconds}s），疑似重放或时钟偏差。");
        }
    }

    /// <summary>把解密明文 JSON 解析为回调信封：顶层 <c>Event</c> 为事件键（官方恒 <c>MsgType=event</c>）。</summary>
    /// <remarks>顶层字段单层读取（对齐企微智能机器人 JSON 通道先例），不做多级手写解析。</remarks>
    internal static WechatChannelsCallbackEvent ParseEvent(
        string appKey, string decryptedJson, string? timestamp, string? nonce, Action<string>? logWarning = null)
    {
        string? toUserName = null;
        string? fromUserName = null;
        string? createTime = null;
        string? msgType = null;
        string? @event = null;

        try
        {
            using var document = JsonDocument.Parse(decryptedJson);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                throw InvalidPayload();
            }

            foreach (var property in document.RootElement.EnumerateObject())
            {
                switch (property.Name)
                {
                    case "ToUserName" when property.Value.ValueKind == JsonValueKind.String:
                        toUserName = property.Value.GetString();
                        break;
                    case "FromUserName" when property.Value.ValueKind == JsonValueKind.String:
                        fromUserName = property.Value.GetString();
                        break;
                    case "CreateTime" when property.Value.ValueKind == JsonValueKind.Number:
                        createTime = property.Value.GetRawText();
                        break;
                    case "MsgType" when property.Value.ValueKind == JsonValueKind.String:
                        msgType = property.Value.GetString();
                        break;
                    case "Event" when property.Value.ValueKind == JsonValueKind.String:
                        @event = property.Value.GetString();
                        break;
                }
            }
        }
        catch (JsonException)
        {
            throw InvalidPayload();
        }

        if (string.IsNullOrEmpty(@event))
        {
            logWarning?.Invoke($"解密明文缺少 Event 字段（配置键 {appKey}），已拒收。");
            throw InvalidPayload();
        }

        return new WechatChannelsCallbackEvent
        {
            AppKey = appKey,
            ToUserName = toUserName,
            FromUserName = fromUserName,
            CreateTime = createTime,
            MsgType = msgType,
            Event = @event,
            DecryptedXml = decryptedJson,
            TimeStamp = timestamp,
            Nonce = nonce,
        };
    }

    private static WechatCallbackException InvalidPayload()
        => new(WechatCallbackFailureKind.MissingEncrypt, "小店回调报文非法：解密明文不是可识别的官方 JSON 事件报文。");
}
