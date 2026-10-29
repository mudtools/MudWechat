// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>商品类目鉴定（<c>product/category/classify</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductClassifyRequest
{
    /// <summary>获取或设置请求类型（官方 <c>req_type</c>，必填）：1 基于标题和主图推断商品类目，2 基于商品内容判断是否类目错放。</summary>
    [JsonPropertyName("req_type")]
    public int ReqType { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置商品主图列表（官方 <c>head_imgs</c>，必填：至少传入一个有效的头图 url）。</summary>
    [JsonPropertyName("head_imgs")]
    public List<string> HeadImgs { get; set; } = new();

    /// <summary>获取或设置类目 id（官方 <c>cat_id</c>，请求类型为 2 时必填）。</summary>
    [JsonPropertyName("cat_id")]
    public string? CatId { get; set; }
}
