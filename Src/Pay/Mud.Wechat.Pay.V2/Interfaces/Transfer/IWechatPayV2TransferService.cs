// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 企业付款族（<b>4 支端点，全部需商户 API 证书</b>）：
/// 企业付款到零钱 / 查询企业付款 / 向员工付款 / 查询向员工付款。
/// </summary>
/// <remarks>
/// <para>
/// <b>形态与理由</b>：与 <see cref="IWechatPayV2RedPackService"/> 同因（证书按商户绑定 ⇒ 逐商户独立通道），
/// 域名同为官方给的 <c>api.mch</c>。本接口<b>不声明 <c>[Token]</c>、不带 <c>[HttpClientApi]</c></b>，
/// 签名与应答验签由 <c>WechatPayV2SecureInvoker</c> 完成。
/// </para>
/// <para>
/// <b>本族是出资操作</b>：企业付款到零钱与向员工付款都<b>直接扣商户可用余额</b>，
/// 幂等键分别是 <c>partner_trade_no</c> ⇒ <b>失败重试必须沿用原单号</b>；
/// <c>SYSTEMERROR</c> 下换单号即重复付款，这是本族最重要的接入纪律。
/// </para>
/// <para>
/// <b>双签</b>：<see cref="PayWorkWxPocketAsync"/> 除 <c>sign</c> 外还要求 <c>workwx_sign</c>
/// （密钥是 <c>agentid</c> 对应的企业微信自建应用 Secret，经 <c>IWechatPayV2WorkWxSecretProvider</c> 桥接）；
/// 查询两支不需要。口径见 <c>WechatPayV2WorkWxSigner</c>。
/// </para>
/// <para>
/// <b>敏感信息</b>（PAY-B7 同线红线）：<c>openid</c> / <c>desc</c> / <c>risk_info</c> 属用户身份与资金说明，
/// SDK 不落日志、不进异常消息。
/// </para>
/// </remarks>
public interface IWechatPayV2TransferService
{
    /// <summary>
    /// 企业付款到零钱（APIv2，需商户证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011989673"/>。
    /// </summary>
    /// <param name="request">付款请求体，见 <see cref="V2TransferToBalanceRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>付款结果，含 <c>payment_no</c> 与 <c>payment_time</c>，见 <see cref="V2TransferToBalanceResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/mmpaymkttransfers/promotion/transfers</c>（域名 <c>api.mch</c>）；
    /// XML 报文 + 报文内 <c>sign</c> + <b>双向证书</b>。</para>
    /// <para><b>与 APIv3 的区别</b>：本支是 APIv2 老版「商家转账到零钱」；新商户接入的是
    /// APIv3 的转账明细接口（<c>Mud.Wechat.Pay</c> 的 Transfer 域），两者字段面与受理模型不同，不要混用。</para>
    /// <para><b>业务限制</b>（官方本页）：频率 <b>100 次/秒</b>，同一笔订单重试间隔 <b>≥ 1 秒</b>；
    /// 金额区间 <b>最低 1 元（100 分）至单笔 10 万元</b>，未实名用户收款门槛更高（≥ 3 元），
    /// 低于下限官方回 <c>AMOUNT_LIMIT</c>；调用方 <b>须先在商户平台配置 IP 白名单</b>，未配置直接被判失败。
    /// 需先开通「企业付款到零钱」权限并保证可用余额充足。</para>
    /// <para><b>不支持公钥证书</b>：本支只认 <c>apiclient_cert.p12</c> 双向证书，
    /// 微信支付的「公钥证书」形态对本族端点无效 ⇒ 凭据面必须给 p12。</para>
    /// </remarks>
    Task<V2TransferToBalanceResponse> TransferToBalanceAsync(
        V2TransferToBalanceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询企业付款到零钱记录（APIv2，需商户证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011989767"/>。
    /// </summary>
    /// <param name="request">查询请求体，见 <see cref="V2GetTransferInfoRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>付款记录，见 <see cref="V2GetTransferInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/mmpaymkttransfers/gettransferinfo</c>；XML + <b>双向证书</b>。</para>
    /// <para><b>频率</b>：官方本页标注 <b>30 次/秒</b>（低于付款支的 100 次/秒），且同样<b>不支持公钥证书</b>形态。</para>
    /// <para><b>字段名陷阱</b>：查询族请求用 <c>appid</c> + <c>mch_id</c>，而付款族用 <c>mch_appid</c> + <c>mchid</c>
    /// ——官方两页参数名即不同，照抄而非统一。</para>
    /// </remarks>
    Task<V2GetTransferInfoResponse> GetTransferInfoAsync(
        V2GetTransferInfoRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 向企业微信成员付款（APIv2，需商户证书 <b>且需 <c>workwx_sign</c> 双签</b>）。
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90278"/>
    /// （微信支付商户平台「商家转账到零钱 · 向员工付款」分支）。
    /// </summary>
    /// <param name="request">付款请求体，见 <see cref="V2PayWorkWxPocketRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>付款结果，见 <see cref="V2PayWorkWxPocketResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/mmpaymkttransfers/promotion/paywwsptrans2pocket</c>；
    /// XML + <b>双向证书</b> + <c>sign</c> + <c>workwx_sign</c>。</para>
    /// <para><b>接入资格</b>：官方本页原文标注「说明：「向员工付款」功能暂不支持新增，
    /// 已开通商户的功能使用不受影响。」⇒ 未开通的历史商户调用会被拒，
    /// 接入前须与微信侧确认资格。</para>
    /// <para><b>双签白名单</b>：<c>workwx_sign</c> 只覆盖官方列出的字段集
    /// （<c>amount</c>/<c>appid</c>/<c>desc</c>/<c>mch_id</c>/<c>nonce_str</c>/<c>openid</c>/
    /// <c>partner_trade_no</c>/<c>ww_msg_type</c>），且<b>参与</b> <c>sign</c> 计算 ⇒
    /// 顺序恒为「先 <c>workwx_sign</c> 后 <c>sign</c>」，口径见 <c>WechatPayV2WorkWxSigner.PayPocketWhitelist</c>。</para>
    /// <para><b>业务限制</b>：单笔单日 <b>200 元</b>、商户单日 <b>10 万元</b>、每用户每日 <b>10 次</b>、
    /// 同一用户两次领取间隔 <b>≥ 15 秒</b>。</para>
    /// <para><b>应答无签名</b>：官方本页应答<b>不带任何签名字段</b> ⇒ 证书通道自然跳过验签，
    /// 属官方契约而非 SDK 漏验。</para>
    /// </remarks>
    Task<V2PayWorkWxPocketResponse> PayWorkWxPocketAsync(
        V2PayWorkWxPocketRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询向企业微信成员的付款记录（APIv2，需商户证书）。
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90279"/>。
    /// </summary>
    /// <param name="request">查询请求体，见 <see cref="V2QueryWorkWxPocketRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>付款记录，见 <see cref="V2QueryWorkWxPocketResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/mmpaymkttransfers/promotion/querywwsptrans2pocket</c>；
    /// XML + <b>双向证书</b>；<b>不需</b> <c>workwx_sign</c>。</para>
    /// <para><b>入参成对</b>：查询须同时给 <c>partner_trade_no</c> 与 <c>openid</c>（官方要求），
    /// 与零钱查询族「单号即可」不同。</para>
    /// </remarks>
    Task<V2QueryWorkWxPocketResponse> QueryWorkWxPocketAsync(
        V2QueryWorkWxPocketRequest request, CancellationToken cancellationToken = default);
}
