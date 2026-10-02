// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback;

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 企业微信回调接收接口（对齐 Mud.Feishu.Webhook 的接收端职能）。
/// </summary>
/// <remarks>
/// v1 方案 §5.3：方法均带 <paramref name="appKey"/> 感知（多应用凭据选择——精确键优先，回退通配键），
/// 由中间件从路由路径提取后显式传入。
/// </remarks>
public interface IWechatCallbackReceiver
{
    /// <summary>
    /// 接收并校验事件回调（URL 验签参数 + 加密 XML + AES 解密），返回结构化事件。
    /// </summary>
    /// <param name="appKey">应用键（路由路径段；凭据按其解析，精确键优先回退通配）。</param>
    /// <param name="urlQuery">回调 URL 的查询串（含 msg_signature / timestamp / nonce）。</param>
    /// <param name="body">回调请求体（加密 XML，含 Encrypt 节点）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <exception cref="WechatCallbackException">验签失败、解密失败或凭据缺失/非法时抛出
    /// （<see cref="WechatCallbackException.Kind"/> 标明失败类别；该类型继承 <see cref="InvalidOperationException"/>）。</exception>
    Task<WechatCallbackEvent> ReceiveAsync(string appKey, string urlQuery, string body, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET URL 验证（v1 方案 §5.1）：验签 + 解密 echostr，返回回显明文。
    /// </summary>
    /// <param name="appKey">应用键（路由路径段；凭据按其解析，精确键优先回退通配）。</param>
    /// <param name="urlQuery">回调 URL 的查询串（含 msg_signature / timestamp / nonce / echostr）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <exception cref="WechatCallbackException">验签失败、解密失败或凭据缺失/非法时抛出。</exception>
    /// <remarks>
    /// echo 为幂等的 URL 配置验证：只过时间戳时效窗口闸，<b>不消费抗重放指纹</b>（v1.2 D8 / D5）——
    /// 同参数二次验证（管理员再次保存回调配置）必须成功。
    /// </remarks>
    Task<string> EchoAsync(string appKey, string urlQuery, CancellationToken cancellationToken = default);
}

/// <summary>
/// 企业微信回调接收器默认实现：SHA1 验签 → 时效窗口 → AES 解密 → receiveid 校验 → 一次性指纹去重 → 事件字段提取。
/// </summary>
/// <remarks>
/// <para>
/// <b>P0-2 抗重放</b>：验签通过后两道 fail-closed 闸门——时间戳时效窗口
/// （<see cref="ReplayWindowSeconds"/>，缺失/非数字即拒绝）与一次性指纹去重
/// （<see cref="IWechatCallbackReplayGuard"/>，键为与攻击者无关的 SHA1 指纹）。
/// <b>P1-1（决策 D2）</b>：指纹闸位于「解密 + receiveid 校验成功」之后、事件返回之前——
/// 解密成功即证明报文经仅企微与我方共知的 AESKey 验证可信，重复解密无副作用；
/// 解密失败不再消耗指纹，官方重试可重新进入管线（at-least-once 修复，不得移回解密之前）。
/// </para>
/// <para>
/// <b>P2-3</b>：验签比对与指纹去重共用同一次 SHA1 计算结果。
/// </para>
/// <para>
/// <b>多应用</b>：凭据经 <see cref="IOptionsMonitor{TOptions}"/> 按 <c>appKey</c> 解析（支持热更新），
/// 本类保持无状态 Singleton。
/// </para>
/// </remarks>
public sealed class WechatCallbackReceiver : IWechatCallbackReceiver
{
    /// <summary>抗重放时间戳容差（秒）。与飞书同源 SDK 的 <c>TimestampValidator</c> 同量级。</summary>
    internal const int ReplayWindowSeconds = 300;

    private readonly IOptionsMonitor<WechatCallbackOptions> _optionsMonitor;
    private readonly IWechatCallbackReplayGuard _replayGuard;
    private readonly Func<DateTimeOffset> _utcNow;
    private readonly ILogger<WechatCallbackReceiver>? _logger;

    /// <summary>「未配置接收方 ID」告警是否已输出（首次命中输出一次）。</summary>
    private int _receiveIdSkipLogged;

