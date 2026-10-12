// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>订单流水商品（官方 <c>product_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsOrderFlowProduct
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品规格列表（官方 <c>param_list</c>）。</summary>
    [JsonPropertyName("param_list")]
    public List<ChannelsOrderFlowProductParam>? ParamList { get; set; }

    /// <summary>获取或设置商品销售价格（官方 <c>sale_price</c>，单位：分）。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>获取或设置商品数量（官方 <c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }

    /// <summary>获取或设置商品名称（官方 <c>product_name</c>）。</summary>
    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    /// <summary>获取或设置是否赠品（官方 <c>is_gift</c>）。</summary>
    [JsonPropertyName("is_gift")]
    public bool? IsGift { get; set; }
}
