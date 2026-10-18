// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopSpu;

/// <summary>商品详情（<c>get</c> 的 <c>spu</c> / <c>get_list</c> 的 <c>spus[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "ShopSpu")]
public class WxaShopSpu
{
    /// <summary>交易组件平台内部商品 ID。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>商家自定义商品 ID。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>标题。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>绑定的小程序商品路径。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>主图列表。</summary>
    [JsonPropertyName("head_img")]
    public List<string>? HeadImg { get; set; }

    /// <summary>商品详情图文信息。</summary>
    [JsonPropertyName("desc_info")]
    public WxaShopSpuDescInfo? DescInfo { get; set; }

    /// <summary>商品审核信息，可能为空。</summary>
    [JsonPropertyName("audit_info")]
    public WxaShopSpuAuditInfo? AuditInfo { get; set; }

    /// <summary>商品线上状态，取值见 <see cref="WxaShopSpuStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>商品草稿状态，取值见 <see cref="WxaShopSpuEditStatuses"/>。</summary>
    [JsonPropertyName("edit_status")]
    public int? EditStatus { get; set; }

    /// <summary>第三级类目 ID。</summary>
    [JsonPropertyName("third_cat_id")]
    public long? ThirdCatId { get; set; }

    /// <summary>品牌 ID。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }

    /// <summary>创建时间。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>更新时间。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>预留字段，用于版本控制。</summary>
    [JsonPropertyName("info_version")]
    public string? InfoVersion { get; set; }

    /// <summary>SKU 数组。</summary>
    [JsonPropertyName("skus")]
    public List<WxaShopSpuReadSku>? Skus { get; set; }
}
