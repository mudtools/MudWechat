// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Services.Authorization;

/// <summary>
/// 授权编排策略（配置节 <c>WechatAuthorization</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>唯一编排策略承载者</b>：编排服务为 Singleton、跨应用共享，故编排策略不落到
/// <see cref="Abstractions.Configuration.WechatAppConfig"/>（避免双份真值）。
/// </para>
/// <para>
/// 校验统一走 <see cref="Validate"/>（启动期快速失败）。
/// </para>
/// </remarks>
public sealed class WechatAuthorizationOptions
{
    /// <summary>配置节名称。</summary>
    public const string SectionName = "WechatAuthorization";

    /// <summary>是否使用 v2 授权接口（<c>/v2/get_permanent_code</c>、<c>/v2/get_auth_info</c>）。默认 <c>true</c>。</summary>
    public bool UseV2AuthApi { get; set; } = true;

    /// <summary>回调收到 <c>create_auth</c> / <c>reset_permanent_code</c> 时是否自动换码落库。默认 <c>true</c>。</summary>
    public bool AutoExchangeAuthCode { get; set; } = true;

    /// <summary>默认授权回跳地址（redirect_uri）；为空时调用方必须显式传入。</summary>
    public string AuthorizationRedirectUri { get; set; } = string.Empty;

    /// <summary>默认应用键；为 <c>null</c> / 空时使用注册的默认应用。</summary>
    public string? DefaultAppKey { get; set; }

    /// <summary><c>authCode → authCorpId</c> 结果记忆 TTL（秒），默认 600。</summary>
    public int AuthCodeMemoTtlSeconds { get; set; } = 600;

    /// <summary>校验取值合法性（配置错误抛出 <see cref="InvalidOperationException"/>）。</summary>
    public void Validate()
    {
        if (AuthCodeMemoTtlSeconds < 60 || AuthCodeMemoTtlSeconds > 3600)
        {
            throw new InvalidOperationException(
                $"WechatAuthorizationOptions.AuthCodeMemoTtlSeconds 必须在 60-3600 秒之间，当前：{AuthCodeMemoTtlSeconds}。");
        }

        if (!string.IsNullOrWhiteSpace(AuthorizationRedirectUri))
        {
            if (!Uri.TryCreate(AuthorizationRedirectUri, UriKind.Absolute, out var uri) ||
                !string.Equals(uri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"WechatAuthorizationOptions.AuthorizationRedirectUri 必须是绝对 HTTPS 地址：{AuthorizationRedirectUri}");
            }
        }
    }
}