// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels.Ams;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「流量主代运营（AMS）」域 SDK · 服务商分账面（<b>3 端点</b>，恒为 <b>POST</b>，消费平台自身 <c>component_access_token</c>）。
/// </summary>
/// <remarks>
/// <para><b>为何与 <see cref="IWechatOpenPlatformAmsService"/> 分家</b>：本面 3 页「注意事项」均写 <c>access_token</c> 取
/// <c>component_access_token</c>（服务商平台自身令牌），而管理面 19 页取 <c>authorizer_access_token</c> ⇒ 令牌归属不同，
/// 必须拆成两支接口、两个注册组（<c>AmsShare</c>），否则接口级 <c>[Token]</c> 无法同时承载两种令牌值（守卫 AM-S1 锁定）。</para>
/// <para><b>路由面</b>：跨两条路由 <c>/wxa/getdefaultamsinfo</c>（查询，2 支）与 <c>/wxa/setdefaultamsinfo</c>（设置，1 支），各以 <c>action</c> query 分派。</para>
/// <para><b>落位</b>：<c>Interfaces/Ams/IWechatOpenPlatformAmsShareService.cs</c>；<c>OpenPlatformModule.AmsShare</c>，注册组 <c>AmsShare</c>。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "AmsShare", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.ComponentAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformAmsShareService
{
    /// <summary>
    /// 「查询自定义分账比例」（<c>agency_get_custom_share_ratio</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/percentage/GetCustomShareRatio.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>appid</c>），见 <see cref="OpenPlatformAmsShareRatioGetRequest"/>。</param>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_get_custom_share_ratio</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsShareRatioResponse"/>。</returns>
    [Post("/wxa/getdefaultamsinfo")]
    Task<OpenPlatformAmsShareRatioResponse> GetCustomShareRatioAsync(
        [Body] OpenPlatformAmsShareRatioGetRequest request,
        [Query("action")] string action = "agency_get_custom_share_ratio",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「设置自定义分账比例」（<c>agency_set_custom_share_ratio</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/percentage/SetCustomShareRatio.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>appid</c> + <c>share_ratio</c>），见 <see cref="OpenPlatformAmsShareRatioSetRequest"/>。</param>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_set_custom_share_ratio</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsResponse"/>。</returns>
    [Post("/wxa/setdefaultamsinfo")]
    Task<OpenPlatformAmsResponse> SetCustomShareRatioAsync(
        [Body] OpenPlatformAmsShareRatioSetRequest request,
        [Query("action")] string action = "agency_set_custom_share_ratio",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取服务商结算收入数据」（<c>get_agency_settled_revenue</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/settlement/GetAgencySettlement.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformAmsRevenuePageRequest"/>。</param>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>get_agency_settled_revenue</c>（无 <c>agency_</c> 前缀，照官方原文）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsSettlementResponse"/>。</returns>
    /// <remarks>请求体与应答形状与 <see cref="IWechatOpenPlatformAmsService.GetSettlementAsync"/> 逐字相同 ⇒ 两端点共用 <see cref="OpenPlatformAmsRevenuePageRequest"/> + <see cref="OpenPlatformAmsSettlementResponse"/>。</remarks>
    [Post("/wxa/getdefaultamsinfo")]
    Task<OpenPlatformAmsSettlementResponse> GetAgencySettledRevenueAsync(
        [Body] OpenPlatformAmsRevenuePageRequest request,
        [Query("action")] string action = "get_agency_settled_revenue",
        CancellationToken cancellationToken = default);
}
