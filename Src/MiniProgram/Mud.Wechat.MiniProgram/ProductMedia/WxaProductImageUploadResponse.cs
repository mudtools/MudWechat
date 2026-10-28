// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductMedia;

/// <summary>
/// 图片上传回包（<c>POST /product/img/upload</c>，两种模式共用）。
/// </summary>
/// <remarks>
/// <para><b>大小限制按 <c>resp_type</c></b>：resp_type=0 为 2MB、resp_type=1 为 10MB（官方原文）。</para>
/// <para><b>链接永久但限频</b>：resp_type=1 返回 <c>mmecimage.cn/p/{appid}/{imgKey}</c> 形式链接，
/// 同一图片无需重复上传；appid 维度 10000/min、imgKey 维度 100/min，超限 404，
/// 不得用于用户端大量展示。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductMedia")]
public class WxaProductImageUploadResponse : WxaResponse
{
    /// <summary>图片信息。</summary>
    [JsonPropertyName("pic_file")]
    public WxaProductImageFile? PicFile { get; set; }
}
