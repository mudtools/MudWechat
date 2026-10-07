// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;
using Mud.Wechat.Abstractions.Callback;
using Mud.Wechat.OfficialAccount.Abstractions.Callback;

namespace Mud.Wechat.OfficialAccount.Callback;

/// <summary>
/// 公众号回调接收器：URL 验证（GET 回显）与消息推送（三模式 + 双验签 + appid 校验 + 抗重放指纹）。
/// </summary>
/// <remarks>
/// <para>
/// <b>双验签（官方明确要求）</b>：GET 用 3 参 <c>signature</c>（<c>sort(token,timestamp,nonce)</c>，<b>不含</b> echostr）；
/// POST 用 4 参 <c>msg_signature</c>（含包体内 <c>Encrypt</c>）。官方原文警告「不要使用 signature 验证！」⇒
/// POST 路径**禁止**回落到 <c>signature</c>（守卫 CB-MP-2 + 反例用例）。
/// </para>
/// <para>
/// <b>三模式</b>：安全模式仅收密文（明文 403）；兼容模式按<b>请求实际形态</b>分支；明文模式仅收明文。
/// </para>
/// <para>
/// <b>抗重放</b>：指纹闸在「解密 + appid 校验成功」之后、事件返回之前消费（解密失败不消耗指纹 ⇒ 官方重试可重入）；
/// GET 回显只过时效闸、<b>不消费指纹</b>（同 echostr 二次保存配置必须成功）。
/// </para>
/// </remarks>
public interface IMpCallbackReceiver
{
    /// <summary>接收 POST 消息推送并解析为信封。</summary>
    /// <param name="appKey">路由提取的应用键。</param>
    /// <param name="urlQuery">回调 URL 查询串（可含前导 <c>?</c>）。</param>
    /// <param name="body">请求体原文（XML）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>解密并解析后的信封。</returns>
    /// <exception cref="WechatCallbackException">验签失败、时效越界、重放、解密失败或 appid 不匹配时抛出。</exception>
    Task<MpCallbackEnvelope> ReceiveAsync(
        string appKey, string urlQuery, string body, CancellationToken cancellationToken = default);

    /// <summary>处理 GET URL 验证并返回应回写的明文（= <c>echostr</c> 原文）。</summary>
    /// <param name="appKey">路由提取的应用键。</param>
    /// <param name="urlQuery">回调 URL 查询串。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>回写体（<c>echostr</c> 原文）。</returns>
    /// <exception cref="WechatCallbackException">验签失败、时效越界或 echostr 解密后的 appid 不匹配时抛出。</exception>
    Task<string> EchoAsync(string appKey, string urlQuery, CancellationToken cancellationToken = default);
}

/// <inheritdoc cref="IMpCallbackReceiver" />
public sealed class MpCallbackReceiver : IMpCallbackReceiver
{
    private readonly IOptionsMonitor<MpCallbackOptions> _optionsMonitor;
    private readonly IWechatCallbackReplayGuard _replayGuard;
    private readonly MpAppIdCrossChecker? _appIdCrossChecker;
    private readonly Func<DateTimeOffset> _utcNow;
    private readonly ILogger<MpCallbackReceiver>? _logger;

    /// <summary>创建回调接收器（无 AppId 交叉校验；宿主直接构造 / 测试用）。</summary>
    /// <param name="optionsMonitor">回调配置监视器（请求期热更）。</param>
    /// <param name="replayGuard">抗重放守卫（默认进程内实现）。</param>
    /// <param name="utcNow">当前时间提供器（测试可注入）。</param>
    /// <param name="logger">日志器。</param>
    public MpCallbackReceiver(
        IOptionsMonitor<MpCallbackOptions> optionsMonitor,
        IWechatCallbackReplayGuard replayGuard,
        Func<DateTimeOffset>? utcNow = null,
        ILogger<MpCallbackReceiver>? logger = null)
        : this(optionsMonitor, replayGuard, null, utcNow, logger)
    {
    }

