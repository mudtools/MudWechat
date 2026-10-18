// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopAccount;

/// <summary>
/// 上传图片应答（<c>POST /shop/img/upload</c>，官方 <c>img_info</c> 一层包装）。
/// </summary>
/// <remarks>
/// <para><b>请求侧无 DTO</b>：官方 <c>Content-Type</c> 为 <c>multipart/form-data</c>
/// （官方请求示例 <c>curl -F media=@test.jpg ".../shop/img/upload?access_token=xxx" -F resp_type=0</c>）
/// ⇒ 走声明式 <c>[MultipartForm] IFormContent</c> 通路（仓内既有形态：短剧媒资单文件上传、
/// 公众号素材、企业微信临时素材），表单字段名须照官方原文（<c>media</c> / <c>resp_type</c> /
/// <c>upload_type</c> / <c>img_url</c>，<c>resp_type = 2</c> 且 <c>upload_type = 0</c> 时另需 <c>filename</c>）。</para>
/// <para><b>回包三支里只会填你请求的那一支</b>（<c>resp_type</c> 决定 0/1/2 语义）⇒ 判空按
/// <see cref="WxaShopImageInfo"/> 的逐支说明，不得三支同时取值。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopImageUploadResponse : WxaResponse
{
    /// <summary>图片信息（<c>img_info</c>，object）。</summary>
    [JsonPropertyName("img_info")]
    public WxaShopImageInfo? ImgInfo { get; set; }
}
