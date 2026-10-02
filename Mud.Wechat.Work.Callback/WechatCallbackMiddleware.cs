// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 企业微信回调 HTTP 中间件（v1 方案 §5.3，对齐 <c>FeishuMultiAppMiddleware</c> 并适配 XML/echo）：
/// 路径提取 AppKey（多应用路由）→ 前置校验（方法/Content-Type/体长/IP 白名单）→
/// GET 走 <see cref="IWechatCallbackReceiver.EchoAsync"/>（URL 验证）、POST 走
/// <see cref="IWechatCallbackReceiver.ReceiveAsync"/> + <see cref="WechatCallbackDispatcher"/> → 按结果写应答。
/// </summary>
/// <remarks>
/// <para>
/// <b>中间件形态（v1.2 定形）</b>：经典约定式（构造器 <see cref="RequestDelegate"/>，应用生命周期单实例）；
/// <c>IMiddleware</c> 工厂式为每请求激活，与 Singleton 模型矛盾。处理器/拦截器实例由分发器内部经
/// <see cref="IServiceScopeFactory"/> 建 scope 解析，本中间件自身无需建 scope。
/// </para>
/// <para>
/// <b>应答契约（企业微信 90930）</b>：POST 成功/未匹配处理器均 200（body=<c>success</c>，官方推荐写法）；
/// 验签/解密/receiveid 失败 403；分发中断（拦截器）或软超时 503（触发重推）；其余 500。
/// 客户端断开（<see cref="HttpContext.RequestAborted"/> 已触发）记日志后不再写响应。
/// </para>
/// <para>
/// <b>IP 白名单</b>：条目支持精确 IP 与 IPv4 CIDR 段（如 <c>101.226.103.0/24</c>，可经官方
/// <c>getcallbackip</c> 获取段值）；白名单非空且无法解析客户端地址时按 fail-closed 拒绝。
/// 反向代理部署下 <see cref="HttpContext.Connection.RemoteIpAddress"/> 为代理地址，
/// 宿主须自行启用 <c>UseForwardedHeaders</c>。
/// </para>
/// </remarks>
public sealed class WechatCallbackMiddleware
{
    /// <summary>POST 事件接收成功应答体（企业微信官方推荐「success」，明确防重试）。</summary>
    private const string SuccessResponseBody = "success";

    private readonly RequestDelegate _next;
    private readonly IWechatCallbackReceiver _receiver;
    private readonly WechatCallbackDispatcher _dispatcher;
    private readonly IOptionsMonitor<WechatCallbackOptions> _optionsMonitor;
    private readonly ILogger<WechatCallbackMiddleware> _logger;

