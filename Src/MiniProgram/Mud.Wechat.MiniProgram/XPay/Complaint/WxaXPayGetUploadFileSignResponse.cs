// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「获取投诉图片签名头部」响应：<c>Authorization</c> 头部值 + 可选转存地址。
/// </summary>
/// <remarks>
/// <para><b>官方要求的三个下载头部（原文照录）</b>：<c>Authorization</c> 取本响应的 <c>sign</c>、<c>Accept</c> 必须是 <c>application/json</c>、<c>User-Agent</c> 不能为空 ⇒ 缺任一支官方报错，本 SDK 只签发签名、下载动作归宿主。</para>
/// <para><b>敏感值</b>：<c>sign</c> 是访问凭证、<c>cos_url</c> 是 <b>30 分钟有效</b>的临时地址 ⇒ 不得进日志、遥测或异常消息，也不得持久化复用。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_upload_file_sign.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayGetUploadFileSignResponse : WxaResponse
{
    /// <summary>sign（官方类型 <c>string</c>）：返回微信支付图片请求的Authorization头部值，具体使用方法可查看备注</summary>
    [JsonPropertyName("sign")]
    public string? Sign { get; set; }

    /// <summary>cos_url（官方类型 <c>string</c>）：当convert_cos为true时才有意义，返回转存后的url地址，30分钟有效</summary>
    [JsonPropertyName("cos_url")]
    public string? CosUrl { get; set; }
}
