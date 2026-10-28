// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2「基础支付」域中<b>无需双向证书</b>的 8 支端点
/// （统一下单 / 付款码支付 / 查单 / 关单 / 退款查询 / 退款查询 v2 / 结算查询 / 汇率查询）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（商户平台 APIv2，2026-10-18 逐页核验）：
/// 统一下单 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935214"/>、
/// 付款码支付 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011937125"/>、
/// 订单查询 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935215"/>、
/// 关闭订单 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935216"/>、
/// 查询退款 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935218"/>。</para>
/// <para><b>与 APIv3 的根本差异</b>：APIv2 全程使用 <b>XML 报文</b> + <b>MD5/HMAC-SHA256 签名</b>
/// （<c>sign</c> 在<b>报文体内</b>而非请求头），与 APIv3 的 JSON + RSA-SHA256 <c>Authorization</c> 头完全独立。
/// 本接口<b>不声明 <c>[Token]</c></b>（签名在传输层由 <c>WechatPayV2SigningHandler</c> 完成），
/// 走支付专用客户端 <c>WechatPayV2HttpClientNames.ClientName</c>（基址 <c>api.mch.weixin.qq.com</c>）。</para>
/// <para><b>涉密端点不在本接口</b>：<c>/secapi/pay/refund</c>、<c>/secapi/pay/refundv2</c>、
/// <c>/secapi/pay/reverse</c> 与两份账单下载要求<b>商户 API 证书</b>（域名 <c>api2.mch.weixin.qq.com</c>），
/// 证书按商户绑定 ⇒ 与「一条命名客户端一个连接池」冲突，由 <see cref="IWechatPayV2SecureService"/>
/// 逐商户通道承载。拆分理由与形态差异见该接口注释。</para>
/// <para><b>应答语义</b>：APIv2 的业务失败也用 <b>HTTP 200</b> + <c>&lt;return_code&gt;FAIL&lt;/return_code&gt;</c>
/// 表达，故本接口标 <c>[AllowAnyStatusCode]</c>：判定一律看报文体里的
/// <c>return_code</c> / <c>result_code</c> 两级，不看状态码。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "BasePayV2", HttpClient = WechatPayV2HttpClientNames.TypeName)]
public interface IWechatPayV2Service
{
    /// <summary>
    /// 统一下单（APIv2）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935214"/>。
    /// </summary>
    /// <param name="request">下单请求体，见 <see cref="V2UnifiedOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>下单应答，含 <c>prepay_id</c>，见 <see cref="V2UnifiedOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/pay/unifiedorder</c>；XML 报文，MD5/HMAC-SHA256 签名。</para>
    /// <para><b>业务场景</b>：JSAPI / Native / APP / H5 / 小程序等多种支付方式的统一入口，
    /// 根据 <c>trade_type</c> 区分具体支付形态；<c>prepay_id</c> 有效期 <b>2 小时</b>。</para>
    /// </remarks>
    [Post("/pay/unifiedorder")]
    Task<V2UnifiedOrderResponse> UnifiedOrderAsync(
        V2UnifiedOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 付款码支付（APIv2）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011937125"/>。
    /// </summary>
    /// <param name="request">付款码支付请求体，见 <see cref="V2MicropayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>支付结果，含 <c>transaction_id</c> / <c>trade_state</c>，见 <see cref="V2MicropayResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/pay/micropay</c>；XML 报文，MD5/HMAC-SHA256 签名。</para>
    /// <para><b>业务特征</b>：用户出示微信付款码，商户扫描后<b>同步扣款</b>，成功即返回交易号。
    /// <c>trade_state = USERPAYING</c> 表示用户正在输入密码，须<b>间隔 5 秒</b>轮询订单查询，
    /// 且<b>不得</b>在此状态下换单重提。</para>
    /// </remarks>
    [Post("/pay/micropay")]
    Task<V2MicropayResponse> MicropayAsync(
        V2MicropayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询订单（APIv2）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935215"/>。
    /// </summary>
    /// <param name="request">查单请求体（<c>transaction_id</c> 或 <c>out_trade_no</c> 二选一），见 <see cref="V2OrderQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>订单详情，见 <see cref="V2OrderQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/pay/orderquery</c>；XML 报文，MD5/HMAC-SHA256 签名。</para>
    /// <para><b>查询键</b>：<c>transaction_id</c>（微信支付订单号）与 <c>out_trade_no</c>（商户订单号）二选一，
    /// 优先使用 <c>transaction_id</c>。</para>
    /// <para><b>频率上限</b>：官方对未支付订单的轮询建议<b>不超过 30 秒 / 次</b>；
    /// 定单超时 5 秒后会自动关闭，此后查询返回 <c>trade_state = CLOSED</c>。</para>
    /// </remarks>
    [Post("/pay/orderquery")]
    Task<V2OrderQueryResponse> OrderQueryAsync(
        V2OrderQueryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 关闭订单（APIv2）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935216"/>。
    /// </summary>
    /// <param name="request">关单请求体，见 <see cref="V2CloseOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>关单结果，见 <see cref="V2CloseOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/pay/closeorder</c>；XML 报文，MD5/HMAC-SHA256 签名。</para>
    /// <para><b>业务限制</b>：① 订单<b>已支付成功不能关闭</b>（须走退款）；② 关单后用户无法再支付该订单；
    /// ③ 已超时未支付的订单官方自动关闭。关单<b>异步生效</b>，以查询结果为准。</para>
    /// </remarks>
    [Post("/pay/closeorder")]
    Task<V2CloseOrderResponse> CloseOrderAsync(
        V2CloseOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询退款（APIv2）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935218"/>。
    /// </summary>
    /// <param name="request">退款查询请求体，见 <see cref="V2RefundQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>退款详情，见 <see cref="V2RefundQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/pay/refundquery</c>；XML 报文，MD5/HMAC-SHA256 签名。</para>
    /// <para><b>查询键</b>：四选一 —— <c>transaction_id</c> / <c>out_trade_no</c> / <c>out_refund_no</c> / <c>refund_id</c>。
    /// 按订单号查询会返回该单<b>全部</b>退款记录（<c>refund_count</c> + 下标后缀字段，最多 100 条）。</para>
    /// </remarks>
    [Post("/pay/refundquery")]
    Task<V2RefundQueryResponse> RefundQueryAsync(
        V2RefundQueryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询退款 V2（APIv2，服务商分账退款查询形态）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011984122"/>。
    /// </summary>
    /// <param name="request">退款查询请求体，见 <see cref="V2RefundQueryV2Request"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>退款详情，见 <see cref="V2RefundQueryV2Response"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/pay/refundqueryv2</c>；XML 报文，MD5/HMAC-SHA256 签名。
    /// 与 <c>/pay/refundquery</c> 相比多了分账回退明细，<b>仅服务商可用</b>。</para>
    /// </remarks>
    [Post("/pay/refundqueryv2")]
    Task<V2RefundQueryV2Response> RefundQueryV2Async(
        V2RefundQueryV2Request request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询结算状态（APIv2）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/global/v2/zh/4013634920"/>。
    /// </summary>
    /// <param name="request">结算查询请求体，见 <see cref="V2SettlementQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>结算状态，见 <see cref="V2SettlementQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/pay/settlementquery</c>；XML 报文，MD5/HMAC-SHA256 签名。</para>
    /// <para><b>业务场景</b>：查询商户号的结算状态，确认是否处于正常结算状态。</para>
    /// </remarks>
    [Post("/pay/settlementquery")]
    Task<V2SettlementQueryResponse> SettlementQueryAsync(
        V2SettlementQueryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询汇率（APIv2）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/global/v2/zh/4013634922"/>。
    /// </summary>
    /// <param name="request">汇率查询请求体，见 <see cref="V2ExchangeRateQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>汇率信息，见 <see cref="V2ExchangeRateQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/pay/queryexchagerate</c>；XML 报文，MD5/HMAC-SHA256 签名。
    /// 注意官方路由拼写即为 <c>exchagerate</c>（缺一个 t），照抄不得「修正」。</para>
    /// <para><b>业务场景</b>：跨境支付场景中，查询指定币种对的实时汇率。</para>
    /// </remarks>
    [Post("/pay/queryexchagerate")]
    Task<V2ExchangeRateQueryResponse> ExchangeRateQueryAsync(
        V2ExchangeRateQueryRequest request,
        CancellationToken cancellationToken = default);
}
