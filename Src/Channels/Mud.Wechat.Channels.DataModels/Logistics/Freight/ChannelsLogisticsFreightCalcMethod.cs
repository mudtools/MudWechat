// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「运费模板」类 DTO（merchant/addfreighttemplate、getfreighttemplatedetail、
// getfreighttemplatelist、updatefreighttemplate）。命名空间恒为 Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>具体计费方法列表项（官方 <c>all_freight_calc_method.freight_calc_method_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsFreightCalcMethod
{
    /// <summary>获取或设置支持的地址列表（官方 <c>address_infos</c>）。</summary>
    [JsonPropertyName("address_infos")]
    public List<ChannelsLogisticsFreightAddressInfo>? AddressInfos { get; set; }

    /// <summary>获取或设置是否默认运费（官方 <c>is_default</c>）。</summary>
    [JsonPropertyName("is_default")]
    public bool? IsDefault { get; set; }

    /// <summary>获取或设置快递公司（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置首段运费需要满足的数量（官方 <c>first_val_amount</c>）。</summary>
    [JsonPropertyName("first_val_amount")]
    public long? FirstValAmount { get; set; }

    /// <summary>获取或设置首段运费的金额（官方 <c>first_price</c>）。</summary>
    [JsonPropertyName("first_price")]
    public long? FirstPrice { get; set; }

    /// <summary>获取或设置续费的数量（官方 <c>second_val_amount</c>）。</summary>
    [JsonPropertyName("second_val_amount")]
    public long? SecondValAmount { get; set; }

    /// <summary>获取或设置续费的金额（官方 <c>second_price</c>）。</summary>
    [JsonPropertyName("second_price")]
    public long? SecondPrice { get; set; }
}
