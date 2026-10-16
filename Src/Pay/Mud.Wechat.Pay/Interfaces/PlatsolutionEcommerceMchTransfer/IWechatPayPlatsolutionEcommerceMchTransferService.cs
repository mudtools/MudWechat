// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceMchTransfer;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台服务商解决方案 - MchTransfer」域 SDK（3 端点）。
/// </summary>
/// <remarks>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PlatsolutionEcommerce", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPlatsolutionEcommerceMchTransferService
{
    /// <summary>
    /// 端点。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504198"/>。
    /// </summary>
    [Post("/v3/platsolution/ecommerce/mch-transfer/transfer-bills")]
    Task<MchTransferResponse> CreateMchTransferAsync(
        [Body] MchTransferRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 端点。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504200"/>。
    /// </summary>
    [Get("/v3/platsolution/ecommerce/mch-transfer/transfer-bills/bill-id/{bill_id}")]
    Task<MchTransferResponse> QueryMchTransferByBillIdAsync(
        [Path] string bill_id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 端点。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504199"/>。
    /// </summary>
    [Get("/v3/platsolution/ecommerce/mch-transfer/transfer-bills/out-bill-no/{out_bill_no}")]
    Task<MchTransferResponse> QueryMchTransferByOutBillNoAsync(
        [Path] string out_bill_no,
        CancellationToken cancellationToken = default);
}
