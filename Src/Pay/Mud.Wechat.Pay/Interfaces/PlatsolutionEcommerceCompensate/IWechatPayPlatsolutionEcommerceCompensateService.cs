// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceCompensate;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台服务商解决方案 - Compensate」域 SDK（5 端点）。
/// </summary>
/// <remarks>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PlatsolutionEcommerce", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPlatsolutionEcommerceCompensateService
{
    /// <summary>
    /// 请求赔付。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504175"/>。
    /// </summary>
    [Post("/v3/platsolution/ecommerce/mch-transfer/compensate-bills")]
    Task<CompensateResponse> CreateCompensateAsync(
        [Body] CompensateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询赔付结果（按微信支付赔付单号）。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504179"/>。
    /// </summary>
    [Get("/v3/platsolution/ecommerce/mch-transfer/compensate-bills/bill-id/{bill_id}")]
    Task<CompensateResponse> QueryByBillIdAsync(
        [Path] string bill_id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询赔付结果（按商户赔付单号）。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504178"/>。
    /// </summary>
    [Get("/v3/platsolution/ecommerce/mch-transfer/compensate-bills/out-bill-no/{out_bill_no}")]
    Task<CompensateResponse> QueryByOutBillNoAsync(
        [Path] string out_bill_no,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 请求撤销赔付。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504177"/>。
    /// </summary>
    [Post("/v3/platsolution/ecommerce/mch-transfer/compensate-bills/out-bill-no/{out_bill_no}/cancel")]
    Task<CompensateResponse> CancelCompensateAsync(
        [Path] string out_bill_no,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 请求赔付预下单。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504176"/>。
    /// </summary>
    [Post("/v3/platsolution/ecommerce/mch-transfer/compensate-bills/pre-transfer")]
    Task<CompensateResponse> PreTransferCompensateAsync(
        [Body] CompensateRequest request,
        CancellationToken cancellationToken = default);
}
