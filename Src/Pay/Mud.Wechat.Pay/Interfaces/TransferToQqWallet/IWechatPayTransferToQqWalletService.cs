// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.TransferToQqWallet;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「转账到QQ钱包」域 SDK（3 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径。本域每支路由官方<b>只印一页</b>（不像营销/商户经营面存在同一路径的【普通商户】与【普通服务商】双页）
/// ⇒ <b>不存在</b>「同路由多页字段表比对」问题，一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域三支官方页<b>明印</b>端点级 QPS，且<b>三档不同</b>（100 / 200 / 20 次/s，逐支见各方法 remarks）
/// ⇒ 与相邻免确认授权域的两档也不同形，不得跨域套用限频数字。</para>
/// <para><b>本域零 query、零列表支</b>：发起走资源段，查询与撤销走 path 上的 <c>out_bill_no</c>（商户单号定位，微信侧单号
/// 在应答里返回）⇒ 撤销支的定位键只有商户单号一种，不存在「按微信单号撤销」的官方面。</para>
/// <para><b>与普通转账域隔离</b>：本域全部走 <c>/fund-app/mch-transfer/transfer-to-qq-wallet-bills/*</c>， 请求/应答多 <c>qq_wallet_openid</c>（收款方 QQ 钱包 OpenID）⇒ DTO 不得与 <see cref="IWechatPayTransferService"/> 互相套用。</para>
/// <para><b>三支应答各异</b>：下单 5 行含 <c>create_time</c>、撤销 5 行含 <c>update_time</c>、查询 13 行 ⇒ 三支<b>不同形</b>、各建一型；
/// 撤销支官方<b>印有</b>应答表且零请求体（定位键 <c>out_bill_no</c> 全在 path）⇒ 既不得返回裸 <c>Task</c>，也不得凭空造请求 DTO。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "TransferToQqWallet", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayTransferToQqWalletService
{
    /// <summary>
    /// 发起转账到QQ钱包。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014297386"/>（【普通商户】）。
    /// </summary>
    /// <param name="request">发起转账到QQ钱包请求体，字段见 <see cref="TransferToQqWalletBillCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>发起转账到QQ钱包，见 <see cref="TransferToQqWalletBillCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/fund-app/mch-transfer/transfer-to-qq-wallet-bills</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>（官方原样）：接口限频： 单个商户的频率限制为100次/s。</para>
    /// <para><b>应答</b>见 <see cref="TransferToQqWalletBillCreateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/fund-app/mch-transfer/transfer-to-qq-wallet-bills")]
    Task<TransferToQqWalletBillCreateResponse> CreateTransferToQqWalletBillAsync(
        [Body] TransferToQqWalletBillCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 转账到QQ钱包结果查询。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014297440"/>（【普通商户】）。
    /// </summary>
    /// <param name="outBillNo">【商户转账单号】 商户系统生成的转账单号，只能由数字、大小写字母组成，在商户系统内部唯一（官方 path <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>转账到QQ钱包结果查询，见 <see cref="TransferToQqWalletBillQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/fund-app/mch-transfer/transfer-to-qq-wallet-bills/{out_bill_no}</c>；path <c>out_bill_no</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>（官方原样）：接口限频： 单个商户的频率限制为200次/s。</para>
    /// <para><b>业务错误码</b>：404 NOT_FOUND 转账单不存在。</para>
    /// </remarks>
    [Get("/v3/fund-app/mch-transfer/transfer-to-qq-wallet-bills/{outBillNo}")]
    Task<TransferToQqWalletBillQueryResponse> QueryTransferToQqWalletBillAsync(
        [Path] string outBillNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤销转账到QQ钱包。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014297396"/>（【普通商户】）。
    /// </summary>
    /// <param name="outBillNo">【商户转账单号】 商户系统生成的转账单号，只能由数字、大小写字母组成，在商户系统内部唯一（官方 path <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>撤销转账到QQ钱包，见 <see cref="TransferToQqWalletBillCancelResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/fund-app/mch-transfer/transfer-to-qq-wallet-bills/{out_bill_no}/cancel</c>；path <c>out_bill_no</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>（官方原样）：接口限频： 单个商户的频率限制为20次/s。</para>
    /// <para><b>业务错误码</b>：404 NOT_FOUND 转账单不存在。</para>
    /// </remarks>
    [Post("/v3/fund-app/mch-transfer/transfer-to-qq-wallet-bills/{outBillNo}/cancel")]
    Task<TransferToQqWalletBillCancelResponse> CancelTransferToQqWalletBillAsync(
        [Path] string outBillNo,
        CancellationToken cancellationToken = default);
}
