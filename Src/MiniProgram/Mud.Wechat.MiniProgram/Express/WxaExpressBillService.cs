// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Mud.HttpUtils;
using Mud.Wechat.MiniProgram.Abstractions;
using Mud.Wechat.MiniProgram.DataModels.Express;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;
using Mud.Wechat.OfficialAccount.Abstractions.Enums;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 商家寄件对账文件通道实现（<see cref="IWxaExpressBillService"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 走 <c>SendRawAsync</c> 原始响应直读，按 Content-Type 分支：JSON → 判错（并做令牌失效自愈重试一次）；
/// 其余 → 对账文件文本内容。<b>令牌手工注入</b>（官方契约 Query <c>access_token</c>），故本类不进 Query 白名单。
/// </para>
/// <para>
/// 请求体序列化优先走 <c>IAotJsonContentSerializer</c> 源生成快车道（net8.0+ 零反射），
/// 低 TFM 回退 options 路径（<c>WxaExpressSingleWaybillGetBillRequest</c> 已登记进 <c>ExpressJsonContext</c>）。
/// </para>
/// </remarks>
public sealed class WxaExpressBillService : IWxaExpressBillService
{
    /// <summary>商家寄件对账文件路由。</summary>
    public const string BillPath = "/cgi-bin/express/delivery/single_waybill/get_bill";

    private readonly IAppContextHolder _appContextHolder;
    private readonly IMpAppManager? _appManager;
    private readonly IHttpContentSerializer _contentSerializer;
#if NET8_0_OR_GREATER
    private readonly IAotJsonContentSerializer? _aotSerializer;
#endif
    private readonly ILogger<WxaExpressBillService> _logger;

    /// <summary>创建商家寄件对账文件通道服务。</summary>
    /// <param name="appContextHolder">应用上下文持有器（与声明式客户端同源）。</param>
    /// <param name="logger">日志器。</param>
    /// <param name="contentSerializer">HTTP 内容序列化器（可选；未注入时用组件默认）。</param>
    /// <param name="appManager">应用注册表（可选；用于「无环境上下文时回退默认应用」，与生成客户端同一口径）。</param>
    public WxaExpressBillService(
        IAppContextHolder appContextHolder,
        ILogger<WxaExpressBillService> logger,
        IHttpContentSerializer? contentSerializer = null,
        IMpAppManager? appManager = null)
    {
        _appContextHolder = appContextHolder ?? throw new ArgumentNullException(nameof(appContextHolder));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _contentSerializer = contentSerializer ?? HttpContentSerializerFactory.CreateDefault();
        _appManager = appManager;
#if NET8_0_OR_GREATER
        _aotSerializer = _contentSerializer as IAotJsonContentSerializer;
#endif
    }

    /// <inheritdoc />
    public Task<string> GetBillAsync(
        WxaExpressSingleWaybillGetBillRequest request, CancellationToken cancellationToken = default)
        => SendAsync(request, cancellationToken);

    /// <summary>共享发送核心：取令牌 → 发送 → 分类 →（令牌失效时）失效重试一次 → 业务判错 / 交回文件文本。</summary>
    private async Task<string> SendAsync(WxaExpressSingleWaybillGetBillRequest request, CancellationToken cancellationToken)
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
                $"{BillPath}?access_token={Uri.EscapeDataString(token)}")
            {
                Content = SerializeBody(request),
            };

            var response = await app.HttpClient.SendRawAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            var outcome = await ClassifyAsync(response, cancellationToken).ConfigureAwait(false);

            if (outcome.IsTokenInvalid && attempt == 1)
            {
                // 令牌失效自愈：失效缓存 → 重试一次。
                _logger.LogWarning(
                    "商家寄件对账文件请求收到令牌失效码 {Errcode}，失效本应用令牌后重试一次。AppKey: {AppKey}, Path: {Path}",
                    outcome.Errcode, app.AppKey, BillPath);
                response.Dispose();
                await InvalidateAccessTokenAsync(app, cancellationToken).ConfigureAwait(false);
                continue;
            }

            if (outcome.Errcode is { } errcode && errcode != WxaErrorCodes.Success)
            {
                response.Dispose();
                throw new WxaException((int)errcode, outcome.Errmsg ?? "微信小程序商家寄件对账文件获取失败。", BillPath);
            }

            var text = response.Content != null
                ? await response.Content.ReadAsStringAsync().ConfigureAwait(false)
                : string.Empty;
            response.Dispose();
            return text;
        }
    }

    /// <summary>序列化请求体：优先源生成 JsonTypeInfo 快车道（零反射），否则走 options 通路。</summary>
    private HttpContent SerializeBody(WxaExpressSingleWaybillGetBillRequest request)
    {
#if NET8_0_OR_GREATER
        // 逐类型取 JsonTypeInfo：请求 DTO 登记于 ExpressJsonContext（AOT 净零）。
        if (_aotSerializer is { } aot)
        {
            return aot.ToHttpContent(request, ExpressJsonContext.Default.WxaExpressSingleWaybillGetBillRequest);
        }
#endif
        return _contentSerializer.ToHttpContent(request);
    }

    // 与生成客户端同一口径：环境上下文缺失时回退默认应用（单应用宿主是最常见形态，
    // 只认 ambient 会让「已配置默认应用」的宿主在本通道恒报错，与本方法原先的提示语自相矛盾）。
    private IMpAppContext ResolveAppContext()
        => _appContextHolder.Current as IMpAppContext
           ?? _appManager?.GetDefaultApp()
           ?? throw new InvalidOperationException(
               "无法找到当前小程序（公众号）应用上下文（IMpAppContext）。" +
               "请先进入应用作用域，或经 AddMpApp 配置默认应用。");

    /// <summary>分类一次响应（对账文件文本 / JSON 错误体）。</summary>
    private static async Task<BillOutcome> ClassifyAsync(
        HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var contentType = response.Content?.Headers.ContentType?.MediaType;
        // ns2.0 无 Contains(string, StringComparison) 重载 ⇒ 用 IndexOf（与公众号线同款）。
        var isJson = contentType != null
                     && contentType.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0;
        if (!isJson)
        {
            return new BillOutcome(contentType);
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
            // 声明为 JSON 却解析失败（如网关错误页）：不参与判错，按「未知 JSON 形态」处理。
        }

        var isTokenInvalid = errcode is MpErrorCodes.InvalidCredential
                             or MpErrorCodes.InvalidAccessToken
                             or MpErrorCodes.ExpiredAccessToken;

        return new BillOutcome(contentType, errcode, errmsg, isTokenInvalid);
    }

    /// <summary>失效本应用 access_token（按契约探测组件的失效能力，与公众号线同形）。</summary>
    private static async Task InvalidateAccessTokenAsync(IMpAppContext app, CancellationToken cancellationToken)
    {
        if (app.AccessTokenManager is TokenManagerBase manager)
        {
            await manager.InvalidateTokenAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>单次响应的分类结果（文件文本 / JSON 错误体两形态）。</summary>
    private sealed class BillOutcome
    {
        /// <summary>创建文件文本形态结果。</summary>
        internal BillOutcome(string? contentType) => ContentType = contentType;

        /// <summary>创建 JSON 错误体形态结果。</summary>
        internal BillOutcome(string? contentType, long? errcode, string? errmsg, bool isTokenInvalid)
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
