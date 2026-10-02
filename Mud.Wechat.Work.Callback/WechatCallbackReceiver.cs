// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 企业微信回调接收接口（对齐 Mud.Feishu.Webhook 的接收端职能）。
/// </summary>
public interface IWechatCallbackReceiver
{
    /// <summary>
    /// 解析并校验回调（URL 验签参数 + 加密 XML + AES 解密），返回结构化事件。
    /// </summary>
    /// <param name="urlQuery">回调 URL 的查询串（含 msg_signature / timestamp / nonce）。</param>
    /// <param name="body">回调请求体（加密 XML，含 Encrypt 节点）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <exception cref="WechatCallbackException">验签失败、疑似重放或解密失败时抛出（<see cref="WechatCallbackException.Kind"/> 标明失败类别）。</exception>
    Task<WechatCallbackEvent> ReceiveAsync(string urlQuery, string body, CancellationToken cancellationToken = default);
}

/// <summary>
/// 企业微信回调接收器（按回调配置条目）：SHA1 验签 → 时效窗口 → AES 解密 → receiveid 校验 → 一次性指纹去重 → 事件字段提取。
/// </summary>
/// <remarks>
/// <para>
/// <b>P0-2 抗重放</b>：验签通过后两道 fail-closed 闸门——时间戳时效窗口
/// （<see cref="ReplayWindowSeconds"/>，缺失/非数字即拒绝）与一次性指纹去重
/// （<see cref="IWechatCallbackReplayGuard"/>，键为与攻击者无关的 SHA1 指纹）。
/// <b>P1-1（决策 D2）</b>：指纹闸位于「解密 + receiveid 校验成功」之后、事件返回之前——
/// 解密成功即证明报文经仅企微与我方共知的 AESKey 验证可信，重复解密无副作用；
/// 解密失败不再消耗指纹，官方重试可重新进入管线（at-least-once 修复）。
/// </para>
/// <para>
/// <b>P2-3</b>：验签比对与指纹去重共用同一次 SHA1 计算结果。
/// </para>
/// <para>
/// 多套件宿主经 <c>AddWechatCallback</c>/<c>AddWechatCallbackSuite</c> 登记进统一注册表
/// （<c>WechatCallbackReceiverGroup</c> 按外层 XML ToUserName 路由到本类）；
/// 「同一接收方 ID、异构密钥」的自定义路由场景可直接构造本类实例（逃逸舱）。
/// </para>
/// </remarks>
public sealed class WechatCallbackReceiver : IWechatCallbackReceiver
{
    /// <summary>抗重放时间戳容差（秒）。与飞书同源 SDK 的 <c>TimestampValidator</c> 同量级。</summary>
    internal const int ReplayWindowSeconds = 300;

    private readonly WechatCallbackOptions _options;
    private readonly IWechatCallbackReplayGuard _replayGuard;
    private readonly Func<DateTimeOffset> _utcNow;
    private readonly ILogger<WechatCallbackReceiver>? _logger;

    /// <summary>「明文未携带 receiveid」告警是否已输出（首次命中输出一次，P3-2）。</summary>
    private int _emptyReceiveIdLogged;

