// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「代小程序管理 · 域名与 DNS 预解析」子域 SDK（<b>9 端点</b>：查有效服务器域名 / 查有效 webview 域名 / 查预解析 / 取校验文件 / 改服务器域名 / 直改服务器域名 / 设预解析 / 改 webview / 直改 webview）。
/// </summary>
/// <remarks>
/// <para><b>令牌面</b>：全域以 <c>authorizer_access_token</c> 承载，Query 参数名恒 <c>access_token</c>（2026-10-17 逐页核验）。</para>
/// <para><b>动词陷阱</b>：<c>get_effective_domain</c>/<c>get_effective_webviewdomain</c>/<c>get_webviewdomain_confirmfile</c> 官方为 <b>POST</b>（含查询类），仅 <c>get_prefetchdnsdomain</c> 为 GET，逐页照抄。</para>
/// <para><b>业务限制</b>：服务器域名每类最多 200、webview 域名最多 300、预解析域名最多 5；修改族每月 50 次。<c>action=set</c> 覆盖、<c>add</c> 追加。<c>setwebviewdomain_directly</c> 须先经校验文件（否则 86103）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MiniProgramManage", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformMpDomainService
{
    /// <summary>
    /// 获取有效服务器域名（<c>get_effective_domain</c>，官方为 POST、请求体须非空对象 <c>{}</c>，否则 <c>44002</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/domain-management/getEffectiveServerDomain.html"/>。
    /// </summary>
    [Post("/wxa/get_effective_domain")]
    Task<OpenPlatformMpEffectiveDomainResponse> GetEffectiveServerDomainAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取有效 webview 域名（<c>get_effective_webviewdomain</c>，官方为 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/domain-management/getEffectiveJumpDomain.html"/>。
    /// </summary>
    [Post("/wxa/get_effective_webviewdomain")]
    Task<OpenPlatformMpEffectiveWebViewDomainResponse> GetEffectiveWebViewDomainAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取 DNS 预解析域名（<c>get_prefetchdnsdomain</c>，官方为 GET，最多 5）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/domain-management/getPrefetchDomain.html"/>。
    /// </summary>
    [Get("/wxa/get_prefetchdnsdomain")]
    Task<OpenPlatformMpPrefetchDomainResponse> GetPrefetchDomainAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取 webview 域名校验文件（<c>get_webviewdomain_confirmfile</c>，官方为 POST；文件名与内容不得修改，直改 webview 域前置校验）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/domain-management/getJumpDomainConfirmFile.html"/>。
    /// </summary>
    [Post("/wxa/get_webviewdomain_confirmfile")]
    Task<OpenPlatformMpWebviewConfirmFileResponse> GetWebViewDomainConfirmFileAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改服务器域名（<c>modify_domain</c>，以 <c>action</c> 分派 add/delete/set/get，每类最多 200，每月 50 次）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/domain-management/modifyServerDomain.html"/>。
    /// </summary>
    [Post("/wxa/modify_domain")]
    Task<OpenPlatformMpModifyDomainResponse> ModifyServerDomainAsync(
        [Body] OpenPlatformMpModifyDomainRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 直接覆盖式修改服务器域名（<c>modify_domain_directly</c>，请求体同 <c>modify_domain</c>，应答仅基础信封）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/domain-management/modifyServerDomainDirectly.html"/>。
    /// </summary>
    [Post("/wxa/modify_domain_directly")]
    Task<OpenPlatformResponse> ModifyServerDomainDirectlyAsync(
        [Body] OpenPlatformMpModifyDomainRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置 DNS 预解析域名（<c>set_prefetchdnsdomain</c>，覆盖式 set，最多 5，每月 50 次）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/domain-management/setPrefetchDomain.html"/>。
    /// </summary>
    [Post("/wxa/set_prefetchdnsdomain")]
    Task<OpenPlatformResponse> SetPrefetchDomainAsync(
        [Body] OpenPlatformMpSetPrefetchDomainRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改 webview 域名（<c>setwebviewdomain</c>，以 <c>action</c> 分派，最多 300）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/domain-management/modifyJumpDomain.html"/>。
    /// </summary>
    [Post("/wxa/setwebviewdomain")]
    Task<OpenPlatformResponse> SetWebViewDomainAsync(
        [Body] OpenPlatformMpWebViewDomainRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 直接覆盖式配置 webview 域名（<c>setwebviewdomain_directly</c>，须先经校验文件，否则 <c>86103</c>；应答回显 <c>webviewdomain</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/domain-management/modifyJumpDomainDirectly.html"/>。
    /// </summary>
    [Post("/wxa/setwebviewdomain_directly")]
    Task<OpenPlatformMpWebViewDomainDirectlyResponse> SetWebViewDomainDirectlyAsync(
        [Body] OpenPlatformMpWebViewDomainRequest request,
        CancellationToken cancellationToken = default);
}
