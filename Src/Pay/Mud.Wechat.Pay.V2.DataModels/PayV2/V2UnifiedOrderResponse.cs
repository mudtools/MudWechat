// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 统一下单应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2UnifiedOrderResponse
{
    /// <summary>返回状态码（<c>return_code</c>，必填 string(16)）：<c>SUCCESS</c> / <c>FAIL</c>。</summary>
    [JsonPropertyName("return_code")]
    public string? ReturnCode { get; set; }

    /// <summary>返回信息（<c>return_msg</c>，选填 string(128)）：非 <c>OK</c> 时为错误原因。</summary>
    [JsonPropertyName("return_msg")]
    public string? ReturnMsg { get; set; }

    /// <summary>公众账号 ID（<c>appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户号（<c>mch_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>子商户号（<c>sub_mch_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_mch_id")]
    public string? SubMchId { get; set; }

    /// <summary>设备号（<c>device_info</c>，选填 string(32)）。</summary>
    [JsonPropertyName("device_info")]
    public string? DeviceInfo { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>，选填 string(32)）。</summary>
    [JsonPropertyName("nonce_str")]
    public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sign")]
    public string? Sign { get; set; }

    /// <summary>业务结果（<c>result_code</c>，选填 string(16)）：<c>SUCCESS</c> / <c>FAIL</c>。</summary>
    [JsonPropertyName("result_code")]
    public string? ResultCode { get; set; }

    /// <summary>错误代码（<c>err_code</c>，选填 string(32)）：<c>result_code=FAIL</c> 时有值。</summary>
    [JsonPropertyName("err_code")]
    public string? ErrCode { get; set; }

    /// <summary>错误代码描述（<c>err_code_des</c>，选填 string(128)）。</summary>
    [JsonPropertyName("err_code_des")]
    public string? ErrCodeDes { get; set; }

    /// <summary>交易类型（<c>trade_type</c>，选填 string(16)）。</summary>
    [JsonPropertyName("trade_type")]
    public string? TradeType { get; set; }

    /// <summary>预支付交易会话标识（<c>prepay_id</c>，选填 string(64)）：有效期 2 小时。</summary>
    [JsonPropertyName("prepay_id")]
    public string? PrepayId { get; set; }

    /// <summary>二维码链接（<c>code_url</c>，选填 string(64)）：<c>trade_type=NATIVE</c> 时返回。</summary>
    [JsonPropertyName("code_url")]
    public string? CodeUrl { get; set; }

    /// <summary>H5 支付链接（<c>mweb_url</c>，选填 string(128)）：<c>trade_type=MWEB</c> 时返回。</summary>
    [JsonPropertyName("mweb_url")]
    public string? MwebUrl { get; set; }
}
