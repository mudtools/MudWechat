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
    /// <exception cref="InvalidOperationException">验签失败或解密失败时抛出。</exception>
    Task<WechatCallbackEvent> ReceiveAsync(string urlQuery, string body, CancellationToken cancellationToken = default);
}

/// <summary>
/// 企业微信回调接收器默认实现：SHA1 验签 → 时效窗口 → 一次性去重 → XML 解析 → AES 解密 → 事件字段提取。
/// </summary>
/// <remarks>
/// <b>P0-2 抗重放</b>：验签通过后追加两道 fail-closed 闸门——时间戳时效窗口
/// （<see cref="ReplayWindowSeconds"/>，缺失/非数字即拒绝）与一次性指纹去重
/// （<see cref="IWechatCallbackReplayGuard"/>，键为与攻击者无关的 SHA1 指纹）。
/// </remarks>
public sealed class WechatCallbackReceiver : IWechatCallbackReceiver
{
    /// <summary>抗重放时间戳容差（秒）。与飞书同源 SDK 的 <c>TimestampValidator</c> 同量级。</summary>
    internal const int ReplayWindowSeconds = 300;

    private readonly WechatCallbackOptions _options;
    private readonly IWechatCallbackReplayGuard _replayGuard;
    private readonly Func<DateTimeOffset> _utcNow;
    private readonly ILogger<WechatCallbackReceiver>? _logger;

    /// <summary>「未配置接收方 ID」告警是否已输出（首次命中输出一次）。</summary>
    private int _receiveIdSkipLogged;

    /// <summary>创建回调接收器。</summary>
    /// <param name="options">回调配置（PushToken / PushEncodingAESKey / 接收方 ID）。</param>
    /// <param name="replayGuard">一次性去重守卫（可选；缺省为进程内实现）。</param>
    /// <param name="utcNow">时钟源（可选，默认 <see cref="DateTimeOffset.UtcNow"/>；便于测试）。</param>
    /// <param name="logger">日志器（可选）。</param>
    /// <remarks>
    /// 使用可选参数而非 <c>TimeProvider</c>：<c>TimeProvider</c> 在 <c>netstandard2.0</c> 不可用。
    /// </remarks>
    public WechatCallbackReceiver(
        IOptions<WechatCallbackOptions> options,
        IWechatCallbackReplayGuard? replayGuard = null,
        Func<DateTimeOffset>? utcNow = null,
        ILogger<WechatCallbackReceiver>? logger = null)
    {
        _options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
        _options.Validate();
        _replayGuard = replayGuard ?? new InMemoryWechatCallbackReplayGuard();
        _utcNow = utcNow ?? (() => DateTimeOffset.UtcNow);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<WechatCallbackEvent> ReceiveAsync(string urlQuery, string body, CancellationToken cancellationToken = default)
    {
        var (signature, timestamp, nonce) = WechatCallbackCrypto.ParseSignatureQuery(urlQuery);
        if (string.IsNullOrEmpty(signature))
        {
            throw new InvalidOperationException("回调验签失败：缺少 msg_signature 参数。");
        }

        var encrypt = ExtractEncrypt(body)
            ?? throw new InvalidOperationException("回调报文非法：未找到 Encrypt 节点。");

        if (!WechatCallbackCrypto.VerifySignature(_options.PushToken, timestamp ?? string.Empty, nonce ?? string.Empty, encrypt, signature))
        {
            throw new InvalidOperationException("回调验签失败：msg_signature 不匹配（请检查 PushToken / CorpId 配置）。");
        }

        // P0-2：验签之后、解密之前的抗重放两道闸（均 fail-closed）。
        ValidateTimestampWindow(timestamp);
        if (string.IsNullOrEmpty(nonce))
        {
            throw new InvalidOperationException("回调验签失败：nonce 缺失。");
        }

        var fingerprint = WechatCallbackCrypto.ComputeSignature(_options.PushToken, timestamp!, nonce!, encrypt);
        if (!await _replayGuard
                .TryMarkAsync(fingerprint, TimeSpan.FromSeconds(ReplayWindowSeconds), cancellationToken)
                .ConfigureAwait(false))
        {
            throw new InvalidOperationException("回调验签失败：报文已处理过（疑似重放）。");
        }

        var decrypted = WechatCallbackCrypto.Decrypt(_options.PushEncodingAESKey, encrypt, out var receiveId);
        ValidateReceiveId(receiveId);

        return ParseEvent(decrypted, timestamp, nonce);
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
            throw new InvalidOperationException("回调验签失败：timestamp 缺失或非数字。");
        }

        var now = _utcNow().ToUnixTimeSeconds();
        if (Math.Abs(now - ts) > ReplayWindowSeconds)
        {
            throw new InvalidOperationException(
                $"回调验签失败：timestamp 超出时效窗口（±{ReplayWindowSeconds}s），疑似重放或时钟偏差。");
        }
    }

    /// <summary>
    /// 校验解密明文的接收方 ID（<c>receiveid</c>）：<see cref="WechatCallbackOptions.CorpId"/> 为空时跳过（仅告警一次）。
    /// </summary>
    /// <remarks>
    /// 语义为「接收方 ID」：企业自建应用回调为企业 <c>CorpId</c>，第三方/服务商<b>套件回调为 <c>SuiteId</c></b>，
    /// 故按「命中其一即通过」判定，避免套件场景误拒合法回调。
    /// </remarks>
    private void ValidateReceiveId(string? receiveId)
    {
        if (string.IsNullOrEmpty(receiveId))
        {
            return;
        }

        var expected = _options.CorpId;
        if (string.IsNullOrEmpty(expected))
        {
            if (Interlocked.Exchange(ref _receiveIdSkipLogged, 1) == 0)
            {
                _logger?.LogWarning(
                    "回调配置未设置 CorpId（接收方 ID），已跳过 receiveid 校验；" +
                    "套件回调请填写 SuiteId，企业自建回调请填写企业 CorpId。");
            }

            return;
        }

        if (!string.Equals(expected, receiveId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
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
