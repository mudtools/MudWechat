// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「获取微信支付反馈投诉图片的签名头部」请求体（<c>POST /xpay/get_upload_file_sign</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>convert_cos</c> 决定返回是否有意义</b>：<c>cos_url</c> 官方原文「当convert_cos为true时才有意义」。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_upload_file_sign.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayGetUploadFileSignRequest
{
    /// <summary>env（官方类型 <c>number</c>，<b>必填</b>）：0-正式环境 1-沙箱环境（仅作为签名校验，查询的结果都是正式环境的）</summary>
    [JsonPropertyName("env")]
    public long? Env { get; set; }

    /// <summary>wxpay_url（官方类型 <c>string</c>，<b>必填</b>）：微信支付的图片地址格式为https://api.mch.weixin.qq.com/v3/merchant-service/images/{xxxxxx}</summary>
    [JsonPropertyName("wxpay_url")]
    public string? WxpayUrl { get; set; }

    /// <summary>convert_cos（官方类型 <c>boolean</c>，<b>必填</b>）：是否转存到cos，转存后可以获得图片的临时下载地址，30分钟有效</summary>
    [JsonPropertyName("convert_cos")]
    public bool? ConvertCos { get; set; }

    /// <summary>complaint_id（官方类型 <c>string</c>，<b>必填</b>）：对应的反馈投诉id</summary>
    [JsonPropertyName("complaint_id")]
    public string? ComplaintId { get; set; }
}
