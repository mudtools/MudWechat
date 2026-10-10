// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Mud.HttpUtils;
using Mud.Wechat.MiniProgram.Abstractions;
using Mud.Wechat.MiniProgram.DataModels.QrCodeLink;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;
using Mud.Wechat.OfficialAccount.Abstractions.Enums;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 小程序码图片通道实现（<see cref="IWxaCodeService"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 走 <c>SendRawAsync</c> 原始响应直读，按 Content-Type 分支：JSON → 判错（并做令牌失效自愈重试一次）；
/// 其余 → 图片流。<b>令牌手工注入</b>（官方契约 Query <c>access_token</c>），故本类不进 Query 白名单。
/// </para>
/// <para>
/// 请求体序列化优先走 <c>IAotJsonContentSerializer</c> 源生成快车道（net8.0+ 零反射），
/// 低 TFM 回退 options 路径（与生成客户端同一通路，<c>WxaCodeRequest</c> 已登记进
/// <c>QrCodeLinkJsonContext</c>）。
/// </para>
/// </remarks>
public sealed class WxaCodeService : IWxaCodeService
{
    /// <summary>不限量小程序码路由。</summary>
    public const string UnlimitedCodePath = "/wxa/getwxacodeunlimit";

    /// <summary>限量小程序码路由。</summary>
    public const string CodePath = "/wxa/getwxacode";

    /// <summary>二维码 C 路由（本线唯一 <c>/cgi-bin/</c> 前缀端点）。</summary>
    public const string QrCodePath = "/cgi-bin/wxaapp/createwxaqrcode";

    private readonly IAppContextHolder _appContextHolder;
    private readonly IHttpContentSerializer _contentSerializer;
#if NET8_0_OR_GREATER
    private readonly IAotJsonContentSerializer? _aotSerializer;
#endif
    private readonly ILogger<WxaCodeService> _logger;

    /// <summary>创建小程序码通道服务。</summary>
    /// <param name="appContextHolder">应用上下文持有器（与声明式客户端同源）。</param>
    /// <param name="logger">日志器。</param>
    /// <param name="contentSerializer">HTTP 内容序列化器（可选；未注入时用组件默认）。</param>
    public WxaCodeService(
        IAppContextHolder appContextHolder,
        ILogger<WxaCodeService> logger,
        IHttpContentSerializer? contentSerializer = null)
    {
        _appContextHolder = appContextHolder ?? throw new ArgumentNullException(nameof(appContextHolder));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _contentSerializer = contentSerializer ?? HttpContentSerializerFactory.CreateDefault();
#if NET8_0_OR_GREATER
        _aotSerializer = _contentSerializer as IAotJsonContentSerializer;
#endif
    }

    /// <inheritdoc />
    public Task<WxaCodeResult> GetUnlimitedCodeAsync(
        WxaCodeUnlimitRequest request, CancellationToken cancellationToken = default)
        => SendAsync(UnlimitedCodePath, request, cancellationToken);

    /// <inheritdoc />
    public Task<WxaCodeResult> GetCodeAsync(
        WxaCodeRequest request, CancellationToken cancellationToken = default)
        => SendAsync(CodePath, request, cancellationToken);

    /// <inheritdoc />
    public Task<WxaCodeResult> CreateQrCodeAsync(
        WxaQrCodeRequest request, CancellationToken cancellationToken = default)
        => SendAsync(QrCodePath, request, cancellationToken);

    /// <summary>共享发送核心：取令牌 → 发送 → 分类 →（令牌失效时）失效重试一次 → 业务判错 / 交回图片流。</summary>
    private async Task<WxaCodeResult> SendAsync<TRequest>(
        string path, TRequest request, CancellationToken cancellationToken)
        where TRequest : class
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var app = ResolveAppContext();

        for (var attempt = 1; ; attempt++)
        {
            var token = await app.AccessTokenManager.GetTokenAsync(cancellationToken).ConfigureAwait(false);
            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"{path}?access_token={Uri.EscapeDataString(token)}")
            {
                Content = SerializeBody(request),
            };

            var response = await app.HttpClient.SendRawAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            var outcome = await ClassifyAsync(response, cancellationToken).ConfigureAwait(false);

            if (outcome.IsTokenInvalid && attempt == 1)
            {
                // 令牌失效自愈（镜像 MpTicketManagerBase 的 40001 处置）：失效缓存 → 重试一次。
                _logger.LogWarning(
                    "小程序码请求收到令牌失效码 {Errcode}，失效本应用令牌后重试一次。AppKey: {AppKey}, Path: {Path}",
                    outcome.Errcode, app.AppKey, path);
                response.Dispose();
                await InvalidateAccessTokenAsync(app, cancellationToken).ConfigureAwait(false);
                continue;
            }

            if (outcome.Errcode is { } errcode && errcode != WxaErrorCodes.Success)
            {
                response.Dispose();
                throw new WxaException((int)errcode, outcome.Errmsg ?? "微信小程序码请求失败。", path);
            }

            var stream = response.Content != null
                ? await response.Content.ReadAsStreamAsync().ConfigureAwait(false)
                : Stream.Null;

