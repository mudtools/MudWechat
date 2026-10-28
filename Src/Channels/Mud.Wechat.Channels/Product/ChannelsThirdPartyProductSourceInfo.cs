// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>商品在货源平台的信息（官方 <c>product_source_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsThirdPartyProductSourceInfo
{
    /// <summary>获取或设置可访问链接（官方 <c>visit_url</c>，必填）。</summary>
    [JsonPropertyName("visit_url")]
    public string VisitUrl { get; set; } = string.Empty;

    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置标题（官方 <c>title</c>，必填）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>获取或设置主图 URL 列表（官方 <c>head_img_urls</c>，含视频 URL，必填）。</summary>
    [JsonPropertyName("head_img_urls")]
    public List<string> HeadImgUrls { get; set; } = new();

    /// <summary>获取或设置详情图 URL 列表（官方 <c>detail_img_urls</c>，必填）。</summary>
    [JsonPropertyName("detail_img_urls")]
    public List<string> DetailImgUrls { get; set; } = new();

    /// <summary>获取或设置商品资质列表（官方 <c>qualification_list</c>）。</summary>
    [JsonPropertyName("qualification_list")]
    public List<string>? QualificationList { get; set; }

    /// <summary>获取或设置品牌（官方 <c>brand</c>）。</summary>
    [JsonPropertyName("brand")]
    public string? Brand { get; set; }

    /// <summary>获取或设置属性列表（官方 <c>attr_list</c>）。</summary>
    [JsonPropertyName("attr_list")]
    public List<ChannelsThirdPartyProductAttr>? AttrList { get; set; }

    /// <summary>获取或设置规格列表（官方 <c>sku_list</c>，必填）。</summary>
    [JsonPropertyName("sku_list")]
    public List<ChannelsThirdPartyProductSku> SkuList { get; set; } = new();

    /// <summary>获取或设置详情描述（官方 <c>detail_description</c>）。</summary>
    [JsonPropertyName("detail_description")]
    public string? DetailDescription { get; set; }

    /// <summary>获取或设置物流信息（官方 <c>logistics_info</c>）。</summary>
    [JsonPropertyName("logistics_info")]
    public ChannelsThirdPartyLogisticsInfo? LogisticsInfo { get; set; }

    /// <summary>获取或设置支持的服务保障功能（官方 <c>service_guarantee</c>）。</summary>
    [JsonPropertyName("service_guarantee")]
    public string? ServiceGuarantee { get; set; }

    /// <summary>获取或设置发货时效 / 承诺发货时间（官方 <c>delivery_time</c>）。</summary>
    [JsonPropertyName("delivery_time")]
    public string? DeliveryTime { get; set; }
}
