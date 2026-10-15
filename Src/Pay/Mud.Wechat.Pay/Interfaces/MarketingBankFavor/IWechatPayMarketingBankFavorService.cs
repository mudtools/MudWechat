// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MarketingBankFavor;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「银行代金券」域 SDK（2 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径；同时有【普通商户】与【普通服务商】两页的端点，两页字段表经逐页指纹比对<b>完全同形</b>
/// ⇒ <b>一面一方法 + 双页文档引用</b>，只有字段表存在差异的端点才按「同路由多方法」分建。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>分页口径不统一</b>：本域既有 <c>offset</c>/<c>limit</c>（整型页码）也有 <c>page_size</c> + <c>offset</c> 形态，
/// 一律<b>照页</b>，不得互相套用；应答的 <c>total_count</c> 官方多为选填（<c>offset=0</c> 或结果为空时才回）。</para>
/// <para><b>PATCH 与 PUT</b>：批次/活动的改预算与改状态走 <b>PATCH</b>，终止走 <b>POST .../terminate</b>
/// ⇒ 「写操作动词」在本域不统一，照页不得合并。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "MarketingBankFavor", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMarketingBankFavorService
{
    /// <summary>
    /// 银行商家券营销活动发券。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014569793"/>（普通商户）。
    /// </summary>
    /// <param name="openid">【用户标识ID】 用户标识ID，用户在公众账号ID下的唯一标识。校验规则：该用户标识ID需要与接口传入中的公众账号ID有对应关系。（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">银行商家券营销活动发券请求体，字段见 <see cref="BankFavorMultiActivitySendRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>银行商家券营销活动发券，见 <see cref="BankFavorMultiActivitySendResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/bank-favor/users/{openid}/bank-multi-activity</c>；path <c>openid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BankFavorMultiActivitySendResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：403 RULE_LIMIT 用户已达最大领券次数 / 403 REQUEST_BLOCKED 参数错误，请检查批次参数 / 403 NOT_ENOUGH 批次预算耗尽 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 403 NOT_ENOUGH 账户余额不足，请充值 / 403 NOT_ENOUGH 发券超过单天限额 / 403 REQUEST_BLOCKED 仅在广告场景下发放批次 / 403 REQUEST_BLOCKED 商户号收款功能受限，无法发券 / 403 USER_ACCOUNT_ABNORMAL 用户未实名 / 400 PARAM_ERROR 批次号必填 / 400 PARAM_ERROR 活动ID必填 / 400 PARAM_ERROR OpenID必填 / 400 PARAM_ERROR 商户单据号必填 / 400 PARAM_ERROR AppID必填 / 400 PARAM_ERROR 商户号必填 / 400 INVALID_REQUEST 非法的商户号 / 403 REQUEST_BLOCKED 批次不支持跨商户发券 / 400 INVALID_REQUEST OpenID与AppID不匹配 / 400 APPID_MCHID_NOT_MATCH 商户号与AppID不匹配 / 403 MCH_NOT_EXISTS 商户号不合法 / 429 FREQUENCY_LIMIT_EXCEED 接口限频。</para>
    /// </remarks>
    [Post("/v3/marketing/bank-favor/users/{openid}/bank-multi-activity")]
    Task<BankFavorMultiActivitySendResponse> SendBankFavorMultiActivityAsync(
        [Path] string openid,
        [Body] BankFavorMultiActivitySendRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 银行商家券详情。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014569864"/>（普通商户）。
    /// </summary>
    /// <param name="openid">【用户标识ID】 用户标识ID是微信用户在公众账号ID下的唯一用户标识（公众账号ID不同，则获取到的用户标识ID就不同），可用于永久标记一个用户。 获取用户标识ID的方式（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="couponId">【代金券ID】 优惠券的主键，唯一定义此资源的标识（官方 path <c>coupon_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="appid">【公众账号ID】 微信为调用商户分配的公众账号ID，接口传入的appid应该为公众号的appid和小程序的appid（在 微信公众平台 申请）或APP的appid（在 微信开发平台 申请）。校验规则：1、该appid需要与接口传入中的openid有对应关系；2、该appid需要与调用接口的商户号（即请求头中的商户号）有绑定关系，若未绑定，可参考该指引完成绑定 （ 商家商户号与AppID账号关联管理）（官方 query <c>appid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>银行商家券详情，见 <see cref="BankFavorCouponQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/bank-favor/users/{openid}/coupons/{coupon_id}</c>；path <c>openid</c> / <c>coupon_id</c>，query <c>appid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>appid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 券ID必填 / 400 INVALID_REQUEST 非法的券ID / 400 INVALID_REQUEST 查询的商户非创建方商户 / 400 PARAM_ERROR OpenID必填 / 400 PARAM_ERROR AppID必填 / 400 INVALID_REQUEST OpenID与AppID不匹配 / 400 APPID_MCHID_NOT_MATCH 商户号与AppID不匹配 / 429 FREQUENCY_LIMIT_EXCEED 接口限频。</para>
    /// </remarks>
    [Get("/v3/marketing/bank-favor/users/{openid}/coupons/{couponId}")]
    Task<BankFavorCouponQueryResponse> QueryBankFavorCouponAsync(
        [Path] string openid,
        [Path] string couponId,
        [Query("appid")] string appid,
        CancellationToken cancellationToken = default);
}
