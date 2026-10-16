// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceDeposit;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台服务商解决方案 - Deposit」域 SDK（7 端点）。
/// </summary>
/// <remarks>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PlatsolutionEcommerce", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPlatsolutionEcommerceDepositService
{
    /// <summary>
    /// 端点。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504196"/>。
    /// </summary>
    [Get("/v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills/bill-id/{bill_id}")]
    Task<DepositResponse> QueryDepositAfterSalesCompensationByBillIdAsync(
        [Path] string bill_id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 端点。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504195"/>。
    /// </summary>
    [Get("/v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills/out-bill-no/{out_bill_no}")]
    Task<DepositResponse> QueryDepositAfterSalesCompensationByOutBillNoAsync(
        [Path] string out_bill_no,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 端点。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010905"/>。
    /// </summary>
    [Post("/v3/platsolution/ecommerce/mch-transfer/deposit-insurance-premiums-compensation-bills")]
    Task<DepositResponse> CreateDepositInsurancePremiumsCompensationAsync(
        [Body] DepositRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 端点。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010920"/>。
    /// </summary>
    [Get("/v3/platsolution/ecommerce/mch-transfer/deposit-insurance-premiums-compensation-bills/{out_bill_no}")]
    Task<DepositResponse> QueryDepositInsurancePremiumsByOutBillNoAsync(
        [Path] string out_bill_no,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 端点。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504202"/>。
    /// </summary>
    [Post("/v3/platsolution/ecommerce/mch-transfer/deposit-shipping-cost-compensation-bills")]
    Task<DepositResponse> CreateDepositShippingCostCompensationAsync(
        [Body] DepositRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 端点。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504203"/>。
    /// </summary>
    [Get("/v3/platsolution/ecommerce/mch-transfer/deposit-shipping-cost-compensation-bills/out-bill-no/{out_bill_no}")]
    Task<DepositResponse> QueryDepositShippingCostByOutBillNoAsync(
        [Path] string out_bill_no,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 端点。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504194"/>。
    /// </summary>
    [Post("/v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills")]
    Task<DepositResponse> CreateDepositAfterSalesCompensationAsync(
        [Body] DepositRequest request,
        CancellationToken cancellationToken = default);
}
