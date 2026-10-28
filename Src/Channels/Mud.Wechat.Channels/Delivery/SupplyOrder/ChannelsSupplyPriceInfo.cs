// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>供货单价格信息（官方 <c>price_info</c> 数组元素，<c>supplyorder/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplyPriceInfo
{
    /// <summary>获取或设置供货单总价（官方 <c>total_price</c>，单位：分；官方口径：商品售价 × 商品数量 + 运费）。</summary>
    [JsonPropertyName("total_price")]
    public long? TotalPrice { get; set; }
}
