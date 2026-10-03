// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.ProductAlbum;

/// <summary>
/// 创建商品图册请求体（<c>/cgi-bin/externalcontact/add_product_album</c>）。
/// <para><see cref="Description"/> / <see cref="Price"/> / <see cref="Attachments"/> 为官方必填；
/// 图片素材须经「上传附件资源」接口（attachment_type = 2）获得。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ProductAlbum")]
public class AddProductAlbumRequest
{
    /// <summary>
    /// 获取或设置商品名称、特色等描述信息（官方必填；不超过 300 字）。
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 获取或设置商品价格（官方必填；单位为分，最大不超过 5 万元）。
    /// </summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>
    /// 获取或设置商品编码（不超过 128 字节，仅支持数字和字母）。
    /// </summary>
    [JsonPropertyName("product_sn")]
    public string? ProductSn { get; set; }

    /// <summary>
    /// 获取或设置商品图册的附件列表（官方必填；仅支持 image 类型，最多 9 个）。
    /// </summary>
    [JsonPropertyName("attachments")]
    public List<ProductAlbumAttachment>? Attachments { get; set; }
}
