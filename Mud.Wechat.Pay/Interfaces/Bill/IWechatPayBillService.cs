// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Bill;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「账单」域 SDK（APIv3 申请交易账单 / 申请资金账单，2 端点）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（普通商户文档中心，2026-10-09 逐页核验）：
/// 申请交易账单 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791907"/>（同内容另有 <c>4013071227</c>）、
/// 申请资金账单 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791908"/>（同内容另有 <c>4013071235</c>）。</para>
/// <para><b>申请 ≠ 下载</b>：本域两个端点只返回<b>下载地址</b>（<c>download_url</c>，5 分钟内有效、只可下载一次）；
/// 账单文件本身须经 <c>IWechatPayBillDownloadService</c> 拉取（见设计方案 §2.7）——
/// 该通道返回<b>不是 JSON</b>（先申请再下载文件流），故与声明式端点分家。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与
/// <see cref="IWechatPayTransactionsService"/> 一致，详见其 remarks。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Bill", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayBillService
{
    /// <summary>
    /// 申请交易账单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791907"/>
    /// （官方接口名 <c>DirectAPIv3BillTradeBill</c>；页面更新时间 2025.01.10）。
    /// </summary>
    /// <param name="billDate">账单日期（Query <c>bill_date</c>，必填 string(10)，格式 <c>yyyy-MM-DD</c>）。</param>
    /// <param name="billType">账单类型（Query <c>bill_type</c>，必填 string(32)）：<c>ALL</c> / <c>SUCCESS</c> / <c>REFUND</c> / <c>RECHARGE_REFUND</c>。</param>
    /// <param name="tarType">压缩类型（Query <c>tar_type</c>，选填 string(32)）：仅 <c>GZIP</c>（不传则返回明文账单）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>账单下载信息（<c>hash_type</c> / <c>hash_value</c> / <c>download_url</c>），见 <see cref="BillDownloadInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/bill/tradebill</c>；必带 <c>Accept: application/json</c>。</para>
    /// <para><b>业务限制（官方原文要点）</b>：① <c>bill_date</c> 仅支持<b>三个月内</b>账单、且当日账单次日 10:00 后才可申请；
    /// ② 同一账单文件须在 <c>download_url</c> 有效期内（<b>5 分钟</b>）下载且<b>只可下载一次</b>；
    /// ③ 下载后须用 <c>hash_value</c>（SHA1）校验完整性；④ 指定日期无可下载账单时返回 <c>BILL_NOT_EXIST</c>。</para>
    /// </remarks>
    [Get("/v3/bill/tradebill")]
    Task<BillDownloadInfoResponse> GetTradeBillAsync(
        [Query("bill_date")] string billDate,
        [Query("bill_type")] string billType,
        [Query("tar_type")] string? tarType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 申请资金账单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791908"/>
    /// （官方接口名 <c>DirectAPIv3BillFundFlowBill</c>；页面更新时间 2025.01.09）。
    /// </summary>
    /// <param name="billDate">账单日期（Query <c>bill_date</c>，必填 string(10)，格式 <c>yyyy-MM-DD</c>，仅支持三个月内）。</param>
    /// <param name="accountType">资金账户类型（Query <c>account_type</c>，选填 string(32)）：<c>BASIC</c>（默认）/ <c>OPERATION</c> / <c>FEES</c>。</param>
    /// <param name="tarType">压缩类型（Query <c>tar_type</c>，选填 string(32)）：仅 <c>GZIP</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>账单下载信息，见 <see cref="BillDownloadInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/bill/fundflowbill</c>；必带 <c>Accept: application/json</c>。</para>
    /// <para>业务限制同 <see cref="GetTradeBillAsync"/>（三个月内、下载地址 5 分钟有效且只可下载一次、SHA1 校验）。</para>
    /// </remarks>
    [Get("/v3/bill/fundflowbill")]
    Task<BillDownloadInfoResponse> GetFundFlowBillAsync(
        [Query("bill_date")] string billDate,
        [Query("account_type")] string? accountType = null,
        [Query("tar_type")] string? tarType = null,
        CancellationToken cancellationToken = default);
}
