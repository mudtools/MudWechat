// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PartnerTransactions;

/// <summary>
/// 服务商 H5（移动端网页）下单请求体（微信支付 APIv3，<b>POST /v3/pay/partner/transactions/h5</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012714759"/>
/// （同内容产品族分身页 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012738604"/>），2026-10-14 逐字段核验。</para>
/// <para><b>⚠️ 本族最容易漏的一处</b>：<c>scene_info</c> 在本族<b>整体必填</b>且类型是
/// <see cref="PartnerH5SceneInfo"/>（比 <see cref="PartnerSceneInfo"/> <b>多一节必填的 <c>h5_info</c></b>）——
/// 从 <see cref="PartnerNativePrepayRequest"/> 或 <see cref="PartnerJsapiPrepayRequest"/> 复制请求体
/// 会<b>传不了</b> <c>h5_info</c>，官方直接判参数错误。守卫 PT-B4 锁定「三套 scene_info 类型不共用」。</para>
/// <para><b>与 JSAPI / Native 的另两处差异</b>：① <b>无 <c>payer</c></b>（H5 由用户在浏览器内发起支付）；
/// ② 应答是 <c>h5_url</c>（<see cref="PartnerH5PrepayResponse"/>）而非 <c>prepay_id</c>。</para>
/// <para><b>仅在微信外浏览器场景可用</b>：微信内置浏览器场景须走 <see cref="PartnerJsapiPrepayRequest"/> 下单。
/// 拿到 <c>h5_url</c> 后须按官方《H5 调起支付》指引跳转（含 <c>Referer</c> 要求），<b>严禁</b>改写或拼装该链接。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerH5PrepayRequest
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

    /// <summary>支付结束时间（<c>time_expire</c>，选填 string(64)，rfc3339）；不传默认下单后 7 天失效。</summary>
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

    /// <summary>场景信息（<c>scene_info</c>，<b>必填</b>）：本族专用类型，含必填的 <c>h5_info</c>，见 <see cref="PartnerH5SceneInfo"/>。</summary>
    [JsonPropertyName("scene_info")]
    public PartnerH5SceneInfo? SceneInfo { get; set; }
}
