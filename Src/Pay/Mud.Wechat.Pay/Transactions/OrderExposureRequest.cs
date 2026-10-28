// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Transactions;

/// <summary>
/// 订单曝光请求体（微信支付 APIv3）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014326314"/>；
/// <b>POST /v3/pay/transactions/exposure</b>。</para>
/// <para><b>业务场景</b>：当商户在用户端展示订单信息时（如收银台页面），调用本接口上报订单曝光事件，
/// 用于微信支付的风控与数据分析。<b>仅供内部使用</b>，不影响支付流程。</para>
/// <para><b>必填</b>：<c>out_trade_no</c> / <c>exposure_type</c>。其余选填一律可空。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class OrderExposureRequest
{
    /// <summary>
    /// 商户订单号（<c>out_trade_no</c>，必填 string(32)）：6-32 字符，
    /// 只能是数字、大小写字母 <c>_-|*</c>，同一商户号下唯一。
    /// </summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>
    /// 曝光类型（<c>exposure_type</c>，必填 string(16)）：
    /// <c>PAYMENT_CODE</c>（付款码曝光）/ <c>QR_CODE</c>（二维码曝光）。
    /// </summary>
    [JsonPropertyName("exposure_type")]
    public string? ExposureType { get; set; }

    /// <summary>
    /// 曝光时间（<c>exposure_time</c>，选填 string(64)）：rfc3339 格式，如 <c>2018-06-08T10:34:56+08:00</c>。
    /// 不传则默认为请求时间。
    /// </summary>
    [JsonPropertyName("exposure_time")]
    public string? ExposureTime { get; set; }

    /// <summary>
    /// 商户号（<c>mchid</c>，选填 string(32)）：若不传则从签名证书中提取。
    /// </summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }
}
