// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.ProductAlbum;

/// <summary>
/// 商品图册详情（<c>product</c>，获取商品图册响应；<c>product_list[]</c> 列表元素复用本类）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ProductAlbum")]
public class ProductAlbumInfo
{
    /// <summary>
    /// 获取或设置商品 id。
    /// </summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>
    /// 获取或设置商品编码。
    /// </summary>
    [JsonPropertyName("product_sn")]
    public string? ProductSn { get; set; }

    /// <summary>
    /// 获取或设置商品名称、特色等描述信息。
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 获取或设置商品价格（单位为分）。
    /// </summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>
    /// 获取或设置商品的创建时间（Unix 时间戳，秒；仅获取商品图册详情返回）。
    /// </summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>
    /// 获取或设置商品图册的附件列表（图片素材可经「获取临时素材」接口下载）。
    /// </summary>
    [JsonPropertyName("attachments")]
    public List<ProductAlbumAttachment>? Attachments { get; set; }
}
