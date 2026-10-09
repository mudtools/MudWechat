// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Combine;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「合单支付」域 SDK（APIv3 合单，<b>首批 2 端点</b>：JSAPI/小程序合单下单 + 合单关闭订单）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（<b>合作伙伴</b>文档中心，2026-10-09 核验）：
/// JSAPI/小程序合单下单 <see href="https://pay.weixin.qq.com/wiki/doc/apiv3_partner/apis/chapter5_1_4.shtml"/>
/// （更新时间 2025.01.16）、合单关闭订单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012709095"/>
/// （更新时间 2024.10.24）。
/// </para>
/// <para>
/// <b>⚠️ 本域仅服务商可用</b>：官方两页均标注<b>支持商户：【普通服务商】</b> ——
/// 合单支付是服务商能力（多商户商品单合并支付，1–50 笔），<b>普通商户不可用</b>。
/// 本仓商户基座已支持服务商（<c>{sp_mchid}:{sub_mchid}</c> 分槽），故可正常使用。
/// </para>
/// <para>
/// <b>本域尚未覆盖的端点</b>（官方「合单支付」产品页族中确有，本批未实现）：
/// 合单查询订单（按商户合单订单号 / 按微信支付订单号）、Native 合单下单
/// （<c>POST /v3/combine-transactions/native</c>，已核路由）、APP/H5 合单下单、调起支付（客户端 SDK，非 HTTP）、
/// 合单退款、合单支付/退款通知等。<b>不要凭推断补路由</b>。
/// </para>
/// <para>
/// <b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与
/// <see cref="IWechatPayTransactionsService"/> 一致，详见其 remarks。
/// </para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "CombineTransactions", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayCombineService
{
    /// <summary>
    /// JSAPI / 小程序合单下单。官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3_partner/apis/chapter5_1_4.shtml"/>。
    /// </summary>
    /// <param name="request">合单下单请求体，见 <see cref="CombinePrepayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>预支付交易会话标识（2 小时有效），见 <see cref="CombinePrepayResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/combine-transactions/jsapi</c>；无 path / query 参数。</para>
    /// <para><b>子单约束</b>：<c>sub_orders</c> <b>1–50 笔</b>；<c>sub_appid</c> <b>仅允许一笔</b>商品单填写。</para>
    /// <para><b>支付者约束</b>：<c>combine_payer_info</c> 的 <c>openid</c> 与 <c>sub_openid</c> <b>二选一必填</b>，
    /// 且传 <c>sub_openid</c> 时 <c>sub_appid</c> 必填。</para>
    /// </remarks>
    [Post("/v3/combine-transactions/jsapi")]
    Task<CombinePrepayResponse> CreateJsapiOrderAsync(
        [Body] CombinePrepayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 合单关闭订单。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012709095"/>。
    /// </summary>
    /// <param name="combineOutTradeNo">合单商户订单号（官方 path <c>combine_out_trade_no</c>，必填 string(32)）。</param>
    /// <param name="request">关单请求体（<c>combine_appid</c> + <c>sub_orders</c>），见 <see cref="CombineCloseOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/combine-transactions/out-trade-no/{combine_out_trade_no}/close</c>；
    /// path 必带 <c>combine_out_trade_no</c>。</para>
    /// <para>
    /// <b>三条硬约束（官方原文）</b>：① 合单支付订单<b>只能</b>用本接口关单；
    /// ② <b>不支持关闭部分子单</b>；③ 主单商户号 / 单号 / 子单个数 / 子单商户号 / 子单单号
    /// <b>必须与下单时完全一致</b>。
    /// </para>
    /// <para>
    /// <b>返回类型为何是 <c>Task</c></b>：官方明确「无应答包体」，成功状态码 <b>204 No Content</b>
    /// （与交易域关单、支付分解除授权同款处置）。
    /// </para>
    /// </remarks>
    [Post("/v3/combine-transactions/out-trade-no/{combineOutTradeNo}/close")]
    Task CloseOrderAsync(
        [Path] string combineOutTradeNo,
        [Body] CombineCloseOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 合单查询订单。官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3_partner/apis/chapter7_3_11.shtml"/>。
    /// </summary>
    /// <param name="combineOutTradeNo">合单商户订单号（官方 path <c>combine_out_trade_no</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>合单订单（含各商品单的交易状态与实付金额），见 <see cref="CombineQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/combine-transactions/out-trade-no/{combine_out_trade_no}</c>；
    /// <b>无 query 参数</b>。</para>
    /// <para>
    /// <b>查询方式只有一种</b>：官方本页仅提供按<b>合单商户订单号</b>查询，
    /// <b>没有</b>「按微信支付订单号」的等价入口（与单笔交易域的两种入口不同）—— 勿照搬交易域的直觉。
    /// </para>
    /// <para>
    /// <b>状态判定</b>：<c>sub_orders[].trade_state</c>（必填）是<b>每个商品单各自</b>的交易状态，
    /// 须逐单显式判定；<c>amount.payer_amount</c> 才是实付金额（<c>total_amount</c> 是标价）。
    /// </para>
    /// </remarks>
    [Get("/v3/combine-transactions/out-trade-no/{combineOutTradeNo}")]
    Task<CombineQueryResponse> QueryOrderAsync(
        [Path] string combineOutTradeNo,
        CancellationToken cancellationToken = default);
}
