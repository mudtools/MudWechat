// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「上传媒体文件」请求体（<c>POST /xpay/upload_vp_file</c>）：为道具图/投诉凭证换取 <c>file_id</c>。
/// </summary>
/// <remarks>
/// <para><b>本端点不是 multipart</b>：官方 <c>Content-Type</c> 为 JSON，图片以 <c>base64_img</c> 或 <c>img_url</c> 字段承载 ⇒ 与本线短剧媒资的分片上传（multipart 手工通路）形态不同，<b>不得</b>照搬 <c>IFormContent</c>。</para>
/// <para><b>两条通道各有大小上限且官方推荐后者</b>：<c>base64_img</c> ≤1MB、<c>img_url</c> ≤2MB，官方原文「优先使用img_url」；且 <c>img_url</c> 必须能直接下载，<b>不能是 302 类跳转地址</b>。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_upload_vp_file.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayUploadVpFileRequest
{
    /// <summary>env（官方类型 <c>number</c>，<b>必填</b>）：0-正式环境 1-沙箱环境（仅作为签名校验，查询的结果都是正式环境的）</summary>
    [JsonPropertyName("env")]
    public long? Env { get; set; }

    /// <summary>base64_img（官方类型 <c>string</c>，<b>必填</b>）：经base64编码后的图片内容，使用这个字段最多只能传1m的图片，超过1m请使用img_url字段</summary>
    [JsonPropertyName("base64_img")]
    public string? Base64Img { get; set; }

    /// <summary>img_url（官方类型 <c>string</c>，<b>必填</b>）：图片url，需要能直接下载，不能是返回302等返回码的地址，最高允许传2m图片（优先使用img_url）</summary>
    [JsonPropertyName("img_url")]
    public string? ImgUrl { get; set; }

    /// <summary>file_name（官方类型 <c>string</c>，<b>必填</b>）：图片名称</summary>
    [JsonPropertyName("file_name")]
    public string? FileName { get; set; }
}