    /// <summary>创建回调中间件（经典约定式，应用生命周期单实例）。</summary>
    public WechatCallbackMiddleware(
        RequestDelegate next,
        IWechatCallbackReceiver receiver,
        WechatCallbackDispatcher dispatcher,
        IOptionsMonitor<WechatCallbackOptions> optionsMonitor,
        ILogger<WechatCallbackMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>处理回调请求。</summary>
    /// <param name="context">HTTP 上下文。</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        var options = _optionsMonitor.CurrentValue;
        var appKey = ExtractAppKey(path, options.GlobalRoutePrefix);
        if (appKey == null)
        {
            // 非回调路由（前缀不匹配 / 深层路径）：交给管线。
            await _next(context).ConfigureAwait(false);
            return;
        }

        // 未知 AppKey 且无通配：跳过（404 由管线末端给出），不打劫非回调端点。
        if (options.ResolveApp(appKey) == null)
        {
            _logger.LogWarning("回调请求命中未知应用键（既无精确键也无通配键），已跳过。AppKey: {AppKey}", appKey);
            await _next(context).ConfigureAwait(false);
            return;
        }

        // IP 白名单（空 = 不限；fail-closed：白名单非空而客户端地址不可解析时拒绝）。
        if (options.AllowedSourceIPs.Count > 0 &&
            !IsIpAllowed(context.Connection.RemoteIpAddress, options.AllowedSourceIPs))
        {
            _logger.LogWarning("回调来源 IP {RemoteIp} 不在白名单中，拒绝。AppKey: {AppKey}",
                context.Connection.RemoteIpAddress, appKey);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        try
        {
            if (HttpMethods.IsGet(context.Request.Method))
            {
                await HandleEchoAsync(context, appKey).ConfigureAwait(false);
                return;
            }

            if (HttpMethods.IsPost(context.Request.Method))
            {
                await HandleReceiveAsync(context, appKey, options).ConfigureAwait(false);
                return;
            }

            // 回调协议方法固定：GET = URL 验证、POST = 事件接收（v1.2 删除 AllowedHttpMethods 死面配置）。
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
        }
        catch (InvalidOperationException ex)
        {
            // 验签/时效/解密/receiveid/凭据失败统一 403（fail-closed；报文解析失败同属验证失败类）。
            _logger.LogWarning(ex, "回调验证失败，AppKey: {AppKey}", appKey);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // 客户端断开：记日志后直接返回，禁止再向已中止连接写响应（对齐飞书 WHF-16）。
            _logger.LogWarning("回调处理期间客户端断开连接，AppKey: {AppKey}", appKey);
        }
        catch (OperationCanceledException)
        {
            // 非客户端断开的取消（分发软超时）→ 503 触发企业微信重推。
            _logger.LogWarning("回调分发软超时，返回 503 触发重推，AppKey: {AppKey}", appKey);
            await WriteTextAsync(context, StatusCodes.Status503ServiceUnavailable, string.Empty, CancellationToken.None)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "回调处理发生未预期异常，AppKey: {AppKey}", appKey);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }

    /// <summary>GET URL 验证：回显解密明文（text/plain，无 BOM/引号/换行）。</summary>
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

    /// <summary>POST 事件接收：Content-Type/体长校验 → 验签解密 → 分发 → 按结果应答。</summary>
    private async Task HandleReceiveAsync(HttpContext context, string appKey, WechatCallbackOptions options)
    {
        var contentType = context.Request.ContentType;
        if (string.IsNullOrEmpty(contentType) ||
            contentType.IndexOf("xml", StringComparison.OrdinalIgnoreCase) < 0)
        {
            // 回调报文为加密 XML（application/xml / text/xml），非 JSON（对齐飞书 415 语义）。
            context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
            return;
        }

        var body = await ReadBodyAsync(context.Request, options.MaxRequestBodySize, context.RequestAborted)
            .ConfigureAwait(false);
        if (body == null)
        {
            _logger.LogWarning("回调请求体超过限制 {MaxBytes} 字节，AppKey: {AppKey}", options.MaxRequestBodySize, appKey);
            context.Response.StatusCode = StatusCodes.Status413RequestEntityTooLarge;
            return;
        }

        var query = context.Request.QueryString.Value ?? string.Empty;
        if (query.StartsWith("?", StringComparison.Ordinal))
        {
            query = query.Substring(1);
        }

        var evt = await _receiver.ReceiveAsync(appKey, query, body, context.RequestAborted).ConfigureAwait(false);
        var outcome = await _dispatcher.DispatchAsync(appKey, evt, context.RequestAborted).ConfigureAwait(false);

        if (outcome == WechatCallbackDispatchOutcome.Interrupted)
        {
            await WriteTextAsync(context, StatusCodes.Status503ServiceUnavailable, string.Empty, context.RequestAborted)
                .ConfigureAwait(false);
            return;
        }

        // Handled / Unhandled 均 200：事件已被企业微信送达并接收（unhandled 已由分发器告警）。
        await WriteTextAsync(context, StatusCodes.Status200OK, SuccessResponseBody, context.RequestAborted)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// 从路径提取应用键：<c>/{prefix}/{appKey}</c> → appKey；<c>/{prefix}</c> → 空串（按通配键解析）；
    /// 其余（前缀不匹配、深层路径）→ <c>null</c>（非本中间件路由）。
    /// </summary>
    private static string? ExtractAppKey(string path, string routePrefix)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        var segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0 ||
            !string.Equals(segments[0], routePrefix, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (segments.Length == 1)
        {
            return string.Empty;
        }

        if (segments.Length == 2)
        {
            return Uri.UnescapeDataString(segments[1]);
        }

        return null;
    }

    /// <summary>逐块读取请求体（按字节计数防多字节超限；超限返回 <c>null</c>）。</summary>
    private static async Task<string?> ReadBodyAsync(HttpRequest request, long maxBytes, CancellationToken cancellationToken)
    {
        // Content-Length 快速拒绝。
        if (request.ContentLength.HasValue && request.ContentLength.Value > maxBytes)
        {
            return null;
        }

        var buffer = new byte[8192];
        using var body = new MemoryStream();
        long total = 0;
        int read;
        while ((read = await request.Body.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
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

    /// <summary>写纯文本应答（echo 明文 / success / 空错误体；无 BOM、不加引号、不带换行）。</summary>
    private static async Task WriteTextAsync(HttpContext context, int statusCode, string text, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "text/plain; charset=utf-8";
        if (text.Length > 0)
        {
            await context.Response.WriteAsync(text, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 判定客户端地址是否在白名单内：条目支持精确 IP 与 IPv4 CIDR 段（<c>a.b.c.d/n</c>）；IPv6 仅精确匹配。
    /// </summary>
    private static bool IsIpAllowed(IPAddress? remoteIp, IReadOnlyList<string> allowed)
    {
        if (remoteIp == null)
        {
            // 白名单已配置而客户端地址不可解析：fail-closed。
            return false;
        }

        foreach (var entry in allowed)
        {
            if (string.IsNullOrWhiteSpace(entry))
            {
                continue;
            }

            var separatorIndex = entry.IndexOf('/');
            if (separatorIndex < 0)
            {
                // 精确 IP。
                if (IPAddress.TryParse(entry.Trim(), out var exact) && exact.Equals(remoteIp))
                {
                    return true;
                }

                continue;
            }

            // IPv4 CIDR 段（官方 getcallbackip 返回形式）。
            var networkPart = entry.Substring(0, separatorIndex).Trim();
            var prefixPart = entry.Substring(separatorIndex + 1).Trim();
            if (!IPAddress.TryParse(networkPart, out var network) ||
                !int.TryParse(prefixPart, System.Globalization.NumberStyles.Integer,
                    System.Globalization.CultureInfo.InvariantCulture, out var prefixLength) ||
                network.AddressFamily != AddressFamily.InterNetwork ||
                remoteIp.AddressFamily != AddressFamily.InterNetwork)
            {
                continue;
            }

            if (IsInIpv4Cidr(remoteIp, network, prefixLength))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>IPv4 CIDR 前缀匹配（前缀 ≤ 0 视为全段放行，≥ 32 退化为精确匹配）。</summary>
    private static bool IsInIpv4Cidr(IPAddress address, IPAddress network, int prefixLength)
    {
        var addressBytes = address.GetAddressBytes();
        var networkBytes = network.GetAddressBytes();
        if (addressBytes.Length != 4 || networkBytes.Length != 4)
        {
            return false;
        }

        if (prefixLength <= 0)
        {
            return true;
        }

        if (prefixLength >= 32)
        {
            for (var i = 0; i < 4; i++)
            {
                if (addressBytes[i] != networkBytes[i])
                {
                    return false;
                }
            }

            return true;
        }

        var fullBytes = prefixLength / 8;
        for (var i = 0; i < fullBytes; i++)
        {
            if (addressBytes[i] != networkBytes[i])
            {
                return false;
            }
        }

        var remainingBits = prefixLength % 8;
        if (remainingBits > 0)
        {
            var mask = (byte)(0xFF << (8 - remainingBits));
            if ((addressBytes[fullBytes] & mask) != (networkBytes[fullBytes] & mask))
            {
                return false;
            }
        }

        return true;
    }
}
