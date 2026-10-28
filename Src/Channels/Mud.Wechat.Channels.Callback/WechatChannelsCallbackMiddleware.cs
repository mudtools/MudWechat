// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Net;
using Mud.Wechat.Abstractions.Callback;
using Mud.Wechat.Channels.Abstractions.Callback;
using Mud.Wechat.Channels.Abstractions.Metrics;

namespace Mud.Wechat.Channels.Callback;

/// <summary>
/// 微信小店回调 HTTP 中间件（路由 <c>/{GlobalRoutePrefix}/{AppKey}</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>响应码映射</b>：验签/时效/重放/AppId 失败 → <c>403</c> + <b>空体</b>（不得回 success：
///  否则攻击者可据响应体差异探测「验签是否通过」）；方法不符 → <c>405</c>；体长超限 → <c>413</c>；
/// Content-Type 非 JSON 或非法 JSON 包体 → <c>415</c>；未预期异常 → <c>500</c>；
/// 正常 → <c>200</c> + 明文 <c>success</c>（小店无被动回复语义）。
/// </para>
/// <para>
/// <b>软超时出口</b>：取消（分发软超时）→ <c>200</c> + 明文 <c>success</c>（官方许可「无法及时处理」出口；
/// 重推同指纹会被指纹闸 403，处理器须幂等）。
/// </para>
/// </remarks>
public sealed class WechatChannelsCallbackMiddleware
{
    private const string SuccessResponseBody = "success";
    private const string PlainTextContentType = "text/plain; charset=utf-8";

    private readonly RequestDelegate _next;
    private readonly IWechatChannelsCallbackReceiver _receiver;
    private readonly WechatChannelsCallbackDispatcher _dispatcher;
    private readonly IOptionsMonitor<ChannelsCallbackOptions> _optionsMonitor;
    private readonly ILogger<WechatChannelsCallbackMiddleware> _logger;

