// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PayInstallment;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「微信分付曝光建议」域 SDK（1 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 1 支中<b>1 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：1 / 1 支官方页<b>印有</b>端点级数字（已原样抄入各方法 remarks），其余支<b>未印</b> ⇒ 未印者不得凭第三方 SDK 补数。</para>
/// <para><b>一支、官方只印商户页，且「查询」语义走 POST</b>：<c>POST /v3/pay/instal/exposure</c>（原文请求方式即 POST）⇒ 照页不得改成 GET。</para>
/// <para><b>官方把限速与耗时都印在正文</b>：「接口频率：<b>每个商户限流200/s</b>，上线前必须与微信支付运营确认」 +「接口耗时：200ms」⇒ 数字照页入 remarks，且「上线前须与运营确认」是<b>前置动作</b>，不得省略为纯技术阈值。</para>
/// <para><b>入参两键联动、SDK 不做本地校验</b>：<c>user_id_type</c> 决定用户标识口径，<c>openid</c> 官方标<b>选填</b> （传 OPENID 口径时才需给出）⇒ 两者均以可空参数承载，联动关系写在注释而<b>不</b>做成必传参数或抛异常。</para>
/// <para><b>应答 <c>repayment_info</c> 是 array[object] 且官方给元素两键</b>（<c>repayment_type</c>、 <c>exist_cash_off_coupon</c>）⇒ 建元素类型而非 <c>JsonElement</c>；根键 <c>recommend_exposure</c> 是必填布尔， 含义是「<b>建议曝光</b>」而非「用户已开通分付」⇒ 不得据其判定开通态，只作曝光决策入参。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PayInstallment", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPayInstallmentService
{
    /// <summary>
    /// 获取微信分付曝光建议。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4016196762"/>（【普通商户】）。
    /// </summary>
    /// <param name="request">获取微信分付曝光建议请求体，字段见 <see cref="PayInstallmentExposureRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>获取微信分付曝光建议，见 <see cref="PayInstallmentExposureResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/pay/instal/exposure</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率：每个商户限流200/s，上线前必须与微信支付运营确认。</para>
    /// <para><b>应答</b>见 <see cref="PayInstallmentExposureResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR：参数错误 / 400 INVALID_REQUEST：HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR：验证不通过 / 500 SYSTEM_ERROR：系统异常，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/pay/instal/exposure")]
    Task<PayInstallmentExposureResponse> QueryPayInstallmentExposureAsync(
        [Body] PayInstallmentExposureRequest request,
        CancellationToken cancellationToken = default);
}
