// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>需双向证书</b>的 5 支端点（申请退款 / 退款 v2 / 撤销 / 下载对账单 / 下载资金账单）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么单独立接口而不是并入 <see cref="IWechatPayV2Service"/></b>：本组端点走
/// <c>api2.mch.weixin.qq.com</c> 且 TLS 握手期必须呈现<b>本商户</b>的 <c>apiclient_cert.p12</c>；
/// 而声明式生成的客户端由 <c>IHttpClientFactory</c> 按<b>客户端名</b>缓存主 Handler 并复用连接池，
/// 一个名字只能绑一份证书 ⇒ 多商户共用即「商户 A 的请求跑在商户 B 的 TLS 会话上」。
/// 故本组由 <c>WechatPayV2SecureService</c> 逐商户通道承载（<b>每商户一条独立连接池</b>），
/// 报文与签名规则与 <see cref="IWechatPayV2Service"/> 完全一致。
/// </para>
/// <para><b>凭据前提</b>：须为该商户配置 <c>ApiV2KeySecretName</c> 与 <c>ApiV2CertificateSecretName</c>
/// （口令默认取商户号，改过则另配 <c>ApiV2CertificatePasswordSecretName</c>）；缺失即 fail-fast。</para>
/// <para><b>证书下载</b>：官方 <c>certdownload</c> 属另一条取证书的路，且要求应答验签，
/// 不在本接口内（本 SDK 由宿主从商户平台下载后登记进 <c>ISecretProvider</c>）。</para>
/// </remarks>
public interface IWechatPayV2SecureService
{
    /// <summary>
    /// 申请退款（APIv2，需双向证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935217"/>。
    /// </summary>
    /// <param name="request">退款请求体，见 <see cref="V2RefundRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>退款结果，见 <see cref="V2RefundResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/secapi/pay/refund</c>；XML 报文，MD5/HMAC-SHA256 签名，
    /// <b>需双向证书</b>。</para>
    /// <para><b>业务特征</b>：退款为<b>异步受理</b>，返回仅表示请求已接收，实际结果需经查询退款确认。</para>
    /// </remarks>
    Task<V2RefundResponse> RefundAsync(V2RefundRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 申请退款 V2（APIv2，需双向证书；服务商分账退款形态）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011983308"/>。
    /// </summary>
    /// <param name="request">退款请求体，见 <see cref="V2RefundV2Request"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>退款结果，见 <see cref="V2RefundV2Response"/>。</returns>
    /// <remarks><b>官方契约</b>：<b>POST</b> <c>/secapi/pay/refundv2</c>；XML + 双向证书。</remarks>
    Task<V2RefundV2Response> RefundV2Async(
        V2RefundV2Request request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤销订单（APIv2，需双向证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011937361"/>。
    /// </summary>
    /// <param name="request">撤销请求体，见 <see cref="V2ReverseRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>撤销结果，见 <see cref="V2ReverseResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/secapi/pay/reverse</c>；XML + 双向证书。</para>
    /// <para><b>业务场景</b>：交易返回失败或超时时撤销；已支付成功则转退款，未成功则直接关单。</para>
    /// </remarks>
    Task<V2ReverseResponse> ReverseAsync(V2ReverseRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 下载对账单（APIv2，需双向证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935219"/>。
    /// </summary>
    /// <param name="request">下载账单请求体，见 <see cref="V2DownloadBillRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>账单文件内容（CSV 原文；失败时为官方 XML 错误报文原文）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/pay/downloadbill</c>（证书域名 <c>api2.mch</c>）；XML 请求体。</para>
    /// <para><b>返回形态陷阱</b>：成功返回 <b>CSV 文本</b>而非 XML ⇒ 不做应答验签，
    /// 调用方须自行判别首行是否为 <c>`</c> 转义的交易明细，或在遇到 <c>&lt;xml&gt;</c> 时按失败处理。</para>
    /// </remarks>
    Task<string> DownloadBillAsync(
        V2DownloadBillRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 下载资金账单（APIv2，需双向证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935220"/>。
    /// </summary>
    /// <param name="request">下载资金账单请求体，见 <see cref="V2DownloadFundFlowRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>资金账单文件内容（CSV 原文；失败时为官方 XML 错误报文原文）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/pay/downloadfundflow</c>（证书域名 <c>api2.mch</c>）；XML + 双向证书。</para>
    /// <para><b>与对账单的区别</b>：对账单记交易流水，资金账单记商户账户资金变动（含退款、提现等）。</para>
    /// </remarks>
    Task<string> DownloadFundFlowAsync(
        V2DownloadFundFlowRequest request, CancellationToken cancellationToken = default);
}
