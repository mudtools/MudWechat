// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.Configuration;

/// <summary>
/// 跨产品线应用配置基座：承载「所有产品线同义」的公共形状与通用校验。
/// </summary>
/// <remarks>
/// <para>
/// <b>职责边界</b>：本基类只描述「与产品线无关的配置面」（应用键、入口域名、超时、令牌阈值、默认应用标记）
/// 与三条通用校验（AppKey 形状 / 数值范围 / BaseUrl HTTPS + 白名单）。产品线专属字段与其互斥必填组合
/// 由派生类在 <see cref="Validate"/> 覆写中追加。
/// </para>
/// <para>
/// <b>AOT 约束</b>：不使用 <c>required</c>（ConfigurationBinder 以 <c>new T()</c> 构造 ⇒ <c>CS9035</c>），
/// 必填校验统一走 <see cref="Validate"/>；绑定走源生成器（<c>EnableConfigurationBindingGenerator</c>），
/// 调用侧必须使用 <c>Configure&lt;T&gt;(o =&gt; section.Bind(o))</c>，不得使用
/// <c>Configure&lt;T&gt;(IConfiguration)</c> 反射重载。
/// </para>
/// <para>
/// <b>审计约束</b>：本文件的公开可写属性与派生类属性同属配置面，必须一并纳入
/// <c>scripts/audit-config-keys.ps1</c> 的 <c>$configFiles</c> 清单（属性上移到本文件后<b>脱离扫描</b>
/// 即为门禁盲区）。
/// </para>
/// </remarks>
public abstract class WechatAppConfigBase
{
    /// <summary>
    /// 应用唯一标识（用于在代码中引用此应用）。
    /// </summary>
    /// <remarks>示例值："default"、"mp-main"。AppKey 必须唯一——重复 AppKey 会导致命名 HttpClient
    /// 与令牌管理器注册冲突。</remarks>
    public string AppKey { get; set; } = string.Empty;

    /// <summary>
    /// API 入口点（留空时由 <see cref="DefaultBaseUrl"/> 提供产品线默认域名）。
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// 是否允许自定义 BaseUrl。默认 <c>false</c>（SSRF 防线：非白名单域名校验失败）。
    /// </summary>
    public bool AllowCustomBaseUrl { get; set; }

    /// <summary>
    /// 请求超时时间（单位：秒，默认 30）。
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// 令牌提前刷新阈值（单位：秒，默认 300）。
    /// </summary>
    /// <remarks>阈值同源：恢复阈值 = 缓存有效性阈值 = 本配置，禁止第二套阈值。</remarks>
    public int TokenRefreshThreshold { get; set; } = 300;

    /// <summary>
    /// 是否为默认应用。多应用注册时必须存在且仅存在一个默认应用
    /// （AppKey 等于 "default" 的应用会被自动推断为默认）。
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>本产品线的默认 API 域名（由派生类提供，如企业微信 / 公众号）。</summary>
    protected abstract string DefaultBaseUrl { get; }

    /// <summary>
    /// 校验通用配置面；派生类须先调用 <c>base.Validate()</c> 再追加自身规则。
    /// </summary>
    /// <exception cref="InvalidOperationException">配置非法时抛出（启动阶段即失败）。</exception>
    public virtual void Validate()
    {
        if (string.IsNullOrWhiteSpace(AppKey))
        {
            throw new InvalidOperationException("AppKey 不能为空。");
        }

        // P1-7：键形状校验（单点收敛，全部产品线入口 + AppManager 均经此）。
        WechatAppKeyValidator.Validate(AppKey);

        if (TimeoutSeconds < 1 || TimeoutSeconds > 300)
        {
            throw new InvalidOperationException($"应用 {AppKey} 的 TimeoutSeconds 必须在 1-300 秒之间。");
        }

        if (TokenRefreshThreshold < 60 || TokenRefreshThreshold > 3600)
        {
            throw new InvalidOperationException($"应用 {AppKey} 的 TokenRefreshThreshold 必须在 60-3600 秒之间。");
        }

        ValidateBaseUrl();
    }

    /// <summary>
    /// 返回掩码后的配置描述（AppKey + BaseUrl；密钥永不写入日志）。
    /// </summary>
    /// <returns>配置摘要文本。</returns>
    public override string ToString()
        => $"{GetType().Name}(AppKey={AppKey}, BaseUrl={BaseUrl})";

    /// <summary>
    /// 校验 BaseUrl：留空回落产品线默认域名；必须为绝对 HTTPS；非白名单域名须显式放开
    /// <see cref="AllowCustomBaseUrl"/>。
    /// </summary>
    private void ValidateBaseUrl()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            BaseUrl = DefaultBaseUrl;
        }

        if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out var uri) ||
            !string.Equals(uri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"应用 {AppKey} 的 BaseUrl 必须是绝对 HTTPS 地址：{BaseUrl}");
        }

        if (!AllowCustomBaseUrl)
        {
            var host = uri.Host;
            var allowed = WechatApiHosts.AllowedBaseUrlDomains.Any(domain =>
                string.Equals(host, domain, StringComparison.OrdinalIgnoreCase) ||
                host.EndsWith("." + domain, StringComparison.OrdinalIgnoreCase));
            if (!allowed)
            {
                throw new InvalidOperationException(
                    $"应用 {AppKey} 的 BaseUrl 域名 {host} 不在白名单内" +
                    $"（{string.Join(", ", WechatApiHosts.AllowedBaseUrlDomains)}）；" +
                    "如需自定义域名请显式设置 AllowCustomBaseUrl = true。");
            }
        }
    }
}
