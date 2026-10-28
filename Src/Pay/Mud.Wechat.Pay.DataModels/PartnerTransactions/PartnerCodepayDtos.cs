// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.PartnerTransactions;

/// <summary>
/// 服务商付款码支付（被扫）下单请求（官方 <c>POST /v3/pay/partner/transactions/codepay</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方字段面（2026-10-25 .md 直抓逐项核对，页 ID 4012382179）</b>：商户标识四分；
/// <c>payer.auth_code</c> 必填（用户付款码）；<c>amount</c> 必填（<c>total</c> 单位为分）；
/// <c>scene_info</c> 必填且 <c>store_info</c> 必填（<c>id</c> 与 <c>out_id</c> 二选一，与扫码支付族同表 ⇒
/// 共用 <see cref="PartnerScannedPosStoreInfo"/>）；<c>support_fapiao</c> 在<b>请求顶层</b>（同扫码支付族）；
/// <c>detail.goods_detail</c> 的单品项与既有 <see cref="PartnerGoodsDetailItem"/> 同表 ⇒ 共用。</para>
/// <para><b>与直连 <c>CodepayRequest</c> 的关系</b>：直连域该 DTO 是扁平旧形态（<c>auth_code</c>/<c>total</c>
/// 顶层平铺），与现行官方「对象分节」字段表不一致——两套 DTO <b>不共用</b>（跨域共用会把直连域的
/// 序列化上下文登记牵进本域）；直连域的形态纠偏另批处置，不在本域顺手改。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerCodepayRequest
{
    /// <summary>获取或设置合作伙伴应用 AppID（<c>sp_appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppId { get; set; }

    /// <summary>获取或设置合作伙伴商户号（<c>sp_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>获取或设置特约商户应用 AppID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>获取或设置特约商户商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>获取或设置商品描述（<c>description</c>，必填 string(127)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置商户订单号（<c>out_trade_no</c>，必填 string(32)；同一商户号下唯一）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>获取或设置附加数据（<c>attach</c>，选填 string(128)；查询与回调原样返回）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>获取或设置订单优惠标记（<c>goods_tag</c>，选填 string(32)）。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>获取或设置电子发票入口开放标识（<c>support_fapiao</c>，选填 boolean；<b>本族在请求顶层</b>）。</summary>
    [JsonPropertyName("support_fapiao")]
    public bool? SupportFapiao { get; set; }

    /// <summary>获取或设置支付者（<c>payer</c>，必填；<c>auth_code</c> 即用户付款码）。</summary>
    [JsonPropertyName("payer")]
    public PartnerCodepayPayer? Payer { get; set; }

    /// <summary>获取或设置订单金额（<c>amount</c>，必填；<c>total</c> 单位为分）。</summary>
    [JsonPropertyName("amount")]
    public PartnerCodepayAmount? Amount { get; set; }

    /// <summary>获取或设置场景信息（<c>scene_info</c>，必填；线下条码支付合规要求携带）。</summary>
    [JsonPropertyName("scene_info")]
    public PartnerCodepaySceneInfo? SceneInfo { get; set; }

    /// <summary>获取或设置优惠功能（<c>detail</c>，选填）。</summary>
    [JsonPropertyName("detail")]
    public PartnerCodepayDetail? Detail { get; set; }

    /// <summary>获取或设置结算信息（<c>settle_info</c>，选填；官方本族仅 <c>profit_sharing</c>）。</summary>
    [JsonPropertyName("settle_info")]
    public PartnerScannedPosSettleInfo? SettleInfo { get; set; }
}

/// <summary>服务商付款码支付的支付者节（官方 <c>payer</c>；仅 <c>auth_code</c> 一键）。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerCodepayPayer
{
    /// <summary>获取或设置授权码（<c>auth_code</c>，必填 string(32)：用户打开微信钱包显示的付款码）。</summary>
    [JsonPropertyName("auth_code")]
    public string? AuthCode { get; set; }
}

/// <summary>服务商付款码支付的订单金额节（官方 <c>amount</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerCodepayAmount
{
    /// <summary>获取或设置总金额（<c>total</c>，必填 integer；单位为分，只能为整数）。</summary>
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    /// <summary>获取或设置货币类型（<c>currency</c>，选填 string(16)；ISO 4217，目前只支持 CNY）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}

/// <summary>
/// 服务商付款码支付的场景信息节（官方 <c>scene_info</c>）。
/// </summary>
/// <remarks>与扫码支付族的 <see cref="PartnerScannedPosSceneInfo"/> 不同表：本族是
/// <c>device_id</c>/<c>device_ip</c>（无 <c>payer_client_ip</c>），门店节同表 ⇒ 共用 <see cref="PartnerScannedPosStoreInfo"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerCodepaySceneInfo
{
    /// <summary>获取或设置商户端设备号（<c>device_id</c>，选填 string(32)；门店号或收银设备 ID）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    /// <summary>获取或设置商户端设备 IP（<c>device_ip</c>，选填 string(45)）。</summary>
    [JsonPropertyName("device_ip")]
    public string? DeviceIp { get; set; }

    /// <summary>获取或设置商户门店信息（<c>store_info</c>，必填；<c>id</c> 与 <c>out_id</c> 二选一必填）。</summary>
    [JsonPropertyName("store_info")]
    public PartnerScannedPosStoreInfo? StoreInfo { get; set; }
}

/// <summary>服务商付款码支付的优惠功能节（官方 <c>detail</c>；单品项与下单族同表 ⇒ 共用 <see cref="PartnerGoodsDetailItem"/>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerCodepayDetail
{
    /// <summary>获取或设置订单原价（<c>cost_price</c>，选填 integer；与支付金额不等则不享受优惠）。</summary>
    [JsonPropertyName("cost_price")]
    public int? CostPrice { get; set; }

    /// <summary>获取或设置商品小票 ID（<c>invoice_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("invoice_id")]
    public string? InvoiceId { get; set; }

    /// <summary>获取或设置单品列表（<c>goods_detail</c>，官方必填数组；传优惠功能时携带）。</summary>
    [JsonPropertyName("goods_detail")]
    public IReadOnlyList<PartnerGoodsDetailItem>? GoodsDetail { get; set; }
}

/// <summary>服务商撤销订单请求（官方 <c>POST /v3/pay/partner/transactions/out-trade-no/{out_trade_no}/reverse</c> 请求体）。</summary>
/// <remarks>官方请求体为四分商户标识（<c>sp_mchid</c>/<c>sub_mchid</c> 必填，两 AppID 选填）——与关单体
/// （<see cref="PartnerCloseOrderRequest"/>，无 AppID）不同表 ⇒ 分建。</remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerReverseOrderRequest
{
    /// <summary>获取或设置合作伙伴应用 AppID（<c>sp_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppId { get; set; }

    /// <summary>获取或设置合作伙伴商户号（<c>sp_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>获取或设置特约商户应用 AppID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>获取或设置特约商户商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }
}

/// <summary>服务商撤销订单应答（官方 <c>reverse</c> 应答：四分商户标识 + 订单号，无交易状态节）。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerReverseOrderResponse : WechatPayResponse
{
    /// <summary>获取或设置合作伙伴应用 AppID（<c>sp_appid</c>，选填）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppId { get; set; }

    /// <summary>获取或设置合作伙伴商户号（<c>sp_mchid</c>，必填）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>获取或设置特约商户应用 AppID（<c>sub_appid</c>，选填）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>获取或设置特约商户商户号（<c>sub_mchid</c>，必填）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>获取或设置商户订单号（<c>out_trade_no</c>，必填）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }
}
