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
/// 公共形状（AppKey / BaseUrl / AllowCustomBaseUrl / TimeoutSeconds / TokenRefreshThreshold / IsDefault
/// 与三条通用校验）由公用层 <see cref="WechatAppConfigBase"/> 承担；本类只声明企业微信专属字段
/// （AppType / CorpId / AgentId / AgentSecret / ProviderSecret / SuiteId / SuiteSecret）与按应用类型的
/// 互斥必填组合。
/// </para>
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
public class WechatAppConfig : WechatAppConfigBase
{
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

    /// <inheritdoc />
    protected override string DefaultBaseUrl => WechatApiHosts.WorkBaseUrl;

    /// <summary>
    /// 按 <see cref="AppType"/> 校验互斥必填项组合，配置错误抛出
    /// <see cref="InvalidOperationException"/>（启动阶段即失败，避免运行期深埋错误）。
    /// </summary>
    /// <remarks>通用校验（AppKey 形状 / 数值范围 / BaseUrl HTTPS + 白名单）由基类承担。</remarks>
    public override void Validate()
    {
        base.Validate();

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

    /// <summary>
    /// 返回掩码后的配置描述（AppKey / AppType / CorpId / BaseUrl；密钥永不写入日志）。
    /// </summary>
    public override string ToString()
        => $"WechatAppConfig(AppKey={AppKey}, AppType={AppType}, CorpId={CorpId}, BaseUrl={BaseUrl})";
}