    /// <summary>创建中间件。</summary>
    /// <param name="next">下一中间件。</param>
    /// <param name="receiver">回调接收器。</param>
    /// <param name="dispatcher">事件分发器。</param>
    /// <param name="optionsMonitor">回调配置监视器（路由前缀与白名单请求期热更）。</param>
    /// <param name="logger">日志器。</param>
    public WechatChannelsCallbackMiddleware(
        RequestDelegate next,
        IWechatChannelsCallbackReceiver receiver,
        WechatChannelsCallbackDispatcher dispatcher,
        IOptionsMonitor<ChannelsCallbackOptions> optionsMonitor,
        ILogger<WechatChannelsCallbackMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>中间件入口。</summary>
    /// <remarks>
    /// <b>观测接线</b>：命中路由的每个入站请求记一笔 <c>callback.request.duration</c>（计时覆盖全程）
    /// 与一笔 <c>callback.request</c> 计数（outcome：success / failure / timeout；细分 error.type）——
    /// 总案 D5 的埋点接线（OB-1），静态 Helper 直调、不进 DI。
    /// </remarks>
    public async Task InvokeAsync(HttpContext context)
    {
        var options = _optionsMonitor.CurrentValue;
        var appKey = TryMatchRoute(context.Request.Path, options.GlobalRoutePrefix);
        if (appKey == null)
        {
            await _next(context).ConfigureAwait(false);
            return;
        }

        using var duration = ChannelsMetricsHelper.RecordCallbackRequest(appKey);
        var outcome = "success";
        string? errorType = null;
        try
        {
            if (!IsSourceAllowed(context, options))
            {
                outcome = "failure";
                errorType = "source_denied";
                _logger.LogWarning("回调来源 IP {RemoteIp} 不在白名单中，拒绝。AppKey: {AppKey}",
                    ResolveRemoteIp(context, options), appKey);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            if (HttpMethods.IsGet(context.Request.Method))
            {
                await HandleEchoAsync(context, appKey).ConfigureAwait(false);
                return;
            }

            if (HttpMethods.IsPost(context.Request.Method))
            {
                // 前置校验（415/413）上提在本层：它们无法经由 HandleReceiveAsync 表达 outcome
                // （局部变量作用域），且与 Pay 中间件的校验层位一致。
                var contentType = context.Request.ContentType;
                var isJson = !string.IsNullOrEmpty(contentType)
                             && contentType!.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0;
                if (!isJson)
                {
                    outcome = "failure";
                    errorType = "content_type";
                    context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                    return;
                }

                var requestBody = await ReadBodyAsync(context.Request, options.MaxRequestBodySize, context.RequestAborted)
                    .ConfigureAwait(false);
                if (requestBody == null)
                {
                    outcome = "failure";
                    errorType = "body_too_large";
                    _logger.LogWarning("回调请求体超过限制 {MaxBytes} 字节，AppKey: {AppKey}",
                        options.MaxRequestBodySize, appKey);
                    context.Response.StatusCode = StatusCodes.Status413RequestEntityTooLarge;
                    return;
                }

                await HandleReceiveAsync(context, appKey, options, requestBody).ConfigureAwait(false);
                return;
            }

            outcome = "failure";
            errorType = "method_not_allowed";
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
        }
        catch (WechatCallbackException ex)
        {
            // 验签/时效/重放/解密/AppId/明文拒收 统一 403 + **空体**。
            outcome = "failure";
            errorType = MapFailureKind(ex.Kind);
            _logger.LogWarning(ex, "回调验证失败（{Kind}），AppKey: {AppKey}", ex.Kind, appKey);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // 客户端断开：不再写应答。
            outcome = "failure";
            errorType = "aborted";
        }
        catch (OperationCanceledException)
        {
            // 分发软超时 → 明文 success + 200（不触发重推）。
            outcome = "timeout";
            _logger.LogWarning("回调分发软超时，回 success + 200（不触发重推），AppKey: {AppKey}", appKey);
            await WriteTextAsync(context, StatusCodes.Status200OK, SuccessResponseBody, CancellationToken.None)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            outcome = "failure";
            errorType = "unexpected";
            _logger.LogError(ex, "回调处理发生未预期异常，AppKey: {AppKey}", appKey);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
        finally
        {
            ChannelsMetricsHelper.RecordCallbackOutcome(appKey, outcome, errorType);
        }
    }

    /// <summary>把回调失败类别映射为观测 <c>error.type</c> 标签（受控取值，拒绝语义不细分——防信息面探测）。</summary>
    private static string MapFailureKind(WechatCallbackFailureKind kind) => kind switch
    {
        WechatCallbackFailureKind.MissingSignature => "signature",
        WechatCallbackFailureKind.InvalidSignature => "signature",
        WechatCallbackFailureKind.MissingTimestamp => "timestamp",
        WechatCallbackFailureKind.TimestampOutOfRange => "timestamp",
        WechatCallbackFailureKind.MissingNonce => "signature",
        WechatCallbackFailureKind.ReceiveIdMismatch => "receive_id",
        WechatCallbackFailureKind.ReplaySuspected => "replay",
        WechatCallbackFailureKind.MissingEncrypt => "payload",
        WechatCallbackFailureKind.UnknownReceiver => "unknown_receiver",
        _ => "unexpected",
    };

    /// <summary>GET URL 验证：回写 <c>echostr</c> 原文（text/plain，无 BOM/引号/换行；不消费一次性指纹）。</summary>
    private async Task HandleEchoAsync(HttpContext context, string appKey)
    {
        var query = context.Request.QueryString.Value ?? string.Empty;
        if (query.StartsWith("?", StringComparison.Ordinal))
        {
            query = query.Substring(1);
        }

        var plain = await _receiver.EchoAsync(appKey, query, context.RequestAborted).ConfigureAwait(false);
        _logger.LogInformation("回调 URL 验证通过，AppKey: {AppKey}", appKey);
        await WriteTextAsync(context, StatusCodes.Status200OK, plain, context.RequestAborted).ConfigureAwait(false);
    }

    /// <summary>POST 事件推送：接收 → 分发 → 应答（415/413 前置校验已上提至 <see cref="InvokeAsync"/>）。</summary>
    private async Task HandleReceiveAsync(
        HttpContext context, string appKey, ChannelsCallbackOptions options, string body)
    {
        var query = context.Request.QueryString.Value ?? string.Empty;
        if (query.StartsWith("?", StringComparison.Ordinal))
        {
            query = query.Substring(1);
        }

        var envelope = await _receiver.ReceiveAsync(appKey, query, body, context.RequestAborted)
            .ConfigureAwait(false);
        var result = await _dispatcher.DispatchAsync(appKey, envelope, context.RequestAborted)
            .ConfigureAwait(false);

        // 结构化应答（会员三事件等）：加密 + 签名信封回写（官方 dev_before/message_push「其他回包内容需加密」）。
        if (result.Reply != null)
        {
            var app = options.ResolveApp(appKey);
            if (app == null)
            {
                _logger.LogError("结构化应答回写时未命中回调配置，AppKey: {AppKey}", appKey);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return;
            }

            try
            {
                var urlNonce = WechatCallbackQuery.Get(query, "nonce");
                var payload = WechatChannelsCallbackReplyWriter.WriteReply(
                    app, result.Reply, urlNonce, DateTimeOffset.UtcNow);
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = WechatChannelsCallbackReplyWriter.JsonContentType;
                await context.Response.WriteAsync(payload, context.RequestAborted).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // 应答组装失败（如 Encrypt 因密钥配置非法抛错）：事件已被全部处理器消费、指纹已消费 ——
                // 丢回复比丢事件更糟，回落明文 success + 200（v1 方案 F3②；重推只会被 403）。
                _logger.LogError(ex, "结构化应答回写失败（已回落明文 success 应答）。AppKey: {AppKey}", appKey);
                await WriteTextAsync(context, StatusCodes.Status200OK, SuccessResponseBody, CancellationToken.None)
                    .ConfigureAwait(false);
            }

            return;
        }

        // 其余事件：官方许可回明文 success / 空串（无需加密）。
        await WriteTextAsync(context, StatusCodes.Status200OK, SuccessResponseBody, context.RequestAborted)
            .ConfigureAwait(false);
    }

    /// <summary>匹配路由前缀并提取应用键（仅前缀路径归一为通配键）。</summary>
    private static string? TryMatchRoute(PathString path, string prefix)
    {
        if (string.IsNullOrEmpty(prefix))
        {
            return null;
        }

        var expected = "/" + prefix.Trim('/');
        var value = path.Value ?? string.Empty;
        if (!value.StartsWith(expected, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var rest = value.Substring(expected.Length).Trim('/');
        if (rest.Length == 0)
        {
            return ChannelsCallbackOptions.WildcardAppKey;
        }

        if (rest.IndexOf('/') >= 0)
        {
            return null;
        }

        return rest;
    }

    /// <summary>来源 IP 准入校验：静态白名单（精确 IP 与 IPv4 CIDR；空 = 不限）。</summary>
    private static bool IsSourceAllowed(HttpContext context, ChannelsCallbackOptions options)
    {
        if (options.AllowedSourceIPs.Count == 0)
        {
            return true;
        }

        var remote = ResolveRemoteIp(context, options);
        if (remote == null)
        {
            return false;
        }

        foreach (var rule in options.AllowedSourceIPs)
        {
            if (MatchesRule(remote, rule))
            {
                return true;
            }
        }

        return false;
    }

    private static bool MatchesRule(IPAddress remote, string rule)
    {
        var trimmed = rule?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            return false;
        }

        var slash = trimmed.IndexOf('/');
        if (slash < 0)
        {
            return IPAddress.TryParse(trimmed, out var exact) && exact.Equals(remote);
        }

        // IPv4 CIDR（/n）：网络前缀逐字节比较。
        if (!IPAddress.TryParse(trimmed.Substring(0, slash), out var network)
            || !int.TryParse(trimmed.Substring(slash + 1), out var prefixLength)
            || remote.AddressFamily != network.AddressFamily)
        {
            return false;
        }

        var remoteBytes = remote.GetAddressBytes();
        var networkBytes = network.GetAddressBytes();
        if (remoteBytes.Length != networkBytes.Length || prefixLength < 0 || prefixLength > remoteBytes.Length * 8)
        {
            return false;
        }

        var fullBytes = prefixLength / 8;
        for (var i = 0; i < fullBytes; i++)
        {
            if (remoteBytes[i] != networkBytes[i])
            {
                return false;
            }
        }

        var remainderBits = prefixLength % 8;
        if (remainderBits > 0)
        {
            var mask = (byte)(0xFF << (8 - remainderBits));
            if ((remoteBytes[fullBytes] & mask) != (networkBytes[fullBytes] & mask))
            {
                return false;
            }
        }

        return true;
    }

    private static IPAddress? ResolveRemoteIp(HttpContext context, ChannelsCallbackOptions options)
    {
        if (options.TrustedProxies
            && context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded)
            && forwarded.Count > 0)
        {
            var first = forwarded[0];
            if (!string.IsNullOrEmpty(first))
            {
                var comma = first!.IndexOf(',');
                var value = comma < 0 ? first : first.Substring(0, comma);
                if (IPAddress.TryParse(value.Trim(), out var parsed))
                {
                    return parsed;
                }
            }
        }

        return context.Connection.RemoteIpAddress;
    }

    /// <summary>逐块读取请求体（按字节计数防多字节超限；超限返回 <c>null</c>）。</summary>
    private static async Task<string?> ReadBodyAsync(
        HttpRequest request, long maxBytes, CancellationToken cancellationToken)
    {
        if (request.ContentLength.HasValue && request.ContentLength.Value > maxBytes)
        {
            return null;
        }

        var buffer = new byte[8192];
        using var body = new MemoryStream();
        long total = 0;
        int read;
        while ((read = await request.Body.ReadAsync(buffer, 0, buffer.Length, cancellationToken)
                   .ConfigureAwait(false)) > 0)
        {
            total += read;
            if (total > maxBytes)
            {
                return null;
            }

            body.Write(buffer, 0, read);
        }

        return Encoding.UTF8.GetString(body.ToArray());
    }

    /// <summary>写纯文本应答（无 BOM、不加引号、不带换行）。</summary>
    private static async Task WriteTextAsync(
        HttpContext context, int statusCode, string text, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = PlainTextContentType;
        if (text.Length > 0)
        {
            await context.Response.WriteAsync(text, cancellationToken).ConfigureAwait(false);
        }
    }
}
