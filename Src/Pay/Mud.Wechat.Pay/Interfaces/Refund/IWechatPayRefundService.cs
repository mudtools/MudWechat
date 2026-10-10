// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Refund;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「退款」域 SDK（APIv3 申请退款 / 查询退款 / 发起异常退款，3 端点）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（普通商户文档中心，2026-10-09 逐页核验）：
/// 退款申请 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791903"/>（同内容另有 <c>4013071036</c>）、
/// 查询单笔退款 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791904"/>、
/// 发起异常退款 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791905"/>。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与
/// <see cref="IWechatPayTransactionsService"/> 一致，详见其 remarks。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Refund", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayRefundService
{
    /// <summary>
    /// 申请退款。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791903"/>
    /// （官方接口名 <c>DirectAPIv3Refund</c>；页面更新时间 2025.01.09）。
    /// </summary>
    /// <param name="request">退款请求体，字段见 <see cref="RefundApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>退款单详情（状态多为 <c>PROCESSING</c>，最终结果另经回调 / 查询获取），见 <see cref="RefundResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/refund/domestic/refunds</c>；支持商户类型：<b>普通商户</b>；
    /// 必带 <c>Accept: application/json</c> 与 <c>Content-Type: application/json</c>。</para>
    /// <para><b>业务限制（官方原文要点）</b>：① <c>out_refund_no</c> 须 6-32 字符、同一商户号下唯一；
    /// ② 单笔订单累计退款金额不得超过订单总额（超限报 <c>REFUND_FEE_INVALID</c> / <c>PARAM_ERROR</c>）；
    /// ③ 退款申请为<b>异步受理</b>，返回的 <c>status</c> 多为 <c>PROCESSING</c>，
    /// 不得据本次应答判定退款成功；④ 退款结果经退款结果回调通知或「查询单笔退款」获取。</para>
    /// </remarks>
    [Post("/v3/refund/domestic/refunds")]
    Task<RefundResponse> CreateRefundAsync(
        RefundApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询单笔退款（按<b>商户退款单号</b>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791904"/>
    /// （官方接口名 <c>DirectAPIv3Refund/out-refund-no</c>；页面更新时间 2025.01.09）。
    /// </summary>
    /// <param name="outRefundNo">商户退款单号（路径参数）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>退款单详情，见 <see cref="RefundResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/refund/domestic/refunds/{out_refund_no}</c>；
    /// <b>无 query 参数</b>；必带 <c>Accept: application/json</c>。</para>
    /// <para><b>异步语义</b>：退款受理后须经本接口<b>轮询</b>至终态（<c>SUCCESS</c> / <c>CLOSED</c> / <c>ABNORMAL</c>），
    /// 或等待退款结果回调；<c>status = ABNORMAL</c> 时应调用 <see cref="ApplyAbnormalRefundAsync"/>。</para>
    /// </remarks>
    [Get("/v3/refund/domestic/refunds/{outRefundNo}")]
    Task<RefundResponse> QueryRefundAsync(
        [Path] string outRefundNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发起异常退款（退款单 <c>status = ABNORMAL</c> 时使用）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791905"/>
    /// （官方接口名 <c>DirectAPIv3Refund/apply-abnormal-refund</c>；页面更新时间 2024.12.30）。
    /// </summary>
    /// <param name="refundId">微信支付退款单号（路径参数，<b>非</b>商户退款单号）。</param>
    /// <param name="request">异常退款请求体，字段见 <see cref="AbnormalRefundRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>更新后的退款单详情，见 <see cref="RefundResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/refund/domestic/refunds/{refund_id}/apply-abnormal-refund</c>；
    /// 支持商户类型：<b>普通商户</b>；<c>refund_id</c> 为 path 参数。</para>
    /// <para><b>业务限制（官方原文要点）</b>：① 仅当退款单 <c>status = ABNORMAL</c>（用户账户异常 / 银行卡注销）
    /// 时可用；② 属<b>变更退款去向</b>的不可逆操作，须谨慎调用。</para>
    /// </remarks>
    [Post("/v3/refund/domestic/refunds/{refundId}/apply-abnormal-refund")]
    Task<RefundResponse> ApplyAbnormalRefundAsync(
        [Path] string refundId,
        AbnormalRefundRequest request,
        CancellationToken cancellationToken = default);
}
