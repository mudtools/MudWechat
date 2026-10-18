// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>支付押金（预授权）</b>域中<b>不需商户证书</b>的 5 支端点
/// （JSAPI 下单 / 人脸支付 / 付款码支付 / 查单 / 查退款）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（APIv2 合作伙伴侧，2026-10-18 逐页核验）：
/// JSAPI 下单 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011986340"/>、
/// 人脸支付 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011986577"/>、
/// 付款码支付 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011986623"/>、
/// 查询订单 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011986679"/>、
/// 查询退款 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011987300"/>。</para>
/// <para><b>本域按证书要求一分为二</b>：撤销 / 消费押金 / 申请退款这 3 支官方标注
/// 「是否需要证书：是」⇒ 落在 <see cref="IWechatPayV2DepositSecureService"/>；
/// 本接口只承载余下 5 支（官方一律标注「不需要」）⇒ 走声明式生成，挂 APIv2 命名客户端。</para>
/// <para><b>域名</b>：押金族 8 支官方<b>全部</b>给 <c>api.mch.weixin.qq.com</c>，
/// 与 <c>/secapi/pay/*</c> 那族的 <c>api2.mch</c> 不同 ⇒ 证书支也走 <c>api.mch</c>。</para>
/// <para><b>服务商形态</b>：本族是服务商接口，<c>sub_mch_id</c> 必填、
/// <c>openid</c>/<c>sub_openid</c> 二选一（传 <c>sub_openid</c> 必须同时传 <c>sub_appid</c>）。</para>
/// <para><b>签名算法不统一</b>：8 支的 <c>sign_type</c> 都必填，但只有 JSAPI 下单这页写明
/// 支持 MD5 与 HMAC-SHA256，其余各页均标 <b>仅 HMAC-SHA256</b> ⇒
/// 除下单外的各支请显式传 <c>HMAC-SHA256</c>，留空按 MD5 签会 <c>SIGNERROR</c>。</para>
/// <para><b>业务限制</b>：官方在下单页标注「不支持代金券功能；押金超时时间 30 个自然日」，
/// 人脸/付款码两页标注「当押金超过一个月未进行处理，会自动执行退款」。
/// 人脸与付款码两支<b>处于灰度测试阶段、暂未开放</b>（官方页面原话），需单独申请。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "DepositV2", HttpClient = WechatPayV2HttpClientNames.TypeName)]
public interface IWechatPayV2DepositService
{
    /// <summary>
    /// 支付押金（JSAPI 下单，服务商侧）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011986340"/>。
    /// </summary>
    /// <param name="request">下单请求体，见 <see cref="V2DepositUnifiedOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>预支付结果（<c>prepay_id</c>），见 <see cref="V2DepositUnifiedOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/deposit/unifiedorder</c>；XML 报文；<c>trade_type</c> 只有 <c>JSAPI</c>。</para>
    /// <para><b><c>prepay_id</c> 有效期 2 小时</b>；本页是押金族唯一允许 <c>sign_type = MD5</c> 的一支。</para>
    /// <para><b>无 <c>deposit</c> 字段</b>：「是否押金支付」只出现在人脸/付款码两支，下单页靠接口路径本身区分。</para>
    /// </remarks>
    [Post("/deposit/unifiedorder")]
    Task<V2DepositUnifiedOrderResponse> UnifiedOrderAsync(
        V2DepositUnifiedOrderRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 支付押金（人脸支付）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011986577"/>。
    /// </summary>
    /// <param name="request">人脸支付请求体，见 <see cref="V2DepositFacePayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>同步支付结果，见 <see cref="V2DepositFacePayResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/deposit/facepay</c>；XML 报文；<c>sign_type</c> <b>仅 HMAC-SHA256</b>。</para>
    /// <para><b>同步返回</b>：本支<b>没有</b> <c>notify_url</c>，支付结果直接落在应答里；
    /// <c>err_code = USERPAYING</c> 表示用户支付中，官方要求<b>等待 5 秒后调 <c>/deposit/orderquery</c> 查单</b>。</para>
    /// <para><b><c>deposit</c> 是 String(1)</b> 的 <c>Y</c>/<c>N</c>（不是布尔），押金场景传 <c>Y</c>。</para>
    /// <para><b>灰度未开放</b>：官方页面标注「本功能目前处于灰度测试阶段、暂未开放使用」。</para>
    /// </remarks>
    [Post("/deposit/facepay")]
    Task<V2DepositFacePayResponse> FacePayAsync(
        V2DepositFacePayRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 支付押金（付款码支付）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011986623"/>。
    /// </summary>
    /// <param name="request">付款码支付请求体，见 <see cref="V2DepositMicropayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>同步支付结果，见 <see cref="V2DepositMicropayResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/deposit/micropay</c>；XML 报文；<c>sign_type</c> <b>仅 HMAC-SHA256</b>。
    /// 该页 H1 是「支付押金（付款码支付）」，<b>不是</b>统一下单 —— 统一下单在 <c>/deposit/unifiedorder</c>。</para>
    /// <para><b><c>auth_code</c> 形状</b>：官方规则为 18 位纯数字、以 10/11/12/13/14/15 开头。</para>
    /// <para><b><c>USERPAYING</c> 与人脸支不同</b>：本支官方要求<b>等待 5 秒后查单</b>再判定，
    /// 不得直接判失败（症状是重复扣款）。</para>
    /// <para><b>灰度未开放</b>：同样标注「目前处于灰度测试阶段、暂未开放使用」。</para>
    /// </remarks>
    [Post("/deposit/micropay")]
    Task<V2DepositMicropayResponse> MicropayAsync(
        V2DepositMicropayRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询押金订单（含押金消费状态）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011986679"/>。
    /// </summary>
    /// <param name="request">查单请求体，见 <see cref="V2DepositOrderQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>订单与押金消费状态，见 <see cref="V2DepositOrderQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/deposit/orderquery</c>；XML 报文；<c>sign_type</c> 仅 HMAC-SHA256；
    /// <c>transaction_id</c> 与 <c>out_trade_no</c> 二选一，同存时前者优先。</para>
    /// <para><b>三级门控</b>：本页成功块的门控是 <c>return_code</c> + <c>result_code</c> + <b><c>trade_state</c></b>
    /// 三者都为 SUCCESS，<c>trade_state</c> 非 SUCCESS 时只回 <c>out_trade_no</c>（必传）与 <c>attach</c>（选传）。</para>
    /// <para><b>押金态 <c>trade_state</c></b>：除常规值外另有 <c>SETTLING</c>（消费已受理）、
    /// <c>SETTLEMENTFAIL</c>（解除冻结失败）、<c>CONSUMED</c>（消费成功）三值；
    /// <c>trade_type</c> 取值 <c>MICROPAY</c> / <c>DEPOSITPAY</c>。</para>
    /// </remarks>
    [Post("/deposit/orderquery")]
    Task<V2DepositOrderQueryResponse> OrderQueryAsync(
        V2DepositOrderQueryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询押金退款。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011987300"/>。
    /// </summary>
    /// <param name="request">查退款请求体，见 <see cref="V2DepositRefundQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>退款单状态，见 <see cref="V2DepositRefundQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/deposit/refundquery</c>；XML 报文；
    /// 只按 <c>refund_id</c> / <c>out_refund_no</c> 二选一（<b>请求面没有</b> <c>transaction_id</c> 与 <c>out_trade_no</c>）。</para>
    /// <para><b>非下标形态</b>：与基础支付的 <c>/pay/refundquery</c> 不同，本页是<b>单退款扁平结构</b>，
    /// 没有 <c>refund_count</c> 与 <c>_$n</c> 后缀字段（官方示例 XML 里出现的 <c>_0</c> 后缀属残留，字段表并无）。</para>
    /// <para><b>字段名照抄官方</b>：到账账户字段是 <c>refund_recv_accout</c>（少一个 n）。</para>
    /// <para><b>到账延时</b>：零钱支付 20 分钟内到账，银行卡支付 3 个工作日后再查。</para>
    /// </remarks>
    [Post("/deposit/refundquery")]
    Task<V2DepositRefundQueryResponse> RefundQueryAsync(
        V2DepositRefundQueryRequest request, CancellationToken cancellationToken = default);
}