            return new WxaCodeResult(
                response.Content?.Headers.ContentType?.MediaType,
                ExtractFileName(response),
                stream,
                response);
        }
    }

    /// <summary>序列化请求体：优先源生成 JsonTypeInfo 快车道（零反射），否则走 options 通路。</summary>
    private HttpContent SerializeBody<TRequest>(TRequest request)
        where TRequest : class
    {
#if NET8_0_OR_GREATER
        // 逐类型取 JsonTypeInfo：三请求 DTO 均登记于 QrCodeLinkJsonContext（AOT 净零）。
        // 注：必须按**具体类型**分支（JsonTypeInfo<T> 与 T 逐类型配对），不能把 JsonTypeInfo 抹平为基类传入。
        if (_aotSerializer is { } aot)
        {
            switch (request)
            {
                case WxaCodeUnlimitRequest unlimited:
                    return aot.ToHttpContent(unlimited, QrCodeLinkJsonContext.Default.WxaCodeUnlimitRequest);
                case WxaCodeRequest code:
                    return aot.ToHttpContent(code, QrCodeLinkJsonContext.Default.WxaCodeRequest);
                case WxaQrCodeRequest qrcode:
                    return aot.ToHttpContent(qrcode, QrCodeLinkJsonContext.Default.WxaQrCodeRequest);
            }
        }
#endif
        return _contentSerializer.ToHttpContent(request);
    }

    private IMpAppContext ResolveAppContext()
        => _appContextHolder.Current as IMpAppContext
           ?? throw new InvalidOperationException(
               "无法找到当前小程序（公众号）应用上下文（IMpAppContext）。" +
               "请先进入应用作用域或配置默认应用。");

    /// <summary>分类一次响应（图片流 / JSON 错误体）。</summary>
    private static async Task<CodeOutcome> ClassifyAsync(
        HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var contentType = response.Content?.Headers.ContentType?.MediaType;
        // ns2.0 无 Contains(string, StringComparison) 重载 ⇒ 用 IndexOf（与公众号线同款）。
        var isJson = contentType != null
                     && contentType.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0;
        if (!isJson)
        {
            return new CodeOutcome(contentType);
        }

        var body = response.Content != null
            ? await response.Content.ReadAsStringAsync().ConfigureAwait(false)
            : string.Empty;

        long? errcode = null;
        string? errmsg = null;
        try
        {
            using var document = JsonDocument.Parse(body);
            var root = document.RootElement;
            if (root.TryGetProperty("errcode", out var errcodeElement) && errcodeElement.TryGetInt64(out var code))
            {
                errcode = code;
            }

            if (root.TryGetProperty("errmsg", out var errmsgElement) && errmsgElement.ValueKind == JsonValueKind.String)
            {
                errmsg = errmsgElement.GetString();
            }
        }
        catch (JsonException)
        {
            // 声明为 JSON 却解析失败（如网关错误页）：不参与判错，按「未知 JSON 形态」处理（不交出假图片）。
        }

        var isTokenInvalid = errcode is MpErrorCodes.InvalidCredential
                             or MpErrorCodes.InvalidAccessToken
                             or MpErrorCodes.ExpiredAccessToken;

        return new CodeOutcome(contentType, errcode, errmsg, isTokenInvalid);
    }

    /// <summary>失效本应用 access_token（按契约探测组件的失效能力，与公众号线同形）。</summary>
    private static async Task InvalidateAccessTokenAsync(IMpAppContext app, CancellationToken cancellationToken)
    {
        if (app.AccessTokenManager is TokenManagerBase manager)
        {
            await manager.InvalidateTokenAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>从 Content-Disposition 提取文件名（未携带则不推断；剥除引号）。</summary>
    private static string? ExtractFileName(HttpResponseMessage response)
    {
        var disposition = response.Content?.Headers.ContentDisposition;
        if (disposition == null)
        {
            return null;
        }

        var name = disposition.FileNameStar ?? disposition.FileName;
        if (name is { Length: > 1 } quoted
            && quoted.StartsWith("\"", StringComparison.Ordinal)
            && quoted.EndsWith("\"", StringComparison.Ordinal))
        {
            name = quoted.Substring(1, quoted.Length - 2);
        }

        return name;
    }

    /// <summary>单次响应的分类结果（图片流 / JSON 错误体两形态）。</summary>
    private sealed class CodeOutcome
    {
        /// <summary>创建图片流形态结果。</summary>
        internal CodeOutcome(string? contentType) => ContentType = contentType;

        /// <summary>创建 JSON 错误体形态结果。</summary>
        internal CodeOutcome(string? contentType, long? errcode, string? errmsg, bool isTokenInvalid)
        {
            ContentType = contentType;
            Errcode = errcode;
            Errmsg = errmsg;
            IsTokenInvalid = isTokenInvalid;
        }

        /// <summary>响应内容类型。</summary>
        internal string? ContentType { get; }

        /// <summary>JSON 体中的 errcode。</summary>
        internal long? Errcode { get; }

        /// <summary>JSON 体中的 errmsg。</summary>
        internal string? Errmsg { get; }

        /// <summary>是否命中令牌失效码集合。</summary>
        internal bool IsTokenInvalid { get; }
    }
}
