// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------
namespace Mud.Wechat.Channels.DataModels.Talent;

/// <summary>佣金单列表条目（官方 <c>order_list</c> 数组项：列表侧只回三个定位键，详情须再调 <c>get_order_detail</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Talent")]
public class ChannelsTalentOrderListItem
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置商品 sku id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置佣金单特殊标识（官方 <c>special_id</c>，目前主要用于本地生活；调详情端点时须回传）。</summary>
    [JsonPropertyName("special_id")]
    public string? SpecialId { get; set; }
}
