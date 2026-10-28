// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using System.Text;
using Mud.Wechat.Ads.Abstractions.Auth;

namespace Mud.Wechat.Ads.Abstractions.Transport;

/// <summary>
/// 广告线全局参数注入 Handler：在请求<b>真正发出前</b>把 <c>access_token</c> / <c>timestamp</c> / <c>nonce</c>
/// 成组写入 Query。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方契约（2026-10-10 逐页核验 v3.0「全局参数」）</b>：
/// <list type="bullet">
/// <item><description><c>access_token</c>：OAuth 认证令牌，<b>有效期 24 小时</b>，通过授权令牌刷新接口获取。</description></item>
/// <item><description><c>timestamp</c>：当前<b>秒</b>级时间戳，GMT+8，「允许误差 300 秒」—— 是 Unix 秒的绝对值，
/// 故本实现直接用 <see cref="DateTimeOffset.ToUnixTimeSeconds"/>，<b>不做</b>时区换算（换算反而偏离绝对时刻）。</description></item>
/// <item><description><c>nonce</c>：随机字串，「<b>长度不超过 32 字节</b>」且「<b>全局唯一</b>」。</description></item>
/// </list>
/// </para>
/// <para>
/// <b>为什么是 Handler 而不是声明式 <c>[Token]</c>（守卫 ADS-B1）</b>：组件的令牌注入只负责一个参数名，
/// 而这里的三项必须<b>同源于同一次取令牌</b> —— <c>timestamp</c>/<c>nonce</c> 若与令牌来自不同时刻/不同请求，
/// 「一次请求一组凭据」的官方语义就不成立（重放同一 nonce 或让 timestamp 落在 300 秒窗口外都会被拒）。
/// 因此三者在同一个方法体内一次取齐、一次写入，<b>不给</b>调用方拆开上送其中任意一项的空间。
/// </para>
/// <para>
/// <b>幂等放行</b>：请求已带 <c>access_token</c> 时整体不注入（三项都不加）。
/// 理由是「覆盖」与「追加」两种处置都错：覆盖会静默替换调用方显式给出的令牌；
/// 追加会产生同名双参数，而官方网关取哪一支未定义。留原样最可预期，也让「宿主自己管令牌」这种
/// 高级用法有一个明确入口（代价是该项由宿主自行保证时效）。
/// </para>
/// <para>
/// <b>不记录任何凭据</b>（守卫 ADS-B5）：本类无日志字段；令牌只进 Query，
/// 而 Query 由公用基类 <see cref="WechatApiException"/> 的构造期脱敏与组件
/// <c>SensitiveUrlRedactor</c> 词表（含 <c>access_token</c>）兜住。
/// </para>
/// </remarks>
internal sealed class AdsAuthorizationHandler : DelegatingHandler
{
    private readonly IAdsAccessTokenProvider _tokens;
    private readonly IAdsAppContextSwitcher _switcher;
    private readonly IAdsClock _clock;

    /// <summary>创建 Handler（由 DI 解析，随命名客户端的 Handler 管道在每次发送时执行）。</summary>
    /// <param name="tokens">令牌取用端口（<b>每次现取</b>：命中缓存即返回，进入阈值期则自动刷新）。</param>
    /// <param name="switcher">应用作用域切换器（决定这次请求用哪个 <c>client_id</c> 的令牌）。</param>
    /// <param name="clock">时钟缝（<c>timestamp</c> 的可测来源）。</param>
    public AdsAuthorizationHandler(
        IAdsAccessTokenProvider tokens,
        IAdsAppContextSwitcher switcher,
        IAdsClock clock)
    {
        _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        _switcher = switcher ?? throw new ArgumentNullException(nameof(switcher));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">请求缺少 <c>RequestUri</c>（无处写入全局参数）。</exception>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var uri = request.RequestUri
            ?? throw new InvalidOperationException("请求缺少 RequestUri，无法写入广告线全局参数。");

        if (HasQueryParameter(uri, AdsOAuthRoutes.AccessTokenParameter))
        {
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        var accessToken = await _tokens
            .GetAccessTokenAsync(_switcher.CurrentAppKey, cancellationToken)
            .ConfigureAwait(false);

        request.RequestUri = AppendQuery(
            uri,
            (AdsOAuthRoutes.AccessTokenParameter, accessToken),
            (AdsOAuthRoutes.TimestampParameter, _clock.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture)),
            (AdsOAuthRoutes.NonceParameter, CreateNonce()));

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 生成 <c>nonce</c>：32 个小写十六进制字符（正好卡在官方「不超过 32 字节」的上限内，且全 ASCII ⇒ 字节数=字符数）。
    /// </summary>
    /// <remarks>
    /// 官方只要求「随机字串 + 全局唯一」，不校验形状 ⇒ 用 <see cref="Guid.NewGuid"/> 的 "N" 格式：
    /// 单次调用零碰撞概率（v4 的 122 位随机量），且不引入对 <c>RandomNumberGenerator</c> 的额外依赖面。
    /// 注意与企微线的 <c>nonce_str</c> 区分：那支是 15~16 字符的字母数字串，本支是 32 位十六进制。
    /// </remarks>
    internal static string CreateNonce() => Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

    /// <summary>把参数追加到既有 Query 之后（<b>保留</b>调用方已写的参数，只做追加）。</summary>
    private static Uri AppendQuery(Uri uri, params (string Name, string Value)[] parameters)
    {
        // GetLeftPart(Query) 含既有查询串（已转义形态）且无查询串时不带 '?' ⇒ 分隔符由这里判定。
        var builder = new StringBuilder(uri.GetLeftPart(UriPartial.Query));
        var hasQuery = uri.Query.Length > 1;
        foreach (var (name, value) in parameters)
        {
            builder.Append(hasQuery ? '&' : '?');
            hasQuery = true;
            builder.Append(Uri.EscapeDataString(name));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(value));
        }

        return new Uri(builder.ToString(), UriKind.Absolute);
    }

    /// <summary>判断请求 Query 中是否已出现指定参数名（按段首精确匹配，不因值内容含同名子串而误判）。</summary>
    private static bool HasQueryParameter(Uri uri, string name)
    {
        var query = uri.Query;
        if (string.IsNullOrEmpty(query) || query == "?")
        {
            return false;
        }

        var body = query.Substring(1);
        var offset = 0;
        while (offset < body.Length)
        {
            var next = body.IndexOf('&', offset);
            var segmentLength = next < 0 ? body.Length - offset : next - offset;
            if (segmentLength > name.Length
                && string.CompareOrdinal(body, offset, name, 0, name.Length) == 0
                && body[offset + name.Length] == '=')
            {
                return true;
            }

            if (next < 0)
            {
                break;
            }

            offset = next + 1;
        }

        return false;
    }
}