    /// <summary>创建回调接收器（DI 用：含 AppId 交叉校验协作者）。</summary>
    /// <param name="optionsMonitor">回调配置监视器。</param>
    /// <param name="replayGuard">抗重放守卫。</param>
    /// <param name="appIdCrossChecker">AppId 交叉校验（<c>null</c> = 跳过；未注册多应用基座时为空转）。</param>
    /// <param name="utcNow">当前时间提供器。</param>
    /// <param name="logger">日志器。</param>
    internal MpCallbackReceiver(
        IOptionsMonitor<MpCallbackOptions> optionsMonitor,
        IWechatCallbackReplayGuard replayGuard,
        MpAppIdCrossChecker? appIdCrossChecker,
        Func<DateTimeOffset>? utcNow = null,
        ILogger<MpCallbackReceiver>? logger = null)
    {
        _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        _replayGuard = replayGuard ?? throw new ArgumentNullException(nameof(replayGuard));
        _utcNow = utcNow ?? (() => DateTimeOffset.UtcNow);
        _logger = logger;
        _appIdCrossChecker = appIdCrossChecker;
    }

    /// <inheritdoc />
    public async Task<MpCallbackEnvelope> ReceiveAsync(
        string appKey, string urlQuery, string body, CancellationToken cancellationToken = default)
    {
        var options = _optionsMonitor.CurrentValue;
        var app = ResolveApp(options, appKey);
        var mode = app.ResolveMode(options.DefaultSecurityMode);
        var query = WechatCallbackQuery.Parse(urlQuery);

        var timestamp = query.TryGetValue("timestamp", out var ts) ? ts : null;
        var nonce = query.TryGetValue("nonce", out var n) ? n : null;
        query.TryGetValue("msg_signature", out var msgSignature);
        query.TryGetValue("signature", out var signature);
        query.TryGetValue("encrypt_type", out var encryptType);

        var encrypt = ExtractEncrypt(body);
        var isEncrypted = encrypt != null;

        string plaintext;
        string fingerprint;

        if (isEncrypted)
        {
            // —— 密文分支：4 参 msg_signature（**禁止**用 signature 验 POST）——
            if (string.IsNullOrEmpty(msgSignature))
            {
                throw new WechatCallbackException(
                    WechatCallbackFailureKind.MissingSignature, "回调验签失败：缺少 msg_signature 参数。");
            }

            var expected = WechatCallbackCrypto.ComputeSignature(
                app.PushToken, timestamp ?? string.Empty, nonce ?? string.Empty, encrypt!);
            if (!FixedTimeEquals(expected, msgSignature!))
            {
                throw new WechatCallbackException(
                    WechatCallbackFailureKind.InvalidSignature,
                    "回调验签失败：msg_signature 不匹配（请检查 PushToken 配置）。");
            }

            ValidateTimestampWindow(options, timestamp);
            ValidateNonce(nonce);

            plaintext = WechatCallbackCrypto.Decrypt(app.PushEncodingAESKey, encrypt!, out var receiveId);
            if (!string.Equals(receiveId, app.AppId, StringComparison.Ordinal))
            {
                throw new WechatCallbackException(
                    WechatCallbackFailureKind.ReceiveIdMismatch,
                    "回调验签失败：解密明文尾部的 appid 与配置 AppId 不一致（官方要求校验该值是否与自身公众号相符）。");
            }

            // 加固（非安全闸）：若宿主同时注册了多应用基座，核对两处 AppId 配置是否自相矛盾（首包一次、不 fail-fast）。
            _appIdCrossChecker?.Check(appKey, app.AppId);

            // P2-3：同一次 SHA1 结果先做验签比对，解密成功后再作为一次性指纹 —— 指纹取材于密文，密文缺失则不可得。
            fingerprint = expected;
        }
        else
        {
            // —— 明文分支：安全模式一律拒收；明文/兼容模式用 3 参 signature 验签 ——
            if (mode == MpCallbackSecurityMode.Safe)
            {
                throw new WechatCallbackException(
                    WechatCallbackFailureKind.PlainTextRejected,
                    "回调拒收：当前公众号配置为安全模式（纯密文），明文推送一律拒绝。");
            }

            if (string.IsNullOrEmpty(signature))
            {
                throw new WechatCallbackException(
                    WechatCallbackFailureKind.MissingSignature, "回调验签失败：缺少 signature 参数（明文模式）。");
            }

            var expected = WechatCallbackCrypto.ComputeSignature(
                app.PushToken, timestamp ?? string.Empty, nonce ?? string.Empty);
            if (!FixedTimeEquals(expected, signature!))
            {
                throw new WechatCallbackException(
                    WechatCallbackFailureKind.InvalidSignature,
                    "回调验签失败：signature 不匹配（请检查 PushToken 配置）。");
            }

            ValidateTimestampWindow(options, timestamp);
            ValidateNonce(nonce);

            plaintext = body;
            // 明文模式无密文可取材：以「时效参数 + 明文包体」计算传输级摘要（非签名值，仅作一次性指纹）。
            fingerprint = WechatCallbackCrypto.ComputeSignature(
                string.Empty, timestamp ?? string.Empty, nonce ?? string.Empty, plaintext);
        }

        if (app.RequireReplayGuard)
        {
            var window = TimeSpan.FromSeconds(options.AllowClockSkewSeconds);
            if (!await _replayGuard.TryMarkAsync(fingerprint, window, cancellationToken).ConfigureAwait(false))
            {
                throw new WechatCallbackException(
                    WechatCallbackFailureKind.ReplaySuspected, "回调验签失败：报文已处理过（疑似重放）。");
            }
        }

        var envelope = new MpCallbackEnvelope
        {
            AppKey = appKey,
            DecryptedXml = plaintext,
            TimeStamp = timestamp,
            Nonce = nonce,
            EncryptType = encryptType,
            SecurityMode = mode,
            IsEncrypted = isEncrypted,
        };

        FillEnvelopeFields(envelope, plaintext);
        return envelope;
    }

