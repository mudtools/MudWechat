// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PartnerTransactions;

/// <summary>
/// 服务商扫码支付（被扫 / <c>scannedpos</c>）下单请求体（微信支付 APIv3，
/// <b>POST /v3/pay/partner/transactions/scannedpos</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012492476"/>，2026-10-14 逐字段核验。</para>
/// <para><b>本族是五族里字段约束最特殊的一个</b>（守卫 PT-B4 逐条锁定，勿按 JSAPI/Native 的直觉改）：
/// ① <c>sp_appid</c> 与 <c>sub_appid</c> <b>二选一必填</b>（其余四族 <c>sp_appid</c> 恒必填）；
/// ② <c>notify_url</c> 是<b>选填</b>（其余四族必填）；
/// ③ <c>support_fapiao</c> 在<b>请求顶层</b>（其余四族在 <c>settle_info</c> 内，形态同直连域）；
/// ④ <c>settle_info</c> 官方只有 <c>profit_sharing</c> 一个字段 ⇒ 独立类型 <see cref="PartnerScannedPosSettleInfo"/>；
/// ⑤ <c>scene_info</c> <b>整体必填</b>、<c>payer_client_ip</c> 反而<b>选填</b>、
/// <c>store_info</c> <b>必填</b>且只有 <c>id</c>/<c>out_id</c> ⇒ 独立类型 <see cref="PartnerScannedPosSceneInfo"/>。</para>
/// <para><b>应答是 <c>prepay_id</c></b>（<see cref="PartnerPrepayIdResponse"/>）：与 JSAPI / APP 表相同 ⇒ 共用类型。
/// 扫码支付为<b>商户扫用户付款码</b>的「被扫」形态，<c>payer</c> 必填且 <c>sp_openid</c>/<c>sub_openid</c> 二选一。</para>
/// <para><b>本域另有 APIv2 的付款码支付与撤销接口</b>（SKIT 清单里的 <c>/pay/partner/transactions/codepay</c>
/// 与 <c>…/reverse</c>）：官方现行正文已是「付款码支付（V2）」指引页、<b>无 APIv3 字段表</b> ⇒
/// 本族<b>只落 scannedpos 一个端点</b>，V2 两支归 P0-8，守卫 PT-B2 以「零声明」锁定。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerScannedPosPrepayRequest
{
    /// <summary>合作伙伴应用 APPID（<c>sp_appid</c>，选填 string(32)）：与 <c>sub_appid</c> <b>二选一必填</b>。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppId { get; set; }

    /// <summary>合作伙伴商户号（<c>sp_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>特约商户应用 APPID（<c>sub_appid</c>，选填 string(32)）：与 <c>sp_appid</c> <b>二选一必填</b>。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>特约商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商品描述（<c>description</c>，必填 string(127)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，必填 string(32)），同一商户号下唯一。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>交易结束时间（<c>time_expire</c>，选填 string(64)，rfc3339）。</summary>
    [JsonPropertyName("time_expire")]
    public string? TimeExpire { get; set; }

    /// <summary>附加数据（<c>attach</c>，选填 string(128)），查询与通知中原样返回。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>通知地址（<c>notify_url</c>，<b>选填</b> string(255)）：本族是五族中唯一不强制回调地址的下单族。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>订单优惠标记（<c>goods_tag</c>，选填 string(32)）。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>
    /// 电子发票入口开放标识（<c>support_fapiao</c>，选填 boolean）：本族位于<b>请求顶层</b>，
    /// 与其余四族的 <c>settle_info.support_fapiao</c> 位置不同（守卫 PT-B4）。
    /// </summary>
    [JsonPropertyName("support_fapiao")]
    public bool? SupportFapiao { get; set; }

    /// <summary>订单金额（<c>amount</c>，必填），见 <see cref="PartnerAmountInfo"/>。</summary>
    [JsonPropertyName("amount")]
    public PartnerAmountInfo? Amount { get; set; }

    /// <summary>支付者信息（<c>payer</c>，必填）：<c>sp_openid</c> 与 <c>sub_openid</c> 二选一。</summary>
    [JsonPropertyName("payer")]
    public PartnerPayerInfo? Payer { get; set; }

    /// <summary>商品详情（<c>detail</c>，选填），见 <see cref="PartnerGoodsDetail"/>。</summary>
    [JsonPropertyName("detail")]
    public PartnerGoodsDetail? Detail { get; set; }

    /// <summary>场景信息（<c>scene_info</c>，<b>必填</b>）：本族专用形态，须上报门店信息，见 <see cref="PartnerScannedPosSceneInfo"/>。</summary>
    [JsonPropertyName("scene_info")]
    public PartnerScannedPosSceneInfo? SceneInfo { get; set; }

    /// <summary>结算信息（<c>settle_info</c>，选填）：本族官方只有 <c>profit_sharing</c>，见 <see cref="PartnerScannedPosSettleInfo"/>。</summary>
    [JsonPropertyName("settle_info")]
    public PartnerScannedPosSettleInfo? SettleInfo { get; set; }
}
