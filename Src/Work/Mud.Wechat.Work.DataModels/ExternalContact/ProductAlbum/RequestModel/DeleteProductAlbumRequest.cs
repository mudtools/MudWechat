// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.ProductAlbum;

/// <summary>
/// 删除商品图册请求体（<c>/cgi-bin/externalcontact/delete_product_album</c>）。
/// <para>应用只可删除应用自己创建的商品图册。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ProductAlbum")]
public class DeleteProductAlbumRequest
{
    /// <summary>
    /// 获取或设置商品 id（官方必填）。
    /// </summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }
}
