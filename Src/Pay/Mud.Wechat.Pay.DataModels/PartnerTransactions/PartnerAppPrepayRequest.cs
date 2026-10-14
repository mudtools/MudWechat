// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PartnerTransactions;

/// <summary>
/// 服务商 APP 下单请求体（微信支付 APIv3，<b>POST /v3/pay/partner/transactions/app</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012714669"/>
/// （同内容产品族分身页 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013080231"/>），2026-10-14 逐字段核验。</para>
/// <para><b><c>sp_appid</c> / <c>sub_appid</c> 语义收窄</b>：本族两者须为<b>移动应用</b> AppID
/// （微信开放平台「移动应用」申请），且各自须与 <c>sp_mchid</c> / <c>sub_mchid</c> 有绑定关系 ——
/// 与 JSAPI（公众号/小程序 AppID）、Native（不限形态）不同，绑定关系错配官方返回 <c>APPID_MCHID_NOT_MATCH</c>。</para>
/// <para><b>字段集与 <see cref="PartnerNativePrepayRequest"/> 同形</b>（均无 <c>payer</c>、无 <c>subsidy_info</c>）：
/// 官方 APP 页字段表内<b>没有</b>补贴节，直连域 APP 下单的 <c>subsidy_info</c> 在本族<b>不存在</b>，
/// 不得据直连形态补字段。应答则与 JSAPI 同为 <c>prepay_id</c>（<see cref="PartnerPrepayIdResponse"/>）。</para>
/// <para><b>调起支付在客户端侧</b>：<c>prepay_id</c> 回传 APP 后由移动 SDK 二次签名调起，
/// <b>服务端不得</b>替客户端组装该签名串。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerAppPrepayRequest
{
    /// <summary>服务商 APPID（<c>sp_appid</c>，必填 string(32)），须为<b>移动应用</b> AppID 且与 <c>sp_mchid</c> 绑定。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppId { get; set; }

    /// <summary>服务商户号（<c>sp_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>子商户 APPID（<c>sub_appid</c>，选填 string(32)），须为<b>移动应用</b> AppID 且与 <c>sub_mchid</c> 绑定。</summary>
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

    /// <summary>支付结束时间（<c>time_expire</c>，选填 string(64)，rfc3339）；须在下单时间 15 天内，不传默认 7 天失效。</summary>
    [JsonPropertyName("time_expire")]
    public string? TimeExpire { get; set; }

    /// <summary>附加数据（<c>attach</c>，选填 string(128)）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>通知地址（<c>notify_url</c>，必填 string(255)）：外网可访问、不携带参数、公网须 HTTPS（专线可用 HTTP）。</summary>
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
