// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>外部商品属性映射（新版，<c>externalproductmappingnew</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsExternalProductMappingNewRequest
{
    /// <summary>获取或设置叶子类目 id（官方 <c>cat_id</c>，必填）。</summary>
    [JsonPropertyName("cat_id")]
    public long CatId { get; set; }

    /// <summary>获取或设置外部商品类目名称（官方 <c>external_category_name</c>）。</summary>
    [JsonPropertyName("external_category_name")]
    public string? ExternalCategoryName { get; set; }

    /// <summary>获取或设置主图列表（官方 <c>head_imgs</c>，必填：至少传一张）。</summary>
    [JsonPropertyName("head_imgs")]
    public List<string> HeadImgs { get; set; } = new();

    /// <summary>获取或设置详情图列表（官方 <c>detail_imgs</c>）。</summary>
    [JsonPropertyName("detail_imgs")]
    public List<string>? DetailImgs { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>，必填）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>获取或设置属性列表（官方 <c>external_attributes</c>）。</summary>
    [JsonPropertyName("external_attributes")]
    public List<ChannelsExternalProductAttr>? ExternalAttributes { get; set; }
}