    /// <inheritdoc />
    public Task<string> EchoAsync(string appKey, string urlQuery, CancellationToken cancellationToken = default)
    {
        var options = _optionsMonitor.CurrentValue;
        var app = ResolveApp(options, appKey);
        var mode = app.ResolveMode(options.DefaultSecurityMode);
        var query = WechatCallbackQuery.Parse(urlQuery);

        query.TryGetValue("signature", out var signature);
        query.TryGetValue("timestamp", out var timestamp);
        query.TryGetValue("nonce", out var nonce);
        query.TryGetValue("echostr", out var echoStr);

        if (string.IsNullOrEmpty(signature))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingSignature, "URL 验证失败：缺少 signature 参数。");
        }

        if (string.IsNullOrEmpty(echoStr))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingEncrypt, "URL 验证失败：缺少 echostr 参数。");
        }

        // F3：URL 验证签名只含 token / timestamp / nonce（**不含** echostr）。
        var expected = WechatCallbackCrypto.ComputeSignature(
            app.PushToken, timestamp ?? string.Empty, nonce ?? string.Empty);
        if (!FixedTimeEquals(expected, signature!))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.InvalidSignature, "URL 验证失败：signature 不匹配（请检查 PushToken 配置）。");
        }

        ValidateTimestampWindow(options, timestamp);
        // 幂等：GET 回显**不消费**指纹（平台会反复点「提交」，同 echostr 二次保存配置必须成功）。

        // 安全/兼容模式下 echostr 为密文：解密校验 appid（明文模式无解密环节，跳过 —— 见方案 R4）。
        if (mode != MpCallbackSecurityMode.Plain)
        {
            try
            {
                WechatCallbackCrypto.Decrypt(app.PushEncodingAESKey, echoStr!, out var receiveId);
                if (!string.Equals(receiveId, app.AppId, StringComparison.Ordinal))
                {
                    throw new WechatCallbackException(
                        WechatCallbackFailureKind.ReceiveIdMismatch,
                        "URL 验证失败：echostr 解密出的 appid 与配置 AppId 不一致。");
                }
            }
            catch (WechatCallbackException ex) when (ex.Kind == WechatCallbackFailureKind.DecryptFailed)
            {
                // 兼容模式下平台可能以明文发送 echostr：此时无解密环节，跳过 appid 校验。
                _logger?.LogDebug("URL 验证的 echostr 非密文形态（兼容/明文模式），跳过 appid 校验。");
            }
        }

        // F3：通过校验后「原样返回 echostr 参数内容」。
        return Task.FromResult(echoStr!);
    }

    private static MpAppCallbackOptions ResolveApp(MpCallbackOptions options, string appKey)
        => options.ResolveApp(appKey)
           ?? throw new WechatCallbackException(
               WechatCallbackFailureKind.UnknownReceiver,
               $"回调配置未命中应用 \"{appKey}\"（既无精确键也无通配 \"{MpCallbackOptions.WildcardAppKey}\" 键）。");

    private void ValidateTimestampWindow(MpCallbackOptions options, string? timestamp)
    {
        if (string.IsNullOrEmpty(timestamp)
            || !long.TryParse(timestamp, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ts))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingTimestamp, "回调验签失败：timestamp 缺失或非数字。");
        }

        var now = _utcNow().ToUnixTimeSeconds();
        if (Math.Abs(now - ts) > options.AllowClockSkewSeconds)
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.TimestampOutOfRange,
                $"回调验签失败：timestamp 超出时效窗口（±{options.AllowClockSkewSeconds}s），疑似重放或时钟偏差。");
        }
    }

    private static void ValidateNonce(string? nonce)
    {
        if (string.IsNullOrEmpty(nonce))
        {
            throw new WechatCallbackException(WechatCallbackFailureKind.MissingNonce, "回调验签失败：nonce 缺失。");
        }
    }

    /// <summary>常量时间比较（防时序侧信道）；<c>netstandard2.0</c> 无 <c>CryptographicOperations</c>，手工等价实现。</summary>
    private static bool FixedTimeEquals(string expected, string actual)
    {
#if NET6_0_OR_GREATER
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expected), Encoding.UTF8.GetBytes(actual));
#else
        var a = Encoding.UTF8.GetBytes(expected);
        var b = Encoding.UTF8.GetBytes(actual);
        if (a.Length != b.Length)
        {
            return false;
        }

        var diff = 0;
        for (var i = 0; i < a.Length; i++)
        {
            diff |= a[i] ^ b[i];
        }

        return diff == 0;
