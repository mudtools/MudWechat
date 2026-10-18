// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Transactions;

/// <summary>
/// 付款码支付请求体（微信支付 APIv3）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012382150"/>；
/// <b>POST /v3/pay/transactions/codepay</b>。</para>
/// <para><b>业务场景</b>：用户出示微信付款码（条码/二维码），商户扫描后调用本接口完成扣款。
/// 与 JSAPI/Native/H5/APP 下单不同，付款码支付<b>实时扣款</b>，成功即返回交易号。</para>
/// <para><b>必填</b>：<c>appid</c> / <c>mchid</c> / <c>description</c> / <c>out_trade_no</c> /
/// <c>total</c> / <c>auth_code</c>。其余选填一律可空。</para>
/// <para><b>金额单位</b>：<c>total</c> 单位为<b>分</b>，必须大于 0。</para>
/// <para><b>频率限制</b>：同一 <c>auth_code</c> 在短时间内的重复请求会被拦截，
/// 官方建议做好幂等控制（使用相同的 <c>out_trade_no</c>）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class CodepayRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）。须与 <c>mchid</c> 有绑定关系。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商品描述（<c>description</c>，必填 string(127)），用户微信账单的商品字段中可见。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 商户订单号（<c>out_trade_no</c>，必填 string(32)）：6-32 字符，
    /// 只能是数字、大小写字母 <c>_-|*</c>，同一商户号下唯一。
    /// </summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>
    /// 订单总金额（<c>total</c>，必填 int）：单位为<b>分</b>，必须大于 0。
    /// </summary>
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    /// <summary>
    /// 付款授权码（<c>auth_code</c>，必填 string(128)）：用户微信客户端「收付款」页面展示的条码/二维码内容，
    /// 由商户扫码设备读取后传入。以 <c>10</c>/<c>11</c>/<c>12</c>/<c>13</c>/<c>14</c>/<c>15</c> 开头的数字串。
    /// </summary>
    [JsonPropertyName("auth_code")]
    public string? AuthCode { get; set; }

    /// <summary>
    /// 子商户号（<c>sub_mchid</c>，选填 string(32)）：服务商模式下的子商户号。
    /// </summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>
    /// 终端 IP（<c>spbill_create_ip</c>，选填 string(45)）：用户的客户端 IP，支持 IPv4 和 IPv6。
    /// </summary>
    [JsonPropertyName("spbill_create_ip")]
    public string? SpbillCreateIp { get; set; }

    /// <summary>
    /// 附加数据（<c>attach</c>，选填 string(128)）：在查询 API 和支付通知中原样返回，可作为自定义参数使用。
    /// </summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>
    /// 优惠标记（<c>goods_tag</c>，选填 string(32)）：代金券或立减优惠功能的参数，说明详见微信支付代金券文档。
    /// </summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>
    /// 电子发票入口开放标识（<c>support_fapiao</c>，选填 bool）：
    /// 传入 <c>true</c> 时，支付成功消息和支付详情页将出现开票入口。
    /// </summary>
    [JsonPropertyName("support_fapiao")]
    public bool? SupportFapiao { get; set; }

    /// <summary>
    /// 订单优惠标记（<c>detail</c>，选填）：商品详情结构体，包含优惠信息。
    /// </summary>
    [JsonPropertyName("detail")]
    public CodepayDetail? Detail { get; set; }

    /// <summary>
    /// 场景信息（<c>scene_info</c>，选填）：支付场景描述信息。
    /// </summary>
    [JsonPropertyName("scene_info")]
    public CodepaySceneInfo? SceneInfo { get; set; }
}

/// <summary>
/// 付款码支付的商品详情结构体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class CodepayDetail
{
    /// <summary>
    /// 订单原价（<c>cost_price</c>，选填 int）：单位为分，可选字段。
    /// </summary>
    [JsonPropertyName("cost_price")]
    public int? CostPrice { get; set; }

    /// <summary>
    /// 小票ID（<c>invoice_id</c>，选填 string(32)）：商家小票的 ID。
    /// </summary>
    [JsonPropertyName("invoice_id")]
    public string? InvoiceId { get; set; }

    /// <summary>
    /// 商品列表（<c>goods_detail</c>，选填）：单品优惠信息。
    /// </summary>
    [JsonPropertyName("goods_detail")]
    public List<CodepayGoodsDetail>? GoodsDetail { get; set; }
}

/// <summary>
/// 付款码支付的商品单项信息。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class CodepayGoodsDetail
{
    /// <summary>商品编码（<c>goods_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("goods_id")]
    public string? GoodsId { get; set; }

    /// <summary>商品名称（<c>goods_name</c>，必填 string(256)）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>商品数量（<c>quantity</c>，必填 int）。</summary>
    [JsonPropertyName("quantity")]
    public int? Quantity { get; set; }

    /// <summary>商品单价（<c>unit_price</c>，必填 int）：单位为分。</summary>
    [JsonPropertyName("unit_price")]
    public int? UnitPrice { get; set; }
}

/// <summary>
/// 付款码支付的场景信息结构体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class CodepaySceneInfo
{
    /// <summary>
    /// 商户端设备号（<c>device_id</c>，选填 string(32)）：门店终端设备编号。
    /// </summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    /// <summary>
    /// 商户门店编号（<c>store_id</c>，选填 string(32)）：商户侧门店编号。
    /// </summary>
    [JsonPropertyName("store_id")]
    public string? StoreId { get; set; }

    /// <summary>
    /// 商户收银台编号（<c>counter_id</c>，选填 string(32)）：商户侧收银台编号。
    /// </summary>
    [JsonPropertyName("counter_id")]
    public string? CounterId { get; set; }
}