    /// <summary>创建回调接收器。</summary>
    /// <param name="options">回调配置（PushToken / PushEncodingAESKey / 接收方 ID 必填，构造期校验）。</param>
    /// <param name="replayGuard">一次性去重守卫（可选；缺省为进程内实现）。</param>
    /// <param name="utcNow">时钟源（可选，默认 <see cref="DateTimeOffset.UtcNow"/>；便于测试）。</param>
    /// <param name="logger">日志器（可选）。</param>
    /// <remarks>
    /// 使用可选参数而非 <c>TimeProvider</c>：<c>TimeProvider</c> 在 <c>netstandard2.0</c> 不可用。
    /// </remarks>
    public WechatCallbackReceiver(
        WechatCallbackOptions options,
        IWechatCallbackReplayGuard? replayGuard = null,
        Func<DateTimeOffset>? utcNow = null,
        ILogger<WechatCallbackReceiver>? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _options.Validate();
        _replayGuard = replayGuard ?? new InMemoryWechatCallbackReplayGuard();
        _utcNow = utcNow ?? (() => DateTimeOffset.UtcNow);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<WechatCallbackEvent> ReceiveAsync(string urlQuery, string body, CancellationToken cancellationToken = default)
    {
        var encrypt = ExtractEncrypt(body)
            ?? throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingEncrypt, "回调报文非法：未找到 Encrypt 节点。");

        return await ReceiveCoreAsync(urlQuery, encrypt, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 接收管线核心（供组合接收器复用：报文外壳已按 ToUserName 路由，Encrypt 已单次解析提取）。
    /// </summary>
    internal async Task<WechatCallbackEvent> ReceiveCoreAsync(string urlQuery, string encrypt, CancellationToken cancellationToken)
    {
        var (signature, timestamp, nonce) = WechatCallbackCrypto.ParseSignatureQuery(urlQuery);
        if (string.IsNullOrEmpty(signature))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingSignature, "回调验签失败：缺少 msg_signature 参数。");
        }

        // P2-3：同一次 SHA1 结果先做验签比对，解密成功后再作为一次性指纹（P1-1）。
        var fingerprint = WechatCallbackCrypto
            .ComputeSignature(_options.PushToken, timestamp ?? string.Empty, nonce ?? string.Empty, encrypt);
        if (!string.Equals(fingerprint, signature, StringComparison.OrdinalIgnoreCase))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.InvalidSignature,
                "回调验签失败：msg_signature 不匹配（请检查 PushToken / 接收方 ID 配置）。");
        }

        // P0-2 第一道闸：时效窗口（fail-closed：缺失或非数字一律拒绝）。
        ValidateTimestampWindow(timestamp);
        if (string.IsNullOrEmpty(nonce))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingNonce, "回调验签失败：nonce 缺失。");
        }

        var decrypted = WechatCallbackCrypto.Decrypt(_options.PushEncodingAESKey, encrypt, out var receiveId);
        ValidateReceiveId(receiveId);

        // P0-2 第二道闸（P1-1/D2 后移）：位于「解密 + receiveid 校验成功」之后、事件返回之前——
        // 解密成功即证明报文经仅企微与我方共知的 AESKey 验证可信；解密失败不消耗指纹，
        // 官方重试（96238）可重新进入管线。指纹含 token 天然跨套件隔离。
        if (!await _replayGuard
                .TryMarkAsync(fingerprint, TimeSpan.FromSeconds(ReplayWindowSeconds), cancellationToken)
                .ConfigureAwait(false))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.ReplaySuspected, "回调验签失败：报文已处理过（疑似重放）。");
        }

        return ParseEvent(decrypted, timestamp, nonce);
    }

    /// <summary>
    /// URL 验证核心（P1-2，供组合接收器按接收方 ID 分发后调用）：
    /// 验签（echostr 参与签名，同 96238）→ 时间窗 → 解密 → receiveid 校验 → 返回明文。
    /// </summary>
    /// <remarks>
    /// 决策 D5：URL 验证为幂等读（管理端可能反复保存重试），<b>不做指纹去重</b>——
    /// 去重会造成「验证被自己上一次消耗」的假失败；时间窗已足够抗重放。
    /// </remarks>
    internal Task<string> VerifyUrlCoreAsync(string urlQuery, string echostr, CancellationToken cancellationToken)
    {
        var (signature, timestamp, nonce) = WechatCallbackCrypto.ParseSignatureQuery(urlQuery);
        if (string.IsNullOrEmpty(signature))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingSignature, "URL 验证失败：缺少 msg_signature 参数。");
        }

        var expected = WechatCallbackCrypto
            .ComputeSignature(_options.PushToken, timestamp ?? string.Empty, nonce ?? string.Empty, echostr ?? string.Empty);
        if (!string.Equals(expected, signature, StringComparison.OrdinalIgnoreCase))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.InvalidSignature,
                "URL 验证失败：msg_signature 不匹配（请检查 PushToken / 接收方 ID 配置）。");
        }

        ValidateTimestampWindow(timestamp);
        if (string.IsNullOrEmpty(nonce))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingNonce, "URL 验证失败：nonce 缺失。");
        }

        var plain = WechatCallbackCrypto.Decrypt(_options.PushEncodingAESKey, echostr ?? string.Empty, out var receiveId);
        ValidateReceiveId(receiveId);
        return Task.FromResult(plain);
    }

    /// <summary>
    /// 校验时间戳时效窗口（fail-closed：缺失或非数字一律拒绝）。
    /// </summary>
    private void ValidateTimestampWindow(string? timestamp)
    {
        if (string.IsNullOrEmpty(timestamp) ||
            !long.TryParse(timestamp, System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture, out var ts))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingTimestamp, "回调验签失败：timestamp 缺失或非数字。");
        }

        var now = _utcNow().ToUnixTimeSeconds();
        if (Math.Abs(now - ts) > ReplayWindowSeconds)
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.TimestampOutOfRange,
                $"回调验签失败：timestamp 超出时效窗口（±{ReplayWindowSeconds}s），疑似重放或时钟偏差。");
        }
    }

    /// <summary>
    /// 校验解密明文的接收方 ID（<c>receiveid</c>）与配置一致性。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 语义为「接收方 ID」：企业自建应用回调为企业 <c>CorpId</c>，第三方/服务商<b>套件回调为 <c>SuiteId</c></b>。
    /// </para>
    /// <para>
    /// P3-2：明文未携带 receiveid 时跳过校验并一次性告警（官方「个人主体第三方为空串」兼容，90968）；
    /// 配置侧的接收方 ID 为必填（<see cref="WechatCallbackOptions.Validate"/> 注册期校验），不存在「配置为空」分支。
    /// </para>
    /// </remarks>
    private void ValidateReceiveId(string? receiveId)
    {
        if (string.IsNullOrEmpty(receiveId))
        {
            if (Interlocked.Exchange(ref _emptyReceiveIdLogged, 1) == 0)
            {
                _logger?.LogWarning(
                    "回调明文未携带 receiveid，已跳过接收方 ID 校验（官方「个人主体第三方」形态明文 receiveid 为空串）。");
            }

            return;
        }

        if (!string.Equals(_options.CorpId, receiveId, StringComparison.Ordinal))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.ReceiveIdMismatch,
                "回调验签失败：receiveid 与配置的 CorpId（接收方 ID）不一致（套件回调应填 SuiteId）。");
        }
    }

    private static string? ExtractEncrypt(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            var doc = XDocument.Parse(body);
            return doc.Root?.Element("Encrypt")?.Value;
        }
        catch (System.Xml.XmlException)
        {
            return null;
        }
    }

    private static WechatCallbackEvent ParseEvent(string decryptedXml, string? timestamp, string? nonce)
    {
        var evt = new WechatCallbackEvent
        {
            TimeStamp = timestamp,
            Nonce = nonce,
            DecryptedXml = decryptedXml,
        };

        try
        {
            var doc = XDocument.Parse(decryptedXml);
            var root = doc.Root;
            if (root != null)
            {
                evt.InfoType = root.Element("InfoType")?.Value;
                evt.SuiteId = root.Element("SuiteId")?.Value;
                evt.SuiteTicket = root.Element("SuiteTicket")?.Value;
                evt.AuthCode = root.Element("AuthCode")?.Value;

                // R11：create_auth / reset_permanent_code 报文体不含 AuthCorpId，
                // 禁止用 FromUserName 兜底伪造授权企业（该文的授权企业须由 auth_code 换码后反查）。
                evt.AuthCorpId = root.Element("AuthCorpId")?.Value;
                if (string.IsNullOrEmpty(evt.AuthCorpId) && !evt.IsAuthCodeEvent)
                {
                    evt.AuthCorpId = root.Element("FromUserName")?.Value;
                }
            }
        }
        catch (System.Xml.XmlException)
        {
            // 明文非 XML（协议外报文）时保留原文，事件字段为空。
        }

        return evt;
    }
}
