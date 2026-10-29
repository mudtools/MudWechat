// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「运费模板」类 DTO（merchant/addfreighttemplate、getfreighttemplatedetail、
// getfreighttemplatelist、updatefreighttemplate）。命名空间恒为 Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>运费计费规则列表项（官方 <c>all_condition_free_detail.condition_free_detail_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsConditionFreeDetail
{
    /// <summary>获取或设置支持的地址列表（官方 <c>address_infos</c>）。</summary>
    [JsonPropertyName("address_infos")]
    public List<ChannelsLogisticsFreightAddressInfo>? AddressInfos { get; set; }

    /// <summary>获取或设置最低件数（官方 <c>min_piece</c>）。</summary>
    [JsonPropertyName("min_piece")]
    public int? MinPiece { get; set; }

    /// <summary>获取或设置最低重量（官方 <c>min_weight</c>，单位千克；订单商品总质量小于一千克算作一千克）。</summary>
    [JsonPropertyName("min_weight")]
    public int? MinWeight { get; set; }

    /// <summary>获取或设置最低金额（官方 <c>min_amount</c>，单位分）。</summary>
    [JsonPropertyName("min_amount")]
    public long? MinAmount { get; set; }

    /// <summary>获取或设置计费方式对应的选项是否已设置（官方 <c>valuation_flag</c>）。</summary>
    [JsonPropertyName("valuation_flag")]
    public int? ValuationFlag { get; set; }

    /// <summary>获取或设置金额是否设置（官方 <c>amount_flag</c>）。</summary>
    [JsonPropertyName("amount_flag")]
    public int? AmountFlag { get; set; }
}