    /// <summary>创建回调接收器。</summary>
    /// <param name="optionsMonitor">回调配置监视器（多应用凭据热更新）。</param>
    /// <param name="replayGuard">一次性去重守卫（可选；缺省为进程内实现）。</param>
    /// <param name="utcNow">时钟源（可选，默认 <see cref="DateTimeOffset.UtcNow"/>；便于测试）。</param>
    /// <param name="logger">日志器（可选）。</param>
    /// <remarks>
    /// 使用可选参数而非 <c>TimeProvider</c>：<c>TimeProvider</c> 在 <c>netstandard2.0</c> 不可用。
    /// 构造期不做全量 <see cref="WechatCallbackOptions.Validate"/>——凭据按请求期 appKey 解析后单应用校验，
    /// 避免一个应用的配置错误拖垮全部回调路由（全量校验为宿主启动期入口）。
    /// </remarks>
    public WechatCallbackReceiver(
        IOptionsMonitor<WechatCallbackOptions> optionsMonitor,
        IWechatCallbackReplayGuard? replayGuard = null,
        Func<DateTimeOffset>? utcNow = null,
        ILogger<WechatCallbackReceiver>? logger = null)
    {
        _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        _replayGuard = replayGuard ?? new InMemoryWechatCallbackReplayGuard();
        _utcNow = utcNow ?? (() => DateTimeOffset.UtcNow);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<WechatCallbackEvent> ReceiveAsync(string appKey, string urlQuery, string body, CancellationToken cancellationToken = default)
    {
        var app = ResolveApp(appKey);
        var (signature, timestamp, nonce) = WechatCallbackCrypto.ParseSignatureQuery(urlQuery);
        if (string.IsNullOrEmpty(signature))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingSignature, "回调验签失败：缺少 msg_signature 参数。");
        }

        var encrypt = ExtractEncrypt(body)
            ?? throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingEncrypt, "回调报文非法：未找到 Encrypt 节点。");

        // P2-3：同一次 SHA1 结果先做验签比对，解密成功后再作为一次性指纹（P1-1）。
        var fingerprint = WechatCallbackCrypto
            .ComputeSignature(app.PushToken, timestamp ?? string.Empty, nonce ?? string.Empty, encrypt);
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

        var decrypted = WechatCallbackCrypto.Decrypt(app.PushEncodingAESKey, encrypt, out var receiveId);
        ValidateReceiveId(app, receiveId);

