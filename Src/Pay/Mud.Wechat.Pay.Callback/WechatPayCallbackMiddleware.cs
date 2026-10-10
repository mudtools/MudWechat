// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;

namespace Mud.Wechat.Pay.Callback;

/// <summary>
/// 微信支付 APIv3 回调 HTTP 中间件（路由 <c>/{GlobalRoutePrefix}/{MerchantKey}</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>应答映射</b>：成功（含无处理器）→ <c>200</c> + <c>{"code":"SUCCESS"}</c>（阻止官方重试）；
/// 三闸失败 / 商户未登记 / 公钥模式 → <c>403</c> + <c>{"code":"FAIL"}</c>（<b>统一文案</b>，不区分失败原因，
/// 避免成为「验签是否通过」的探测 oracle）；方法不符 → <c>405</c>；体长超限 → <c>413</c>；
/// Content-Type 非 JSON → <c>415</c>；软超时 → <c>503</c>（触发官方重试）；未预期异常 → <c>500</c>。
/// </para>
/// <para>
/// <b>凭据来源</b>：商户与密钥全部来自 <c>AddPayApp</c> 注册的 <c>IWechatPayMerchantManager</c>；
/// 本中间件不持有任何密钥。
/// </para>
/// <para><b>PAY-B7</b>：日志只记类别、商户键与耗时；签名原文 / <c>ciphertext</c> / 解密明文一律不入日志。</para>
/// </remarks>
public sealed class WechatPayCallbackMiddleware
{
    /// <summary>成功应答体（官方以 <c>code</c> 判定，返回 SUCCESS 即不再重试）。</summary>
    public const string SuccessResponseBody = "{\"code\":\"SUCCESS\"}";

    /// <summary>失败应答体（<b>固定文案</b>，不含任何具体失败原因）。</summary>
    public const string FailureResponseBody = "{\"code\":\"FAIL\",\"message\":\"处理失败\"}";

    /// <summary>应答内容类型（官方为 JSON）。</summary>
    public const string ResponseContentType = "application/json";

    private readonly RequestDelegate _next;
    private readonly WechatPayCallbackReceiver _receiver;
    private readonly WechatPayCallbackDispatcher _dispatcher;
    private readonly IOptionsMonitor<WechatPayCallbackOptions> _optionsMonitor;
    private readonly ILogger<WechatPayCallbackMiddleware> _logger;

    /// <summary>创建中间件。</summary>
    /// <param name="next">下一中间件。</param>
    /// <param name="receiver">通知接收器（三闸 + 解密）。</param>
    /// <param name="dispatcher">通知分发器。</param>
    /// <param name="optionsMonitor">回调配置监视器（路由前缀与软超时请求期热更）。</param>
    /// <param name="logger">日志器。</param>
    public WechatPayCallbackMiddleware(
        RequestDelegate next,
        WechatPayCallbackReceiver receiver,
        WechatPayCallbackDispatcher dispatcher,
        IOptionsMonitor<WechatPayCallbackOptions> optionsMonitor,
        ILogger<WechatPayCallbackMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>中间件入口。</summary>
    /// <param name="context">HTTP 上下文。</param>
    /// <returns>处理任务。</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var options = _optionsMonitor.CurrentValue;
        var merchantKey = TryMatchRoute(context.Request.Path, options.GlobalRoutePrefix);
        if (merchantKey is null)
        {
            await _next(context).ConfigureAwait(false);
            return;
        }

        try
        {
            // APIv3 通知恒为 POST；无 GET echo 流程（与公众号 URL 验证不同）。
            if (!HttpMethods.IsPost(context.Request.Method))
            {
                context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                return;
            }

            var contentType = context.Request.ContentType;
            if (string.IsNullOrEmpty(contentType)
                || contentType!.IndexOf("json", StringComparison.OrdinalIgnoreCase) < 0)
            {
                context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                return;
            }

            var body = await ReadBodyAsync(context.Request, options.MaxRequestBodySize, context.RequestAborted)
                .ConfigureAwait(false);
            if (body is null)
            {
                _logger.LogWarning("通知请求体超过限制 {MaxBytes} 字节，MerchantKey: {MerchantKey}",
                    options.MaxRequestBodySize, merchantKey);
                context.Response.StatusCode = StatusCodes.Status413RequestEntityTooLarge;
                return;
            }

            var headers = WechatPayCallbackHeaders.From(context.Request.Headers);
            var notificationContext = await _receiver
                .ReceiveAsync(merchantKey, headers, body, context.RequestAborted)
                .ConfigureAwait(false);

            var outcome = await _dispatcher
                .DispatchAsync(notificationContext, context.RequestAborted)
                .ConfigureAwait(false);

            if (outcome == WechatPayCallbackDispatchOutcome.TimedOut)
            {
                await WriteJsonAsync(context, StatusCodes.Status503ServiceUnavailable, FailureResponseBody)
                    .ConfigureAwait(false);
                return;
            }

            await WriteJsonAsync(context, StatusCodes.Status200OK, SuccessResponseBody).ConfigureAwait(false);
        }
        catch (WechatCallbackException ex)
        {
            // 统一 403 + 固定文案：不区分「签名不符 / 序列号未知 / 超窗 / 重放 / 解密失败」，避免探测 oracle。
            _logger.LogWarning(WechatPayCallbackLogEvents.VerificationFailed, ex,
                "通知验证失败（{Kind}），MerchantKey: {MerchantKey}", ex.Kind, merchantKey);
            await WriteJsonAsync(context, StatusCodes.Status403Forbidden, FailureResponseBody)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // 客户端断开：不再写应答。
        }
        catch (OperationCanceledException)
        {
            // 软超时经分发器以结果表达，正常不会走到这里；保留兜底以触发官方重试。
            await WriteJsonAsync(context, StatusCodes.Status503ServiceUnavailable, FailureResponseBody)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(WechatPayCallbackLogEvents.UnhandledError, ex,
                "通知处理发生未预期异常，MerchantKey: {MerchantKey}", merchantKey);
            await WriteJsonAsync(context, StatusCodes.Status500InternalServerError, FailureResponseBody)
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 匹配路由前缀并提取商户键（服务商键含 <c>:</c>，先做 URL 解码归一）。
    /// </summary>
    /// <param name="path">请求路径。</param>
    /// <param name="prefix">配置的路由前缀。</param>
    /// <returns>商户键；未命中返回 <c>null</c>（交给后续中间件）。</returns>
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
        if (rest.Length == 0 || rest.IndexOf('/') >= 0)
        {
            // 只有前缀（无商户段）或出现多段，都视为非本中间件的路由。
            return null;
        }

        return Uri.UnescapeDataString(rest);
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

    /// <summary>写 JSON 应答（无 BOM、不带换行）。</summary>
    private static async Task WriteJsonAsync(HttpContext context, int statusCode, string json)
    {
        context.Response.ContentType = ResponseContentType;
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(json, CancellationToken.None).ConfigureAwait(false);
    }
}
