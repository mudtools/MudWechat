// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json;
using Mud.Wechat.OpenPlatform.Abstractions;
using Mud.Wechat.OpenPlatform.Abstractions.Transport;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 第三方平台授权流程服务（<see cref="IComponentAuthorizationService"/>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方契约（2026-10-09 逐页核验）</b>：
/// 预授权码 <c>POST /cgi-bin/component/api_create_preauthcode</c>（有效期 1800 秒）、
/// 换取授权信息 <c>POST /cgi-bin/component/api_query_auth</c>、
/// 刷新接口调用令牌 <c>POST /cgi-bin/component/api_authorizer_token</c>；
/// 三个接口的<b>平台令牌都走 URL 查询参数</b>。
/// </para>
/// <para>
/// <b>⚠️ 令牌会出现在 URL 里（安全）</b>：这是官方契约强制的（无法改为 Header），
/// 与公众号线的 MUD005 同源风险。故本实现<b>绝不</b>把带令牌的 URL 写进日志 / 异常消息 /
/// 遥测 —— 异常消息只带<b>路径</b>，不带查询串。
/// </para>
/// </remarks>
public sealed class ComponentAuthorizationService : IComponentAuthorizationService
{
    private readonly IWechatOpenPlatformHttpClient _httpClient;
    private readonly IComponentTokenProvider _tokenProvider;
    private readonly IOpenPlatformClock _clock;
    private readonly OpenPlatformAppConfig _config;

