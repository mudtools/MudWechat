// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PartnerTransactions;

/// <summary>
/// 服务商 Native（扫码支付）下单请求体（微信支付 APIv3，<b>POST /v3/pay/partner/transactions/native</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012714902"/>
/// （同内容产品族分身页 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012738659"/>），2026-10-14 逐字段核验。</para>
/// <para><b>与 <see cref="PartnerJsapiPrepayRequest"/> 的唯一结构差异</b>：<b>无 <c>payer</c></b> ——
/// Native 由用户<b>扫码</b>发起支付，下单时不存在支付者标识。其余字段（含 <c>settle_info.support_fapiao</c>
/// 的位置与 <c>scene_info</c> 形态）与 JSAPI 逐项一致，故共用
/// <see cref="PartnerSettleInfo"/> / <see cref="PartnerSceneInfo"/>。</para>
/// <para><b>应答是 <c>code_url</c> 而非 <c>prepay_id</c></b>，见 <see cref="PartnerNativePrepayResponse"/>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerNativePrepayRequest
{
    /// <summary>服务商 APPID（<c>sp_appid</c>，必填 string(32)）。须与 <c>sp_mchid</c> 有绑定关系。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppId { get; set; }

    /// <summary>服务商户号（<c>sp_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>子商户 APPID（<c>sub_appid</c>，选填 string(32)）。须与 <c>sub_mchid</c> 有绑定关系。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商品描述（<c>description</c>，必填 string(127)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，必填 string(32)），同一商户号下唯一。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>支付结束时间（<c>time_expire</c>，选填 string(64)，rfc3339）；不传则默认下单后 7 天失效。</summary>
    [JsonPropertyName("time_expire")]
    public string? TimeExpire { get; set; }

    /// <summary>附加数据（<c>attach</c>，选填 string(128)）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>通知地址（<c>notify_url</c>，必填 string(255)）：外网可访问、不携带参数、公网须 HTTPS。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>订单优惠标记（<c>goods_tag</c>，选填 string(32)）。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>结算信息（<c>settle_info</c>，选填），见 <see cref="PartnerSettleInfo"/>。</summary>
    [JsonPropertyName("settle_info")]
    public PartnerSettleInfo? SettleInfo { get; set; }

    /// <summary>订单金额（<c>amount</c>，必填），见 <see cref="PartnerAmountInfo"/>。</summary>
    [JsonPropertyName("amount")]
    public PartnerAmountInfo? Amount { get; set; }

    /// <summary>商品详情（<c>detail</c>，选填），见 <see cref="PartnerGoodsDetail"/>；压缩后总长不超过 6144 字节。</summary>
    [JsonPropertyName("detail")]
    public PartnerGoodsDetail? Detail { get; set; }

    /// <summary>场景信息（<c>scene_info</c>，选填），见 <see cref="PartnerSceneInfo"/>；一旦上送则 <c>payer_client_ip</c> 必填。</summary>
    [JsonPropertyName("scene_info")]
    public PartnerSceneInfo? SceneInfo { get; set; }
}
