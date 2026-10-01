// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Enums;

namespace Mud.Wechat.Work.Abstractions.Configuration;

/// <summary>
/// 企业微信应用配置（对齐 <c>FeishuAppConfig</c> 的嵌套 Options 治理）。
/// 由原 <c>WechatWorkClientOptions</c> 更名迁移。
/// </summary>
/// <remarks>
/// <para>
/// AOT：配置 DTO 不使用 <c>required</c>（ConfigurationBinder 以 <c>new T()</c> 构造，
/// required 触发 CS9035），必填校验统一走 <see cref="Validate"/>，在启动阶段按
/// <see cref="AppType"/> 校验互斥必填项组合，配置错误即时报错。
/// </para>
/// <para>
/// 回调推送密钥（<c>PushEncodingAESKey</c> / <c>PushToken</c>）属回调运维面，
/// 随 <c>Mud.Wechat.Work.Callback</c> 包配置，不进本配置。
/// </para>
/// </remarks>
public class WechatAppConfig
{
    /// <summary>
    /// 应用唯一标识（用于在代码中引用此应用）。
    /// </summary>
    /// <remarks>示例值："default"、"saas-suite"。AppKey 必须唯一——重复 AppKey
    /// 会导致命名 HttpClient 与令牌管理器注册冲突。</remarks>
    public string AppKey { get; set; } = string.Empty;

    /// <summary>
    /// 应用类型，决定必填项组合与令牌链（默认企业内部自建应用）。
    /// </summary>
    public WechatAppType AppType { get; set; } = WechatAppType.Internal;

    /// <summary>
    /// 企业 CorpId。
    /// </summary>
    /// <remarks>自建应用为本企业 CorpId；第三方/服务商应用为服务商企业 CorpId。</remarks>
    public string CorpId { get; set; } = string.Empty;

    /// <summary>
    /// 应用 AgentId（仅企业内部自建应用使用；业务可选，仅作展示/路由用途）。
    /// </summary>
    public string AgentId { get; set; } = string.Empty;

    /// <summary>
    /// 应用凭证密钥 AgentSecret（仅企业内部自建应用使用，即 corpsecret）。
    /// </summary>
    public string AgentSecret { get; set; } = string.Empty;

    /// <summary>
    /// 服务商 Secret（仅第三方应用/服务商代开发使用）。
    /// </summary>
    public string ProviderSecret { get; set; } = string.Empty;

    /// <summary>
    /// 第三方应用的 SuiteId（仅第三方应用/服务商代开发使用）。
    /// </summary>
    public string SuiteId { get; set; } = string.Empty;

    /// <summary>
    /// 第三方应用的 SuiteSecret（仅第三方应用/服务商代开发使用）。
    /// </summary>
    public string SuiteSecret { get; set; } = string.Empty;

    /// <summary>
    /// 企业微信 API 入口点（默认 <c>https://qyapi.weixin.qq.com</c>）。
    /// </summary>
    public string BaseUrl { get; set; } = Consts.DefaultBaseUrl;

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
    /// <remarks>D9 阈值同源：恢复阈值 = 缓存有效性阈值 = 本配置，禁止第二套阈值。</remarks>
    public int TokenRefreshThreshold { get; set; } = 300;

    /// <summary>
    /// 是否为默认应用。多应用注册时必须存在且仅存在一个默认应用
    /// （AppKey 等于 "default" 的应用会被自动推断为默认）。
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 按 <see cref="AppType"/> 校验互斥必填项组合，配置错误抛出
    /// <see cref="InvalidOperationException"/>（启动阶段即失败，避免运行期深埋错误）。
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(AppKey))
        {
            throw new InvalidOperationException("AppKey 不能为空。");
        }

        // P1-7：键形状校验（单点收敛，三入口 + AddApp 全部经此）。
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

        switch (AppType)
        {
            case WechatAppType.Internal:
                if (string.IsNullOrWhiteSpace(CorpId))
                    throw new InvalidOperationException($"自建应用 {AppKey} 缺少 CorpId。");
                if (string.IsNullOrWhiteSpace(AgentSecret))
                    throw new InvalidOperationException($"自建应用 {AppKey} 缺少 AgentSecret。");
                break;

            case WechatAppType.ThirdParty:
            case WechatAppType.Provider:
                if (string.IsNullOrWhiteSpace(CorpId))
                    throw new InvalidOperationException($"第三方/服务商应用 {AppKey} 缺少 CorpId（服务商企业）。");
                if (string.IsNullOrWhiteSpace(ProviderSecret))
                    throw new InvalidOperationException($"第三方/服务商应用 {AppKey} 缺少 ProviderSecret。");
                if (string.IsNullOrWhiteSpace(SuiteId))
                    throw new InvalidOperationException($"第三方/服务商应用 {AppKey} 缺少 SuiteId。");
                if (string.IsNullOrWhiteSpace(SuiteSecret))
                    throw new InvalidOperationException($"第三方/服务商应用 {AppKey} 缺少 SuiteSecret。");

                // K2：协议上「代开发模板 id 即 suite_id」，故不再提供独立的 TemplateId 配置项
                // ——原 TemplateId 仅被本方法用于「与非空 SuiteId 比对」，属死配置（无真实消费点，
                // audit-config-keys.ps1 判红）。删除后「不一致」这一非法状态在类型层面不可表达，
                // 比"启动期校验一致性"更强（make illegal states unrepresentable）。
                break;

            default:
                throw new InvalidOperationException($"应用 {AppKey} 的 AppType 取值无效：{AppType}。");
        }
    }

    private void ValidateBaseUrl()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            BaseUrl = Consts.DefaultBaseUrl;
        }

        if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out var uri) ||
            !string.Equals(uri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"应用 {AppKey} 的 BaseUrl 必须是绝对 HTTPS 地址：{BaseUrl}");
        }

        if (!AllowCustomBaseUrl)
        {
            var host = uri.Host;
            var allowed = Consts.AllowedBaseUrlDomains.Any(domain =>
                string.Equals(host, domain, StringComparison.OrdinalIgnoreCase) ||
                host.EndsWith("." + domain, StringComparison.OrdinalIgnoreCase));
            if (!allowed)
            {
                throw new InvalidOperationException(
                    $"应用 {AppKey} 的 BaseUrl 域名 {host} 不在白名单内" +
                    $"（{string.Join(", ", Consts.AllowedBaseUrlDomains)}）；" +
                    "如需自定义域名请显式设置 AllowCustomBaseUrl = true。");
            }
        }
    }

    /// <summary>
    /// 返回掩码后的配置描述（AppKey / AppType / CorpId；密钥永不写入日志）。
    /// </summary>
    public override string ToString()
        => $"WechatAppConfig(AppKey={AppKey}, AppType={AppType}, CorpId={CorpId}, BaseUrl={BaseUrl})";
}
