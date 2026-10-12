// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 更新商品（<c>product/update</c>）请求体 —— 字段集与 <see cref="ChannelsAddProductRequest"/> 一致，额外携带 <c>product_id</c>。
/// </summary>
/// <remarks>官方契约：<b>POST</b> + 请求体；更新只影响草稿数据，需上架并审核通过后生效。</remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsUpdateProductRequest : ChannelsAddProductRequest
{
    /// <summary>获取或设置小店内部商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }
}
