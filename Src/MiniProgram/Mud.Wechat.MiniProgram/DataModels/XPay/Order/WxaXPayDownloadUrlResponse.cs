// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「下载链接」应答（单字段 <c>url</c>），小程序账单与广告金商户订单两支共用。
/// </summary>
/// <remarks>
/// <para><b>两处共用裁决</b>：<c>download_bill</c> 与 <c>download_adverfunds_order</c> 的官方返回体表逐字段相同（除 <c>errcode</c>/<c>errmsg</c> 外只有 <c>url</c>）⇒ 共用本类型，由 <c>sameAs</c> 断言取证。</para>
/// <para><b>敏感值</b>：<c>url</c> 是账单文件下载链接 ⇒ 不得进日志、遥测或异常消息。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_download_bill.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayDownloadUrlResponse : WxaResponse
{
    /// <summary>url（官方类型 <c>string</c>）：下载地址（有效时间为半小时，失效后需重新获取）</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