#endif
    }

    /// <summary>提取包体内 <c>&lt;Encrypt&gt;</c> 节点；缺失或非 XML 返回 <c>null</c>（明文形态）。</summary>
    private static string? ExtractEncrypt(string body)
    {
        var root = TryParseXml(body);
        var value = root?.Element("Encrypt")?.Value;
        return string.IsNullOrEmpty(value) ? null : value;
    }

    /// <summary>填充信封公共与专有字段（明文非 XML 时字段留空，不抛 —— 协议外报文交兜底处理器）。</summary>
    private static void FillEnvelopeFields(MpCallbackEnvelope envelope, string plaintext)
    {
        var root = TryParseXml(plaintext);
        if (root == null)
        {
            return;
        }

        envelope.ToUserName = ElementText(root, "ToUserName");
        envelope.FromUserName = ElementText(root, "FromUserName");
        envelope.CreateTime = ElementText(root, "CreateTime");
        envelope.MsgType = ElementText(root, "MsgType");
        envelope.Event = ElementText(root, "Event");
        envelope.MsgId = ElementText(root, "MsgId");
        envelope.MsgDataId = ElementText(root, "MsgDataId");
        envelope.Idx = ElementText(root, "Idx");
    }

    private static string? ElementText(XElement root, string name)
    {
        var element = root.Element(name);
        return element == null ? null : element.Value;
    }

    /// <summary>
    /// 解析 XML（**显式禁用 DTD 与外部实体解析**，防 XXE / 十亿笑声）。
    /// </summary>
    /// <param name="xml">待解析文本。</param>
    /// <returns>根元素；文本为空、非法或含 DTD 时返回 <c>null</c>（不抛）。</returns>
    private static XElement? TryParseXml(string? xml)
    {
        if (string.IsNullOrEmpty(xml))
        {
            return null;
        }

        try
        {
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                MaxCharactersFromEntities = 0,
            };

            using var reader = XmlReader.Create(new StringReader(xml!), settings);
            return XDocument.Load(reader).Root;
        }
        catch (XmlException)
        {
            return null;
        }
    }
}
