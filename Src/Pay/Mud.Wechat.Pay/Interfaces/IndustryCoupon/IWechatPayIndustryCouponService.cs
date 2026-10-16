// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.IndustryCoupon;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「行业券（出行优惠券）」域 SDK（1 端点）。
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
[HttpClientApi(RegistryGroupName = "IndustryCoupon", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayIndustryCouponService
{
    /// <summary>
    /// 出行券快捷切卡组件 Token。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553563"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715654"/>（普通服务商）。
    /// </summary>
    /// <param name="request">出行券快捷切卡组件 Token请求体，字段见 <see cref="IndustryCouponTransitCouponTokenRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>出行券快捷切卡组件 Token，见 <see cref="IndustryCouponTransitCouponTokenResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/industry-coupon/tokens</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】【渠道商】【从业机构】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="IndustryCouponTransitCouponTokenResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误，请检查重试 / 400 INVALID_REQUEST authinfo解析商户code失败 / 400 INVALID_REQUEST authinfo解析商户ID失败 / 400 INVALID_REQUEST OpenID有误，请检查后重试 / 400 INVALID_REQUEST 参数错误，没传入券列表 / 400 INVALID_REQUEST 单次请求只能传一个批次号 / 400 INVALID_REQUEST 券ID存在重复，请检查参数列表 / 400 INVALID_REQUEST 用户近五分钟没有领取该批次的券 / 400 INVALID_REQUEST 参数传入的券ID列表有误（不是该用户最近五分钟内领取的券） / 403 REQUEST_BLOCKED 调用方商户号非批次的创建商户或发券商户 / 429 FREQUENCY_LIMIT_EXCEED 频率过快，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/industry-coupon/tokens")]
    Task<IndustryCouponTransitCouponTokenResponse> CreateTransitCouponTokenAsync(
        [Body] IndustryCouponTransitCouponTokenRequest request,
        CancellationToken cancellationToken = default);
}
