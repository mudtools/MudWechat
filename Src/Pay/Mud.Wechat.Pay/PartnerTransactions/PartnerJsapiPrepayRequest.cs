// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PartnerTransactions;

/// <summary>
/// 服务商 JSAPI / 小程序下单请求体（微信支付 APIv3，<b>POST /v3/pay/partner/transactions/jsapi</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012714911"/>（JSAPI/小程序下单），
/// 2026-10-14 逐字段核验。</para>
/// <para><b>与直连域 <c>JsapiPrepayRequest</c> 的三处结构差异</b>（勿复制粘贴混用）：
/// ① 商户标识四分 —— <c>sp_appid</c>/<c>sp_mchid</c> 必填、<c>sub_appid</c> 选填、<c>sub_mchid</c> 必填，
/// 直连的 <c>appid</c>/<c>mchid</c> 在本族<b>不存在</b>；
/// ② <c>payer</c> 装的是 <see cref="PartnerPayerInfo"/>（<c>sp_openid</c> / <c>sub_openid</c> <b>二选一</b>），
/// 不是直连的单 <c>openid</c>；
/// ③ <c>support_fapiao</c> 位于 <c>settle_info</c> 内（<see cref="PartnerSettleInfo"/>），
/// 而直连域把它放在请求顶层 —— 该位置差异由守卫 PT-B4 锁定。</para>
/// <para><b>频率</b>：官方对本接口有商户号级频率上限，超限返回 <c>FREQUENCY_LIMITED</c>(429)，须降低请求频率。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerJsapiPrepayRequest
{
    /// <summary>服务商 APPID（<c>sp_appid</c>，必填 string(32)）。须与 <c>sp_mchid</c> 有绑定关系。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppId { get; set; }

    /// <summary>服务商户号（<c>sp_mchid</c>，必填 string(32)），由微信支付生成并下发。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>子商户 APPID（<c>sub_appid</c>，选填 string(32)）。须与 <c>sub_mchid</c> 有绑定关系；上送 <c>sub_openid</c> 时必填。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)），由微信支付生成并下发。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商品描述（<c>description</c>，必填 string(127)），用户账单中可见。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 商户订单号（<c>out_trade_no</c>，必填 string(32)）：只能是数字、大小写字母与 <c>_-*</c>，
    /// 同一商户号下唯一；重复提交官方返回 <c>OUT_TRADE_NO_USED</c>(403)。
    /// </summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>
    /// 支付结束时间（<c>time_expire</c>，选填 string(64)，rfc3339）：须在下单时间 15 天内、且不得早于下单后 1 分钟；
    /// 不传则默认下单后 7 天失效。保持字符串而非 <see cref="DateTimeOffset"/> —— 偏移量往返格式不可控，一字节偏差即 <c>PARAM_ERROR</c>。
    /// </summary>
    [JsonPropertyName("time_expire")]
    public string? TimeExpire { get; set; }

    /// <summary>附加数据（<c>attach</c>，选填 string(128)），查询与支付通知中原样返回，仅支付完成状态返回。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>通知地址（<c>notify_url</c>，必填 string(255)）：外网可访问、不携带参数；公网域名须 HTTPS（专线可用 HTTP）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>订单优惠标记（<c>goods_tag</c>，选填 string(32)），代金券/立减优惠按标记匹配。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>结算信息（<c>settle_info</c>，选填），见 <see cref="PartnerSettleInfo"/>；<c>support_fapiao</c> 在本族位于<b>本节内</b>。</summary>
    [JsonPropertyName("settle_info")]
    public PartnerSettleInfo? SettleInfo { get; set; }

    /// <summary>订单金额（<c>amount</c>，必填），见 <see cref="PartnerAmountInfo"/>。</summary>
    [JsonPropertyName("amount")]
    public PartnerAmountInfo? Amount { get; set; }

    /// <summary>支付者信息（<c>payer</c>，必填）：官方原文要求 <c>sp_openid</c> 与 <c>sub_openid</c> <b>必须要填一个</b>。</summary>
    [JsonPropertyName("payer")]
    public PartnerPayerInfo? Payer { get; set; }

    /// <summary>商品详情（<c>detail</c>，选填），见 <see cref="PartnerGoodsDetail"/>；压缩后总长不超过 6144 字节。</summary>
    [JsonPropertyName("detail")]
    public PartnerGoodsDetail? Detail { get; set; }

    /// <summary>场景信息（<c>scene_info</c>，选填），见 <see cref="PartnerSceneInfo"/>；一旦上送则 <c>payer_client_ip</c> 必填。</summary>
    [JsonPropertyName("scene_info")]
    public PartnerSceneInfo? SceneInfo { get; set; }
}
