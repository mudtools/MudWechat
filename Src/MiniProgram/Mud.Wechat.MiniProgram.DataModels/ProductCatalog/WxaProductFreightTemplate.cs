// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 运费模板（查询侧，官方 <c>template_list[]</c>）。
/// </summary>
/// <remarks>
/// <para>查询侧独有 <c>template_id</c> / <c>create_time</c> / <c>update_time</c>；请求侧无时间戳。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductFreightTemplate
{
    /// <summary>运费模板 id。</summary>
    [JsonPropertyName("template_id")]
    public long? TemplateId { get; set; }

    /// <summary>运费模板名字。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>计费方式（1 按件 / 2 按重量）。</summary>
    [JsonPropertyName("valuation_type")]
    public int? ValuationType { get; set; }

    /// <summary>发货时间（官方枚举 1~19）。</summary>
    [JsonPropertyName("send_time")]
    public int? SendTime { get; set; }

    /// <summary>发货地址。</summary>
    [JsonPropertyName("address_info")]
    public WxaProductAddressInfo? AddressInfo { get; set; }

    /// <summary>配送方式（1 快递）。</summary>
    [JsonPropertyName("delivery_type")]
    public int? DeliveryType { get; set; }

    /// <summary>快递公司列表。</summary>
    [JsonPropertyName("delivery_id")]
    public List<string>? DeliveryId { get; set; }

    /// <summary>是否包邮（1 包邮 / 2 条件包邮 / 3 不包邮）。</summary>
    [JsonPropertyName("shipping_method")]
    public int? ShippingMethod { get; set; }

    /// <summary>条件包邮详情。</summary>
    [JsonPropertyName("all_condition_free_detail")]
    public WxaProductConditionFreeDetail? AllConditionFreeDetail { get; set; }

    /// <summary>指定地区具体计费方法。</summary>
    [JsonPropertyName("all_freight_calc_method")]
    public WxaProductFreightCalcMethods? AllFreightCalcMethod { get; set; }

    /// <summary>创建时间（秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>更新时间（秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>是否为默认模板（查询侧 number）。</summary>
    [JsonPropertyName("is_default")]
    public int? IsDefault { get; set; }

    /// <summary>不发货地区。</summary>
    [JsonPropertyName("not_send_area")]
    public WxaProductNotSendArea? NotSendArea { get; set; }
}
