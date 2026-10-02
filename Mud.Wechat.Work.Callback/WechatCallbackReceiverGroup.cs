// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 多套件回调组合接收器（P1-3 统一注册表，决策 D11）：POST 报文按外层 XML <c>ToUserName</c>
/// 路由到对应条目接收器，GET URL 验证按接收方 ID 分发。
/// </summary>
/// <remarks>
/// <para>
/// 官方语义：企业自建回调 ToUserName = 企业 CorpId；第三方/代开发回调 ToUserName = suiteid（模板 id）
/// —— 与 <see cref="WechatCallbackOptions.CorpId"/>（接收方 ID）同语义，路由键与 receiveid 校验键一致。
/// </para>
/// <para>
/// 未命中注册表即 fail-closed 拒绝（<see cref="WechatCallbackException"/>，Kind =
/// <see cref="WechatCallbackFailureKind.UnknownReceiver"/>；异常消息不泄露已登记 ID 清单，
/// 清单仅记入日志）。单次 <c>XDocument.Parse</c> 同时提取 ToUserName 与 Encrypt（F15 收敛）。
/// </para>
/// </remarks>
internal sealed class WechatCallbackReceiverGroup : IWechatCallbackReceiver, IWechatCallbackUrlVerifier
{
    private readonly IReadOnlyDictionary<string, WechatCallbackReceiver> _receivers;
    private readonly ILogger<WechatCallbackReceiverGroup>? _logger;

    /// <summary>创建组合接收器（按注册表条目逐个构建条目接收器；指纹守卫跨条目共享）。</summary>
    /// <param name="registry">回调配置注册表（非空，否则构造失败——经 Add API 登记的注册表恒非空）。</param>
    /// <param name="replayGuard">一次性去重守卫（跨条目共享：指纹含 token 天然隔离）。</param>
    /// <param name="entryLogger">条目接收器日志器（可选）。</param>
    /// <param name="logger">组合接收器日志器（可选）。</param>
    public WechatCallbackReceiverGroup(
        WechatCallbackOptionsRegistry registry,
        IWechatCallbackReplayGuard replayGuard,
        ILogger<WechatCallbackReceiver>? entryLogger = null,
        ILogger<WechatCallbackReceiverGroup>? logger = null)
    {
        if (registry == null) throw new ArgumentNullException(nameof(registry));
        if (replayGuard == null) throw new ArgumentNullException(nameof(replayGuard));

        if (registry.Entries.Count == 0)
        {
            throw new InvalidOperationException(
                "回调注册表为空：请先经 AddWechatCallback / AddWechatCallbackSuite 登记回调配置。");
        }

        _logger = logger;

        var receivers = new Dictionary<string, WechatCallbackReceiver>(StringComparer.Ordinal);
        foreach (var pair in registry.Entries)
        {
            receivers.Add(pair.Key, new WechatCallbackReceiver(pair.Value, replayGuard, logger: entryLogger));
        }

        _receivers = receivers;
        logger?.LogInformation(
            "已登记 {Count} 个回调接收配置（接收方 ID: {ReceiverIds}）。",
            receivers.Count, string.Join(", ", receivers.Keys));
    }

    /// <inheritdoc />
    public async Task<WechatCallbackEvent> ReceiveAsync(string urlQuery, string body, CancellationToken cancellationToken = default)
    {
        var (toUserName, encrypt) = ExtractEnvelope(body);
        if (string.IsNullOrEmpty(encrypt))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingEncrypt, "回调报文非法：未找到 Encrypt 节点。");
        }

        var receiver = ResolveReceiver(toUserName);
        return await receiver.ReceiveCoreAsync(urlQuery, encrypt, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task<string> VerifyUrlAsync(string receiverId, string urlQuery, string echostr, CancellationToken cancellationToken = default)
    {
        var receiver = ResolveReceiver(receiverId);
        return receiver.VerifyUrlCoreAsync(urlQuery, echostr, cancellationToken);
    }

    /// <summary>按接收方 ID 解析条目接收器（fail-closed：未命中抛 <see cref="WechatCallbackFailureKind.UnknownReceiver"/>）。</summary>
    private WechatCallbackReceiver ResolveReceiver(string? receiverId)
    {
        if (!string.IsNullOrEmpty(receiverId) && _receivers.TryGetValue(receiverId, out var receiver))
        {
            return receiver;
        }

        // 已登记 ID 清单只记日志（不进异常消息——该消息会出现在 HTTP 应答链路上）。
        _logger?.LogWarning(
            "回调未命中任何已登记的接收方 ID（收到：{ReceivedId}；已登记：{RegisteredIds}）。",
            receiverId ?? "(空)", string.Join(", ", _receivers.Keys));

        throw new WechatCallbackException(
            WechatCallbackFailureKind.UnknownReceiver,
            $"回调未命中任何已登记的接收方 ID（收到：{receiverId ?? "(空)"}），已拒绝。");
    }

    /// <summary>单次解析同时提取 ToUserName 与 Encrypt（F15：避免双次解析）；报文非法时两者为 null。</summary>
    private static (string? ToUserName, string? Encrypt) ExtractEnvelope(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return (null, null);
        }

        try
        {
            var doc = XDocument.Parse(body);
            var root = doc.Root;
            return (root?.Element("ToUserName")?.Value, root?.Element("Encrypt")?.Value);
        }
        catch (System.Xml.XmlException)
        {
            return (null, null);
        }
    }
}
