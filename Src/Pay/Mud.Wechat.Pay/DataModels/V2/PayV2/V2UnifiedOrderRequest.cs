// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 统一下单请求体。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935214"/>；
/// <b>POST /pay/unifiedorder</b>。</para>
/// <para><b>报文格式</b>：XML（非 JSON），MD5/HMAC-SHA256 签名。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2UnifiedOrderRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户号（<c>mch_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>子商户号（<c>sub_mch_id</c>，选填 string(32)）：服务商模式下的子商户号。</summary>
    [JsonPropertyName("sub_mch_id")]
    public string? SubMchId { get; set; }

    /// <summary>设备号（<c>device_info</c>，选填 string(32)）：终端设备编号。</summary>
    [JsonPropertyName("device_info")]
    public string? DeviceInfo { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>，必填 string(32)）：不长于 32 位的随机串。</summary>
    [JsonPropertyName("nonce_str")]
    public string? NonceStr { get; set; }

    /// <summary>签名类型（<c>sign_type</c>，选填 string(32)）：默认 <c>MD5</c>，支持 <c>HMAC-SHA256</c>。</summary>
    [JsonPropertyName("sign_type")]
    public string? SignType { get; set; }

    /// <summary>商品描述（<c>body</c>，必填 string(128)）。</summary>
    [JsonPropertyName("body")]
    public string? Body { get; set; }

    /// <summary>商品详情（<c>detail</c>，选填 string(6000)）：单品优惠信息，JSON 格式字符串。</summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    /// <summary>附加数据（<c>attach</c>，选填 string(127)）：在查询和通知中原样返回。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，必填 string(32)）：6-32 字符，同一商户号下唯一。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>标价币种（<c>fee_type</c>，选填 string(16)）：默认 <c>CNY</c>。</summary>
    [JsonPropertyName("fee_type")]
    public string? FeeType { get; set; }

    /// <summary>标价金额（<c>total_fee</c>，必填 int）：单位为<b>分</b>。</summary>
    [JsonPropertyName("total_fee")]
    public int? TotalFee { get; set; }

    /// <summary>终端 IP（<c>spbill_create_ip</c>，必填 string(45)）：用户的客户端 IP。</summary>
    [JsonPropertyName("spbill_create_ip")]
    public string? SpbillCreateIp { get; set; }

    /// <summary>交易起始时间（<c>time_start</c>，选填 string(14)）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("time_start")]
    public string? TimeStart { get; set; }

    /// <summary>交易结束时间（<c>time_expire</c>，选填 string(14)）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("time_expire")]
    public string? TimeExpire { get; set; }

    /// <summary>商品标记（<c>goods_tag</c>，选填 string(32)）：代金券或立减优惠参数。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>通知地址（<c>notify_url</c>，必填 string(256)）：接收支付结果通知的 URL。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>交易类型（<c>trade_type</c>，必填 string(16)）：
    /// <c>JSAPI</c> / <c>NATIVE</c> / <c>APP</c> / <c>MWEB</c>（H5）/ <c>MICROPAY</c>（付款码）。</summary>
    [JsonPropertyName("trade_type")]
    public string? TradeType { get; set; }

    /// <summary>指定支付方式（<c>limit_pay</c>，选填 string(32)）：<c>no_credit</c> 表示禁用信用卡。</summary>
    [JsonPropertyName("limit_pay")]
    public string? LimitPay { get; set; }

    /// <summary>用户标识（<c>openid</c>，选填 string(128)）：<c>trade_type=JSAPI</c> 时必填。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>是否关注公众账号（<c>is_subscribe</c>，选填 string(1)）：<c>Y</c> 或 <c>N</c>。</summary>
    [JsonPropertyName("is_subscribe")]
    public string? IsSubscribe { get; set; }

    /// <summary>场景信息（<c>scene_info</c>，选填 string(256)）：H5 支付必填，JSON 格式。</summary>
    [JsonPropertyName("scene_info")]
    public string? SceneInfo { get; set; }

    /// <summary>签名（<c>sign</c>，必填 string(32)）：由调用方计算后填入。</summary>
    [JsonPropertyName("sign")]
    public string? Sign { get; set; }
}
