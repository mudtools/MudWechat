// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>带货者商品数据条目（官方 <c>list</c> 数组元素，<c>talent/getproductdatalist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueTalentProductDataItem
{
    /// <summary>获取或设置商品 id（官方 <c>spu_id</c>）。</summary>
    [JsonPropertyName("spu_id")]
    public long? SpuId { get; set; }

    /// <summary>获取或设置商品数据（官方 <c>product_data</c>）。</summary>
    [JsonPropertyName("product_data")]
    public ChannelsLeagueProductData? ProductData { get; set; }

    /// <summary>获取或设置带货者佣金信息（官方 <c>talent_commission_info</c>）。</summary>
    [JsonPropertyName("talent_commission_info")]
    public ChannelsLeagueTalentCommissionInfo? TalentCommissionInfo { get; set; }
}
