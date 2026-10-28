// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Metrics;
using System.Text.Json;

namespace Mud.Wechat.Work.Abstractions.Authentication;

/// <summary>
/// 企业微信 errcode 令牌失效判定器（<see cref="ITokenInvalidationDetector"/> 两阶段实现）。
/// </summary>
/// <remarks>
/// <para>
/// 业务响应 HTTP 200 + <c>errcode ∈ {40014, 42001, 42007, 42009, 42011}</c> 判真，
/// 触发恢复链路：失效缓存 → 强制刷新 → 重建请求重注入 → 重试。
/// </para>
/// <para>
/// <b>登记方式</b>：以「子判定器」身份注册为 DI 集合项
/// （<c>services.TryAddEnumerable(...)</c>），由公用层
/// <see cref="WechatTokenRecoveryRegistration.AddWechatTokenRecovery"/> 经 <c>IPostConfigureOptions</c>
/// 追加进 <c>TokenRecoveryOptions.AdditionalTokenInvalidationDetectors</c>（与单槽属性为并集语义）。
/// </para>
/// <para>
/// <b>不可退回「本判定器直接写单槽属性」</b>：该属性为<b>单槽</b>，公众号产品线同样需要写入 ⇒
/// 后注册者会覆盖前者，导致另一方 errcode 令牌恢复静默失效。
/// </para>
/// <para>
/// <b>corp 槽补偿失效（F1，架构审查报告 v2 P1-1）</b>：上游恢复管线对
/// <see cref="TokenManagerBase"/> 派生管理器走 internal 非虚
/// <c>InvalidateCachedAccessToken(scopes)</c>，null scopes 落 <c>default</c> 槽——而企业级令牌
/// 永不写 default 槽 ⇒ corp 槽未被失效，<see cref="CorpTokenManager.InvalidateTokenAsync"/>
/// 的环境补齐覆写在管线内被绕过（40014 自愈空转）。本判定器在「判定为真」处对
/// <b>access_token 族</b> errcode（40014/42001/42007，不含恰在 default 槽的 42009/42011）
/// 经 <see cref="IAppContextHolder.Current"/> 直取环境应用的
/// <see cref="IWechatAppContext.CorpTokenManager"/> 主动失效 corp 槽（按环境补齐 scope），
/// 补偿后管线的 default 槽失效落空无害、刷新半程命中已清空的 corp 槽 ⇒ 真实刷新。
/// 补偿失败仅退化为无自愈现状，<b>不影响判定结果</b>。
/// </para>
/// </remarks>
public sealed class WechatTokenInvalidationDetector : ITokenInvalidationDetector
{
    private static readonly long[] InvalidErrcodes =
    [
        WechatErrorCodes.InvalidAccessToken,          // 40014
        WechatErrorCodes.ExpiredAccessToken,          // 42001
        WechatErrorCodes.RelatedAccessTokenInvalid,   // 42007
        WechatErrorCodes.InvalidSuiteAccessToken,     // 42009
        WechatErrorCodes.InvalidProviderAccessToken,  // 42011
    ];

    /// <summary>
    /// access_token 族失效码（corp 槽补偿失效的族别门控；<b>不含</b> 42009/42011——
    /// 套件/服务商令牌恰在 default 槽，既有管线失效路径本来就正确）。
    /// </summary>
    private static readonly long[] AccessTokenFamilyErrcodes =
    [
        WechatErrorCodes.InvalidAccessToken,          // 40014
        WechatErrorCodes.ExpiredAccessToken,          // 42001
        WechatErrorCodes.RelatedAccessTokenInvalid,   // 42007
    ];

    private readonly IServiceProvider? _services;

    /// <summary>创建判定器（手工构造：无补偿失效能力，判定行为不变）。</summary>
    public WechatTokenInvalidationDetector()
        : this(null)
    {
    }

    /// <summary>创建判定器（DI 注入：启用 corp 槽补偿失效，见类型 remarks）。</summary>
    /// <param name="serviceProvider">服务提供器（用于解析 <see cref="IAppContextHolder"/>；
    /// <c>null</c> = 手工构造，补偿静默跳过）。</param>
    public WechatTokenInvalidationDetector(IServiceProvider? serviceProvider)
        => _services = serviceProvider;

