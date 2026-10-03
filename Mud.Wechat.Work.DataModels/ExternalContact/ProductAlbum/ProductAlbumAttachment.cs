// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.ProductAlbum;

/// <summary>
/// 商品图册的图片附件（<c>attachments[]</c> 元素）。
/// <para>附件类型仅支持 <c>image</c>，最多 9 个；图片素材须经「上传附件资源」接口获得
/// （attachment_type = 2 商品图册场景），可经「获取临时素材」接口下载。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ProductAlbum")]
public class ProductAlbumAttachment
{
    /// <summary>
    /// 获取或设置附件类型（仅支持 <c>image</c>）。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置图片附件详情。
    /// </summary>
    [JsonPropertyName("image")]
    public ProductAlbumImage? Image { get; set; }
}

/// <summary>
/// 商品图册的图片详情（<c>attachments[].image</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ProductAlbum")]
public class ProductAlbumImage
{
    /// <summary>
    /// 获取或设置图片的素材 id（经「上传附件资源」接口获得）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}
