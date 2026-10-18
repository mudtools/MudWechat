// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>支付押金（预授权）</b>域中<b>需商户证书</b>的 3 支端点（撤销 / 消费押金 / 申请退款）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（APIv2 合作伙伴侧，2026-10-18 逐页核验）：
/// 撤销订单 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011986741"/>、
/// 消费押金 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011986801"/>、
/// 申请退款 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011987251"/>。
/// 三页的「是否需要证书」栏均为<b>「是，详见证书使用」</b>。</para>
/// <para><b>为什么手写</b>：客户端证书按<b>商户</b>绑定，与「一条命名客户端一个连接池」的生成式模型冲突
/// （理由同 <see cref="IWechatPayV2SecureService"/>），故由 <c>WechatPayV2SecureInvoker</c>
/// 走逐商户证书通道；报文规则与声明式管道完全同源。</para>
/// <para><b>域名是 <c>api.mch</c> 而非 <c>api2.mch</c></b>：官方给这三页的请求域名同为
/// <c>https://api.mch.weixin.qq.com</c> ⇒ 本接口<b>不</b>复用 <see cref="IWechatPayV2SecureService"/>
/// 的证书域名（那条是 <c>/secapi/pay/*</c> 与账单下载专用的 <c>api2.mch</c>）。</para>
/// <para><b>不出密钥</b>（PAY-B7）：异常消息只含商户键、路由与官方返回字段名，
/// 不含密钥、<c>sign</c>、签名原文或证书材料。</para>
/// </remarks>
public interface IWechatPayV2DepositSecureService
{
    /// <summary>
    /// 撤销押金订单（APIv2，需证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011986741"/>。
    /// </summary>
    /// <param name="request">撤销请求体，见 <see cref="V2DepositReverseRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>撤销结果，见 <see cref="V2DepositReverseResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/deposit/reverse</c>；XML；<c>sign_type</c> 仅 HMAC-SHA256。</para>
    /// <para><b>时序要求（官方硬性）</b>：「调用支付接口后请勿立即调用撤销订单 API，建议支付后至少 15s 后再调用」。</para>
    /// <para><b>语义</b>：用户支付失败⇒订单关闭；支付成功⇒押金<b>全部原路退回</b>用户并在到账后下发通知；
    /// <c>USERPAYING</c>（用户支付中）的订单不允许撤销。</para>
    /// <para><b>应答无 <c>recall_result</c></b>：与基础支付的 <c>/secapi/pay/reverse</c> 形态不同，本支只回两级标识。</para>
    /// </remarks>
    Task<V2DepositReverseResponse> ReverseAsync(
        V2DepositReverseRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 消费押金（APIv2，需证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011986801"/>。
    /// </summary>
    /// <param name="request">消费请求体，见 <see cref="V2DepositConsumeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>消费受理结果，见 <see cref="V2DepositConsumeResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/deposit/consume</c>；XML；<c>sign_type</c> 仅 HMAC-SHA256。</para>
    /// <para><b>金额规则</b>：扣费区间为 <c>(0, 押金金额]</c> 且 <c>Σ消费金额 ≤ 押金金额</c>；
    /// 微信侧扣除手续费后结算给商户、余额原路退还用户，并生成押金退款交易记录。</para>
    /// <para><b>受理 ≠ 完成</b>：<c>SETTLING</c> 表示「押金消费受理中」，
    /// <c>SETTLEMENTERROR</c> 表示解除冻结失败（手续费账户余额不足或无结算权限），最终态以查单为准。</para>
    /// <para><b><c>HIGH_RISK</c></b>：商户收款受限时官方直接拒绝押金消费，处置是去商户平台「账户中心-违约记录」查看
    /// （与 <see cref="IWechatPayV2RiskService"/> 的违规记录查询同源）。</para>
    /// </remarks>
    Task<V2DepositConsumeResponse> ConsumeAsync(
        V2DepositConsumeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 申请押金退款（APIv2，需证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011987251"/>。
    /// </summary>
    /// <param name="request">退款请求体，见 <see cref="V2DepositRefundRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>退款<b>受理</b>结果，见 <see cref="V2DepositRefundResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/deposit/refund</c>；XML；<c>sign_type</c> 仅 HMAC-SHA256；
    /// 以 <c>transaction_id</c> 单键定位（请求面<b>无</b> <c>out_trade_no</c>）。</para>
    /// <para><b>频率上限</b>：正常申请 <b>150 qps</b>，错误或无效申请 <b>6 qps</b>（押金族唯一给出数字的一支）；
    /// <c>FREQUENCY_LIMITED</c> 的官方处置是「该笔退款为受理中，请查单确认或<b>降低频率原单重试</b>，重试勿换单号」。</para>
    /// <para><b>退款期限与次数</b>：交易超过一年不可退款（<c>TRADE_OVERDUE</c>）；单笔交易可分多次退，
    /// 同一订单部分退款不超过 50 次；重试<b>必须复用原 <c>out_refund_no</c></b> —— 换号导致的多次退款责任由商户承担。</para>
    /// <para><b>受理语义</b>：<c>result_code = SUCCESS</c> 仅代表申请接收成功，退款结果以
    /// <see cref="IWechatPayV2DepositService.RefundQueryAsync"/> 为准。</para>
    /// </remarks>
    Task<V2DepositRefundResponse> RefundAsync(
        V2DepositRefundRequest request, CancellationToken cancellationToken = default);
}