    /// <summary>
    /// 同步预过滤：仅企业微信域名（或显式登记的自定义 BaseUrl 主机）请求参与判定，无关请求零捕获开销。
    /// </summary>
    /// <remarks>
    /// <b>P2-9</b>：本方法为<b>同步</b>预过滤，只有请求对象、无法按应用读取
    /// <c>AllowCustomBaseUrl</c>，故原实现硬编码 <c>weixin.qq.com</c> 会让私有化/网关部署
    /// （<c>AllowCustomBaseUrl = true</c>）静默失去 errcode 令牌恢复能力。
    /// 现在改为「官方域白名单 ∪ 注册期显式登记的自定义主机」（见
    /// <c>WechatCustomBaseUrlRegistry</c>）。放行与否只影响"是否参与判定"，
    /// 判定本身仍要求响应体为合法 JSON 且 errcode 命中集合，不会造成误判。
    /// </remarks>
    public bool ShouldInspect(HttpRequestMessage request)
    {
        var host = request.RequestUri?.Host;
        if (string.IsNullOrEmpty(host))
        {
            return false;
        }

        foreach (var domain in WechatApiHosts.AllowedBaseUrlDomains)
        {
            if (string.Equals(host, domain, StringComparison.OrdinalIgnoreCase)
                || host!.EndsWith("." + domain, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return WechatCustomBaseUrlRegistry.IsRegistered(host);
    }

    /// <summary>
    /// 判断响应是否表示令牌失效：解析 JSON 响应体的 errcode 并比对失效码集合
    /// （body 为空——超限 / chunked / 未声明 Content-Length——时回退状态码判定，即返回 false）。
    /// </summary>
    /// <remarks>
    /// 判定为真且 errcode 属 access_token 族时，先执行 corp 槽补偿失效（见类型 remarks）
    /// 再返回——补偿失败只记 Warning，不影响判定结果。
    /// </remarks>
    public async ValueTask<bool> IsTokenInvalidAsync(
        HttpResponseMessage response,
        ReadOnlyMemory<byte>? body,
        CancellationToken cancellationToken)
    {
        if (body is not { Length: > 0 } captured)
        {
            return false;
        }

        long errcode;
        try
        {
            using var doc = JsonDocument.Parse(captured);
            errcode = doc.RootElement.TryGetProperty("errcode", out var e) && e.TryGetInt64(out var code)
                ? code
                : -1;
        }
        catch (JsonException)
        {
            // 非企业微信 JSON 响应体（如网关错误页）不参与判定（判定器链路异常由组件侧统一降级）。
            return false;
        }

        var invalid = Array.IndexOf(InvalidErrcodes, errcode) >= 0;
        if (invalid)
        {
            await CompensateCorpScopeInvalidationAsync(errcode, cancellationToken).ConfigureAwait(false);
        }

        return invalid;
    }

    /// <summary>
    /// corp 槽补偿失效（F1）：对 access_token 族失效码，经环境应用上下文直取
    /// <see cref="IWechatAppContext.CorpTokenManager"/> 主动失效（按环境补齐 scope）。
    /// </summary>
    /// <remarks>
    /// 全程不抛（OCE 重抛除外）：环境无应用 / 非企微上下文 / 自建应用（无 corp 管理器）/
    /// 手工构造（<c>_services == null</c>）均静默跳过；失效动作幂等，失败仅退化为无自愈现状。
    /// </remarks>
    private async Task CompensateCorpScopeInvalidationAsync(long errcode, CancellationToken cancellationToken)
    {
        try
        {
            if (Array.IndexOf(AccessTokenFamilyErrcodes, errcode) < 0)
            {
                return;
            }

            var holder = _services?.GetService<IAppContextHolder>();
            if (holder?.Current is not IWechatAppContext context)
            {
                return;
            }

            if (context.CorpTokenManager is not { } corpManager)
            {
                return;
            }

            // 无需读取环境 authCorpId：CorpTokenManager.InvalidateTokenAsync 覆写自会按
            // 环境（WechatCorpContext）补齐 scope；环境不存在时退化为 default 槽（no-op，无害）。
            await corpManager.InvalidateTokenAsync(null, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _services?.GetService<ILoggerFactory>()?.CreateLogger(nameof(WechatTokenInvalidationDetector))
                ?.LogWarning(ex, "corp 令牌补偿失效失败（退化为无自愈现状，不影响判定结果）。");
        }
    }

    /// <summary>
    /// 记录令牌失效判定结果（由组件恢复链路在 <see cref="IsTokenInvalidAsync"/> 返回 true 后调用）。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="tokenOwner">令牌归属域（取 <see cref="WorkMetrics.TokenOwners"/> 常量）。</param>
    /// <param name="success">恢复是否成功。</param>
    /// <remarks>
    /// 本方法为便利方法，供宿主或测试在恢复链路完成后显式调用。
    /// 上游组件 <c>TokenManagerBase</c> 已自带 <c>mud.token.refresh</c> 指标记录刷新耗时与结果，
    /// 此处补充微信侧的归属域维度。
    /// </remarks>
    public static void RecordInvalidation(string appKey, string tokenOwner, bool success)
        => WorkMetricsHelper.RecordTokenInvalidation(appKey, tokenOwner, success ? "success" : "failure");
}
