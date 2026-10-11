// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Microsoft.Extensions.Logging;
using Mud.HttpUtils;
using Mud.Wechat.OfficialAccount.Abstractions;
using Mud.Wechat.OfficialAccount.Abstractions.Enums;
using Mud.Wechat.OfficialAccount.Abstractions.Exceptions;
using Mud.Wechat.OfficialAccount.DataModels;
using Mud.Wechat.OfficialAccount.DataModels.Nontax;

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 非税缴费对账单下载服务实现（对齐 <c>MpMediaDownloadService</c> 的双形态解释机器：
/// 取令牌 → POST JSON 体 → 分类「JSON 错误体 vs CSV 账单流」→ 令牌失效重试一次 → 业务判错）。
/// </summary>
/// <remarks>
/// <para>
/// <b>单例语义</b>：本类无状态（流的生命周期由结果对象承载），经 <c>MpModule.Nontax</c> 注册。
/// </para>
/// </remarks>
public sealed class MpNontaxBillDownloadService : IMpNontaxBillDownloadService
{
    /// <summary>下载对账单路径常量（计入 <c>MpRouteCountGuard</c> RC2 的下载通道集合）。</summary>
    public const string DownloadBillPath = "/nontax/downloadbill";

    private readonly IAppContextHolder _appContextHolder;
    private readonly IHttpContentSerializer _contentSerializer;
#if NET8_0_OR_GREATER
    private readonly IAotJsonContentSerializer? _aotSerializer;
#endif
    private readonly ILogger<MpNontaxBillDownloadService> _logger;

    /// <summary>创建非税账单下载服务。</summary>
    /// <param name="appContextHolder">应用上下文持有器（取当前应用令牌）。</param>
    /// <param name="logger">日志器。</param>
    /// <param name="contentSerializer">内容序列化器（可选；缺省用组件默认）。</param>
    public MpNontaxBillDownloadService(
        IAppContextHolder appContextHolder,
        ILogger<MpNontaxBillDownloadService> logger,
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
    public async Task<MpNontaxBillDownloadResult> DownloadBillAsync(
        MpNontaxDownloadBillRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.BillDate))
        {
            throw new ArgumentException("bill_date 不能为空。", nameof(request));
        }

        var app = ResolveAppContext();

        for (var attempt = 1; ; attempt++)
        {
            var token = await app.AccessTokenManager.GetTokenAsync(cancellationToken).ConfigureAwait(false);
            var requestUri = $"{DownloadBillPath}?access_token={Uri.EscapeDataString(token)}";

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                // 官方契约：请求体 JSON。优先走源生成 JsonTypeInfo 快车道（零反射、AOT 安全）。
                Content = SerializeBody(request),
            };

            var response = await app.HttpClient.SendRawAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            var outcome = await ClassifyAsync(response, cancellationToken).ConfigureAwait(false);

            // 令牌失效码：失效缓存 → 强制刷新 → 重试一次（镜像素材下载的 40001 自愈先例）。
            if (outcome.IsTokenInvalid && attempt == 1)
            {
                _logger.LogWarning(
                    "非税账单请求收到令牌失效码 {Errcode}，失效本应用令牌后重试一次。AppKey: {AppKey}",
                    outcome.Errcode, app.AppKey);
                response.Dispose();
                await InvalidateAccessTokenAsync(app, cancellationToken).ConfigureAwait(false);
                continue;
            }

            if (outcome.Errcode is { } errcode && errcode != 0)
            {
                response.Dispose();
                throw new MpException((int)errcode, outcome.Errmsg ?? "微信公众号非税账单请求失败。", requestUri);
            }

            var stream = response.Content != null
                ? await response.Content.ReadAsStreamAsync().ConfigureAwait(false)
                : Stream.Null;
            return new MpNontaxBillDownloadResult(outcome.ContentType, ExtractFileName(response), stream, response);
        }
    }

    private HttpContent SerializeBody(MpNontaxDownloadBillRequest request)
    {
#if NET8_0_OR_GREATER
        if (_aotSerializer != null)
        {
            return _aotSerializer.ToHttpContent(request, NontaxJsonContext.Default.MpNontaxDownloadBillRequest);
        }
#endif
        return _contentSerializer.ToHttpContent(request);
    }

    private IMpAppContext ResolveAppContext()
        => _appContextHolder.Current as IMpAppContext
           ?? throw new InvalidOperationException(
               "无法找到当前公众号应用上下文（IMpAppContext）。" +
               "请先进入应用作用域（SwitchTo / UseAppScope）或配置默认应用。");

    /// <summary>把响应二分为「JSON 错误体」与「账单文本流」。</summary>
    private static async Task<BillOutcome> ClassifyAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var contentType = response.Content?.Headers.ContentType?.MediaType;
        // ns2.0 无 Contains(string, StringComparison) 重载 ⇒ 用 IndexOf。
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
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
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
            // 声明为 JSON 但解析失败（如网关错误页）：不参与判错，按「未定义 JSON 形态」防御处理。
        }

        var isTokenInvalid = errcode == MpErrorCodes.InvalidCredential
                             || errcode == MpErrorCodes.InvalidAccessToken
                             || errcode == MpErrorCodes.ExpiredAccessToken;
        return new BillOutcome(contentType, errcode, errmsg, isTokenInvalid);
    }

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

    /// <summary>失效本应用 access_token（按契约探测组件的失效能力，与素材下载同形）。</summary>
    private static async Task InvalidateAccessTokenAsync(IMpAppContext app, CancellationToken cancellationToken)
    {
        if (app.AccessTokenManager is Mud.HttpUtils.TokenManagerBase manager)
        {
            await manager.InvalidateTokenAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    private sealed class BillOutcome
    {
        internal BillOutcome(string? contentType)
        {
            ContentType = contentType;
        }

        internal BillOutcome(string? contentType, long? errcode, string? errmsg, bool isTokenInvalid)
        {
            ContentType = contentType;
            Errcode = errcode;
            Errmsg = errmsg;
            IsTokenInvalid = isTokenInvalid;
        }

        internal string? ContentType { get; }

        internal long? Errcode { get; }

        internal string? Errmsg { get; }

        internal bool IsTokenInvalid { get; }
    }
}
