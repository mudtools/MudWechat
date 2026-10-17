// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 物流信息（官方 <c>item.product_info.freight_info</c>，2026-03-09 新增）。
/// </summary>
/// <remarks>
/// <para>
/// 与商家侧运费模板对象 <see cref="Mud.Wechat.Channels.DataModels.Product.ChannelsProductExpressInfo"/>
/// （<c>template_id</c> / <c>weight</c> / <c>express_type</c>）字段集完全不同 ⇒ 分建 DTO。
/// </para>
/// <para>
/// <c>valuation_type</c> / <c>shipping_method</c> 官方「类型」列写 number，但枚举说明与代码示例均为字符串名
/// （<c>PIECE</c> / <c>WEIGHT</c> / <c>FREE</c> / <c>CONDITION_FREE</c> / <c>NO_FREE</c>，示例 <c>"shipping_method": "NO_FREE"</c>）
/// ⇒ 建模为 string 兼容两种形态。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierFreightInfo
{
    /// <summary>获取或设置发货时间期限（官方 <c>send_time</c>，枚举取值见 <see cref="ChannelsLeagueHeadSupplierSendTimes"/>）。</summary>
    [JsonPropertyName("send_time")]
    public string? SendTime { get; set; }

    /// <summary>获取或设置计费类型（官方 <c>valuation_type</c>：PIECE 按件数 / WEIGHT 按重量）。</summary>
    [JsonPropertyName("valuation_type")]
    public string? ValuationType { get; set; }

    /// <summary>获取或设置发货地址（官方 <c>address_info</c>）。</summary>
    [JsonPropertyName("address_info")]
    public ChannelsLeagueHeadSupplierFreightAddressInfo? AddressInfo { get; set; }

    /// <summary>获取或设置计费方式（官方 <c>shipping_method</c>：FREE 包邮 / CONDITION_FREE 条件包邮 / NO_FREE 不包邮）。</summary>
    [JsonPropertyName("shipping_method")]
    public string? ShippingMethod { get; set; }

    /// <summary>获取或设置条件包邮详情（官方 <c>condition_free_detail_list</c>）。</summary>
    [JsonPropertyName("condition_free_detail_list")]
    public List<ChannelsLeagueHeadSupplierConditionFreeDetail>? ConditionFreeDetailList { get; set; }

    /// <summary>获取或设置具体计费方法（官方 <c>freight_calc_method_list</c>：默认运费、指定地区运费等）。</summary>
    [JsonPropertyName("freight_calc_method_list")]
    public List<ChannelsLeagueHeadSupplierFreightCalcMethod>? FreightCalcMethodList { get; set; }

    /// <summary>获取或设置不发货区域（官方 <c>not_send_area</c>）。</summary>
    [JsonPropertyName("not_send_area")]
    public List<ChannelsLeagueHeadSupplierFreightAddressInfo>? NotSendArea { get; set; }
}
