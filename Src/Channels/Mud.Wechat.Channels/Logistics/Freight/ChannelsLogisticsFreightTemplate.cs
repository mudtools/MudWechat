// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「运费模板」类 DTO（merchant/addfreighttemplate、getfreighttemplatedetail、
// getfreighttemplatelist、updatefreighttemplate）。命名空间恒为 Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>运费模板详细信息（官方 <c>freight_template</c>，增改查共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsFreightTemplate
{
    /// <summary>获取或设置模板 id（官方 <c>template_id</c>）。</summary>
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>获取或设置模板名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置计费类型（官方 <c>valuation_type</c>）：PIECE 按件数；WEIGHT 按重量。</summary>
    [JsonPropertyName("valuation_type")]
    public string? ValuationType { get; set; }

    /// <summary>获取或设置发货时间期限详情（官方 <c>send_time</c>）：SendTime_TWENTYFOUR_HOUR / SendTime_FOUTYEIGHT_HOUR / SendTime_SHIP_TODAY。</summary>
    [JsonPropertyName("send_time")]
    public string? SendTime { get; set; }

    /// <summary>获取或设置发货地址（官方 <c>address_info</c>）。</summary>
    [JsonPropertyName("address_info")]
    public ChannelsLogisticsFreightAddressInfo? AddressInfo { get; set; }

    /// <summary>获取或设置运输方式（官方 <c>delivery_type</c>）：EXPRESS 快递。</summary>
    [JsonPropertyName("delivery_type")]
    public string? DeliveryType { get; set; }

    /// <summary>获取或设置计费方式（官方 <c>shipping_method</c>）：FREE 包邮 / CONDITION_FREE 条件包邮 / NO_FREE 不包邮。</summary>
    [JsonPropertyName("shipping_method")]
    public string? ShippingMethod { get; set; }

    /// <summary>获取或设置条件包邮详情（官方 <c>all_condition_free_detail</c>）。</summary>
    [JsonPropertyName("all_condition_free_detail")]
    public ChannelsLogisticsAllConditionFreeDetail? AllConditionFreeDetail { get; set; }

    /// <summary>获取或设置具体计费方法（官方 <c>all_freight_calc_method</c>）。</summary>
    [JsonPropertyName("all_freight_calc_method")]
    public ChannelsLogisticsAllFreightCalcMethod? AllFreightCalcMethod { get; set; }

    /// <summary>获取或设置创建时间戳（官方 <c>create_time</c>）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间戳（官方 <c>update_time</c>）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置是否默认模板（官方 <c>is_default</c>）。</summary>
    [JsonPropertyName("is_default")]
    public bool? IsDefault { get; set; }

    /// <summary>获取或设置不发货区域（官方 <c>not_send_area</c>）。</summary>
    [JsonPropertyName("not_send_area")]
    public ChannelsLogisticsNotSendArea? NotSendArea { get; set; }
}