    /// <summary>创建授权服务。</summary>
    /// <param name="httpClient">本线命名 HTTP 客户端。</param>
    /// <param name="tokenProvider">平台令牌提供者（内部自动按需刷新）。</param>
    /// <param name="clock">时间源。</param>
    /// <param name="config">平台配置。</param>
    /// <exception cref="ArgumentNullException">任一参数为 <c>null</c>。</exception>
    public ComponentAuthorizationService(
        IWechatOpenPlatformHttpClient httpClient,
        IComponentTokenProvider tokenProvider,
        IOpenPlatformClock clock,
        OpenPlatformAppConfig config)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _config.EnsureValid();
    }

    /// <inheritdoc />
    public async Task<PreAuthCodeResult> CreatePreAuthCodeAsync(CancellationToken cancellationToken = default)
    {
        var root = await PostAsync(
            OpenPlatformContract.PreAuthCodePath,
            writer => writer.WriteString(OpenPlatformContract.ComponentAppIdField, _config.ComponentAppId),
            cancellationToken).ConfigureAwait(false);

        var code = ReadString(root, OpenPlatformContract.PreAuthCodeField);
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new WechatOpenPlatformException("获取预授权码：官方应答中不含 pre_auth_code。");
        }

        return new PreAuthCodeResult(code!, _clock.UtcNow.AddSeconds(ResolveLifetime(root, OpenPlatformContract.PreAuthCodeLifetimeSeconds)));
    }

    /// <inheritdoc />
    public async Task<AuthorizerTokens> QueryAuthorizationAsync(
        string authorizationCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(authorizationCode))
        {
            throw new ArgumentException("授权码不能为空。", nameof(authorizationCode));
        }

        var root = await PostAsync(
            OpenPlatformContract.QueryAuthPath,
            writer =>
            {
                writer.WriteString(OpenPlatformContract.ComponentAppIdField, _config.ComponentAppId);
                writer.WriteString(OpenPlatformContract.AuthorizationCodeField, authorizationCode);
            },
            cancellationToken).ConfigureAwait(false);

        if (!root.TryGetProperty(OpenPlatformContract.AuthorizationInfoField, out var info)
            || info.ValueKind != JsonValueKind.Object)
        {
            throw new WechatOpenPlatformException("换取授权信息：官方应答中缺少 authorization_info。");
        }

        return BuildTokens(info);
    }

    /// <inheritdoc />
    public async Task<AuthorizerTokens> RefreshAuthorizerTokenAsync(
        string authorizerAppId,
        string authorizerRefreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(authorizerAppId))
        {
            throw new ArgumentException("授权方 appid 不能为空。", nameof(authorizerAppId));
        }

        if (string.IsNullOrWhiteSpace(authorizerRefreshToken))
        {
            throw new ArgumentException("授权方刷新令牌不能为空。", nameof(authorizerRefreshToken));
        }

        var root = await PostAsync(
            OpenPlatformContract.AuthorizerTokenPath,
            writer =>
            {
                writer.WriteString(OpenPlatformContract.ComponentAppIdField, _config.ComponentAppId);
                writer.WriteString(OpenPlatformContract.AuthorizerAppIdField, authorizerAppId);
                writer.WriteString(OpenPlatformContract.AuthorizerRefreshTokenField, authorizerRefreshToken);
            },
            cancellationToken).ConfigureAwait(false);

        // 应答也返回 authorizer_refresh_token，但官方**未说明**它是否轮换 ⇒
        // 若返回了就按最新值保存（若官方保持不变则等价无操作），未返回则沿用传入值。
        var returnedRefreshToken = ReadString(root, OpenPlatformContract.AuthorizerRefreshTokenField);

        return new AuthorizerTokens(
            authorizerAppId,
            ReadString(root, OpenPlatformContract.AuthorizerAccessTokenField),
            string.IsNullOrWhiteSpace(returnedRefreshToken) ? authorizerRefreshToken : returnedRefreshToken,
            _clock.UtcNow.AddSeconds(ResolveLifetime(root, OpenPlatformContract.AuthorizerTokenLifetimeSeconds)));
    }

    /// <summary>从官方应答构造授权方令牌集合（<c>access_token</c> 可空，见 <see cref="AuthorizerTokens"/>）。</summary>
    private AuthorizerTokens BuildTokens(JsonElement container)
        => new(
            ReadString(container, OpenPlatformContract.AuthorizerAppIdField),
            ReadString(container, OpenPlatformContract.AuthorizerAccessTokenField),
            ReadString(container, OpenPlatformContract.AuthorizerRefreshTokenField),
            _clock.UtcNow.AddSeconds(ResolveLifetime(container, OpenPlatformContract.AuthorizerTokenLifetimeSeconds)));

    /// <summary>取有效期秒数（缺失 / 非正数时回落到契约常量）。</summary>
    private static int ResolveLifetime(JsonElement root, int fallbackSeconds)
    {
        var expiresIn = ReadInt64(root, OpenPlatformContract.ExpiresInField);
        return expiresIn > 0 && expiresIn <= int.MaxValue ? (int)expiresIn : fallbackSeconds;
    }

    /// <summary>
    /// 带平台令牌发一次 POST 并返回应答根元素（<b>已 <c>Clone</c></b>：<see cref="JsonDocument"/> 出作用域即失效）。
    /// </summary>
    /// <remarks>
    /// <b>错误处理顺序</b>：先看 <c>errcode</c>、再看 HTTP 状态 —— 开放平台常以 <b>HTTP 200 + errcode</b>
    /// 表达业务失败，只看状态码会把错误应答当成功。
    /// </remarks>
    private async Task<JsonElement> PostAsync(
        string path,
        Action<Utf8JsonWriter> writeBody,
        CancellationToken cancellationToken)
    {
        // 平台令牌走 URL 查询参数（官方契约）。**该 URL 含令牌 ⇒ 绝不可入日志。**
        var token = await _tokenProvider.GetComponentAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        var uri = string.Concat(
            path, "?", OpenPlatformContract.ComponentAccessTokenQueryName, "=", token);

        using var buffer = new MemoryStream();
        using (var writer = new Utf8JsonWriter(buffer))
        {
            writer.WriteStartObject();
            writeBody(writer);
            writer.WriteEndObject();
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = new StringContent(Encoding.UTF8.GetString(buffer.ToArray()), Encoding.UTF8, "application/json"),
        };

        using var response = await _httpClient.SendRawAsync(request, cancellationToken).ConfigureAwait(false);
        var payload = response.Content != null
            ? await response.Content.ReadAsStringAsync().ConfigureAwait(false)
            : string.Empty;

        JsonElement root = default;
        var parsed = false;
        try
        {
            using var doc = JsonDocument.Parse(payload);
            if (doc.RootElement.ValueKind == JsonValueKind.Object)
            {
                root = doc.RootElement.Clone();
                parsed = true;
            }
        }
        catch (JsonException)
        {
            parsed = false;
        }

        var errorCode = parsed ? ReadString(root, OpenPlatformContract.ErrCodeField) : null;
        if (!string.IsNullOrWhiteSpace(errorCode))
        {
            var errorMessage = ReadString(root, OpenPlatformContract.ErrMsgField);
            throw new WechatOpenPlatformException(
                $"开放平台调用失败（{path}）：errcode={errorCode}, errmsg={errorMessage}。", errorCode);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new WechatOpenPlatformException($"开放平台调用失败（{path}）：HTTP {(int)response.StatusCode}。");
        }

        if (!parsed)
        {
            throw new WechatOpenPlatformException($"开放平台调用失败（{path}）：应答不是可解析的 JSON 对象。");
        }

        return root;
    }

    private static string? ReadString(JsonElement container, string name)
    {
        if (container.ValueKind != JsonValueKind.Object || !container.TryGetProperty(name, out var element))
        {
            return null;
        }

        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.GetRawText(),
            _ => null,
        };
    }

    private static long ReadInt64(JsonElement container, string name)
    {
        if (container.ValueKind != JsonValueKind.Object || !container.TryGetProperty(name, out var element))
        {
            return 0;
        }

        if (element.ValueKind == JsonValueKind.Number && element.TryGetInt64(out var value))
        {
            return value;
        }

        return element.ValueKind == JsonValueKind.String && long.TryParse(element.GetString(), out var parsed)
            ? parsed
            : 0;
    }
}
