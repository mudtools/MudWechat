// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>获取商品（<c>product/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetProductResponse : ChannelsResponse
{
    /// <summary>获取或设置商品线上数据（官方 <c>product</c>；<c>data_type==2</c> 时不返回）。</summary>
    [JsonPropertyName("product")]
    public ChannelsProductDetail? Product { get; set; }

    /// <summary>获取或设置商品草稿数据（官方 <c>edit_product</c>；<c>data_type==1</c> 时不返回）。</summary>
    [JsonPropertyName("edit_product")]
    public ChannelsProductDetail? EditProduct { get; set; }

    /// <summary>获取或设置当日售卖上限提醒（官方 <c>sale_limit_info</c>）。</summary>
    [JsonPropertyName("sale_limit_info")]
    public ChannelsSaleLimitInfo? SaleLimitInfo { get; set; }

    /// <summary>获取或设置商品信息质量（官方 <c>info_score</c>）。</summary>
    [JsonPropertyName("info_score")]
    public ChannelsProductInfoScore? InfoScore { get; set; }

    /// <summary>获取或设置商品高价预警（官方 <c>cmp_price_info</c>）。</summary>
    [JsonPropertyName("cmp_price_info")]
    public ChannelsProductCmpPriceInfo? CmpPriceInfo { get; set; }

    /// <summary>获取或设置审核信息（官方 <c>audit_info</c>）。</summary>
    [JsonPropertyName("audit_info")]
    public ChannelsProductAuditInfo? AuditInfo { get; set; }
}