        // P0-2 第二道闸（P1-1/D2 后移）：位于「解密 + receiveid 校验成功」之后、事件返回之前——
        // 解密成功即证明报文经仅企微与我方共知的 AESKey 验证可信；解密失败不消耗指纹，
        // 官方重试（96238）可重新进入管线。指纹含 token 天然跨应用隔离。
        if (!await _replayGuard
                .TryMarkAsync(fingerprint, TimeSpan.FromSeconds(ReplayWindowSeconds), cancellationToken)
                .ConfigureAwait(false))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.ReplaySuspected, "回调验签失败：报文已处理过（疑似重放）。");
        }

        return ParseEvent(decrypted, timestamp, nonce);
    }

    /// <inheritdoc />
    public Task<string> EchoAsync(string appKey, string urlQuery, CancellationToken cancellationToken = default)
    {
        var app = ResolveApp(appKey);
        var (signature, timestamp, nonce, echoStr) = ParseEchoQuery(urlQuery);
        if (string.IsNullOrEmpty(signature))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingSignature, "URL 验证失败：缺少 msg_signature 参数。");
        }

        if (string.IsNullOrEmpty(echoStr))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingEncrypt, "URL 验证失败：缺少 echostr 参数。");
        }

        // 官方 90930：echostr 充当 encrypt 参与项计算签名（`!`：IsNullOrEmpty 在 netstandard2.0 无收窄标注）。
        if (!WechatCallbackCrypto.VerifySignature(app.PushToken, timestamp ?? string.Empty, nonce ?? string.Empty, echoStr!, signature!))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.InvalidSignature,
                "URL 验证失败：msg_signature 不匹配（请检查 PushToken 配置）。");
        }

        // 时效窗口闸保持 fail-closed；不消费指纹（幂等验证，v1.2 D8/D5）。
        ValidateTimestampWindow(timestamp);

        var plain = WechatCallbackCrypto.Decrypt(app.PushEncodingAESKey, echoStr, out var receiveId);
        ValidateReceiveId(app, receiveId);

        return Task.FromResult(plain);
    }

    /// <summary>
    /// 解析指定应用键的回调凭据（精确键优先，回退通配键）；未命中或凭据非法即抛出。
    /// </summary>
    private WechatAppCallbackOptions ResolveApp(string appKey)
    {
        var options = _optionsMonitor.CurrentValue;
        var app = options.ResolveApp(appKey)
            ?? throw new WechatCallbackException(
                WechatCallbackFailureKind.UnknownReceiver,
                $"回调配置未命中应用 \"{appKey}\"（既无精确键也无通配 \"{WechatCallbackOptions.WildcardAppKey}\" 键）。");

        app.Validate(appKey);
        return app;
    }

    /// <summary>
    /// 解析 URL 验证查询串（msg_signature / timestamp / nonce / echostr）。
    /// </summary>
    private static (string? Signature, string? Timestamp, string? Nonce, string? EchoStr) ParseEchoQuery(string urlQuery)
    {
        string? signature = null, timestamp = null, nonce = null, echoStr = null;
        foreach (var pair in (urlQuery ?? string.Empty)
                     .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = pair.Split(new[] { '=' }, 2);
            if (kv.Length != 2)
            {
                continue;
            }

            var value = Uri.UnescapeDataString(kv[1]);
            switch (kv[0])
            {
                case "msg_signature": signature = value; break;
                case "timestamp": timestamp = value; break;
                case "nonce": nonce = value; break;
                case "echostr": echoStr = value; break;
            }
        }

        return (signature, timestamp, nonce, echoStr);
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
    /// 校验解密明文的接收方 ID（<c>receiveid</c>）：应用凭据的 <see cref="WechatAppCallbackOptions.CorpId"/> 为空时跳过（仅告警一次）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 语义为「接收方 ID」：企业自建应用回调为企业 <c>CorpId</c>，第三方/服务商<b>套件回调为 <c>SuiteId</c></b>，
    /// 通讯录同步助手（通配键）可留空；故按「命中其一即通过」判定，避免套件场景误拒合法回调。
    /// </para>
    /// <para>
    /// P3-2：明文未携带 receiveid 时跳过校验（官方「个人主体第三方为空串」兼容，90968）。
    /// </para>
    /// </remarks>
    private void ValidateReceiveId(WechatAppCallbackOptions app, string? receiveId)
    {
        if (string.IsNullOrEmpty(receiveId))
        {
            return;
        }

        var expected = app.CorpId;
        if (string.IsNullOrEmpty(expected))
        {
            if (Interlocked.Exchange(ref _receiveIdSkipLogged, 1) == 0)
            {
                _logger?.LogWarning(
                    "回调配置未设置 CorpId（接收方 ID），已跳过 receiveid 校验；" +
                    "套件回调请填写 SuiteId，企业自建回调请填写企业 CorpId，通讯录同步助手可留空。");
            }

            return;
        }

        if (!string.Equals(expected, receiveId, StringComparison.Ordinal))
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

    /// <summary>
    /// 解析事件信封（v1 方案 §5.4.1）：先抽取通用字段，再补授权族字段。
    /// </summary>
    /// <remarks>
    /// <b>D10（v1.2）</b>：<c>AuthCorpId ← FromUserName</c> 兜底仅限授权族（InfoType 非空且非 authcode 族）——
    /// 通讯录变更事件的 FromUserName 固定为 sys，无差别兜底会伪造授权企业字段。
    /// </remarks>
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
                // 通用信封字段。
                evt.ToUserName = root.Element("ToUserName")?.Value;
                evt.FromUserName = root.Element("FromUserName")?.Value;
                evt.CreateTime = root.Element("CreateTime")?.Value;
                evt.MsgType = root.Element("MsgType")?.Value;
                evt.Event = root.Element("Event")?.Value;
                evt.ChangeType = root.Element("ChangeType")?.Value;
                evt.AgentID = root.Element("AgentID")?.Value;

                // 授权族字段。
                evt.InfoType = root.Element("InfoType")?.Value;
                evt.SuiteId = root.Element("SuiteId")?.Value;
                evt.SuiteTicket = root.Element("SuiteTicket")?.Value;
                evt.AuthCode = root.Element("AuthCode")?.Value;

                // R11：create_auth / reset_permanent_code 报文体不含 AuthCorpId，
                // 禁止用 FromUserName 兜底伪造授权企业（该文的授权企业须由 auth_code 换码后反查）。
                // D10：FromUserName 兜底亦仅限授权族——change_contact 的 FromUserName 固定为 sys。
                evt.AuthCorpId = root.Element("AuthCorpId")?.Value;
                if (string.IsNullOrEmpty(evt.AuthCorpId) && !evt.IsAuthCodeEvent && !string.IsNullOrEmpty(evt.InfoType))
                {
                    evt.AuthCorpId = evt.FromUserName;
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
