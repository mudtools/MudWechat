// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels;
using Mud.Wechat.OpenPlatform.DataModels.ServiceMarket;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「第三方服务市场」域 SDK（<b>3 端点</b>：服务用户有效期列表 / 服务用户有效期 / 用户已支付订单列表）。
/// </summary>
/// <remarks>
/// <para><b>令牌面</b>：本组恒 <c>component_access_token</c>（Query 参数名 <c>access_token</c>）——
/// 官方服务市场文档明写「对于普通业务，access_token 为第三方平台 component_access_token」，且三页均要求服务端调用。</para>
/// <para><b>为何不含 <c>login_auth</c></b>：官方「登录验证」页明写「本接口不支持第三方平台调用」，
/// 其令牌面是服务商自有 <c>access_token</c>（非本线的 component / authorizer 两键系）⇒ 本仓判为<b>非契约</b>、零声明，
/// 由守卫 SM 系列锁定（追踪号 SM-LOGINAUTH-NONCONTRACT-01）。</para>
/// <para><b>与小程序线自建服务市场区分</b>：小程序线持有 <c>/wxa/servicemarket</c> 与 <c>/wxa/servicemarketretrieve</c>
/// （小程序自调面），本域四条官方路由是第三方服务商面，令牌归属不同。</para>
/// <para><b>落位</b>：<c>Interfaces/ServiceMarket/IWechatOpenPlatformServiceMarketService.cs</c>；DTO 命名空间
/// <c>Mud.Wechat.OpenPlatform.DataModels.ServiceMarket</c>；<c>OpenPlatformModule.ServiceMarket</c>，注册组 <c>ServiceMarket</c>。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ServiceMarket", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.ComponentAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformServiceMarketService
{
    /// <summary>
    /// 获取服务用户有效期列表（<c>POST /wxa/servicemarket/service/get_service_buyer_list</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/service_market/interface/value_added/servicemarket_get_service_buyer_list.html"/>。
    /// </summary>
    /// <param name="request">请求体（服务 ID + 分页 + 购买方类型），见 <see cref="OpenPlatformServiceMarketBuyerListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>服务用户列表，见 <see cref="OpenPlatformServiceMarketBuyerListResponse"/>。</returns>
    /// <remarks>官方约束：仅第三方平台可调用，须在服务端调用；分页 <c>limit</c> 默认 10。</remarks>
    [Post("/wxa/servicemarket/service/get_service_buyer_list")]
    Task<OpenPlatformServiceMarketBuyerListResponse> GetServiceBuyerListAsync(
        [Body] OpenPlatformServiceMarketBuyerListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取服务用户有效期（<c>POST /wxa/servicemarket/service/get_service_buyer</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/service_market/interface/value_added/servicemarket_get_service_buyer.html"/>。
    /// </summary>
    /// <param name="request">请求体（服务 ID + 小程序 AppId 或用户 OpenId + 购买方类型），见 <see cref="OpenPlatformServiceMarketBuyerRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>服务用户信息，见 <see cref="OpenPlatformServiceMarketBuyerResponse"/>。</returns>
    /// <remarks>官方约束：仅第三方平台可调用，须在服务端调用；<c>appid</c>/<c>openid</c> 按 <c>buyer_type</c> 二选一。</remarks>
    [Post("/wxa/servicemarket/service/get_service_buyer")]
    Task<OpenPlatformServiceMarketBuyerResponse> GetServiceBuyerAsync(
        [Body] OpenPlatformServiceMarketBuyerRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户已支付订单列表（<c>POST /wxa/servicemarket/get_paid_order_list</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/service_market/interface/value_added/servicemarket_get_paid_order_list.html"/>。
    /// </summary>
    /// <param name="request">请求体（服务 ID + 过滤条件 + 分页），见 <see cref="OpenPlatformServiceMarketPaidOrderListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>已支付订单列表，见 <see cref="OpenPlatformServiceMarketPaidOrderListResponse"/>。</returns>
    /// <remarks>官方约束：仅第三方平台可调用，须在服务端调用；<c>phone</c> 属个人联系方式，<b>不得</b>落日志。</remarks>
    [Post("/wxa/servicemarket/get_paid_order_list")]
    Task<OpenPlatformServiceMarketPaidOrderListResponse> GetPaidOrderListAsync(
        [Body] OpenPlatformServiceMarketPaidOrderListRequest request,
        CancellationToken cancellationToken = default);
}
