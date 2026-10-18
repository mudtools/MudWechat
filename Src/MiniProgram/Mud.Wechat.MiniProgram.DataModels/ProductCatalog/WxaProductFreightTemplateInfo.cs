// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 运费模板请求体对象（官方 <c>freight_template</c>）。
/// </summary>
/// <remarks>
/// <para><c>template_id</c> 仅更新侧必填；新增侧官方请求表无该列，留空由序列化侧省略。整面覆盖式写入。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductFreightTemplateInfo
{
    /// <summary>模板 id（仅更新侧必填）。</summary>
    [JsonPropertyName("template_id")]
    public long? TemplateId { get; set; }

    /// <summary>运费模板名字（必填）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>计费方式（1 按件 / 2 按重量，必填）。</summary>
    [JsonPropertyName("valuation_type")]
    public int? ValuationType { get; set; }

    /// <summary>发货时间（必填）。</summary>
    [JsonPropertyName("send_time")]
    public int? SendTime { get; set; }

    /// <summary>发货地址。</summary>
    [JsonPropertyName("address_info")]
    public WxaProductAddressInfo? AddressInfo { get; set; }

    /// <summary>配送方式（1 快递，必填）。</summary>
    [JsonPropertyName("delivery_type")]
    public int? DeliveryType { get; set; }

    /// <summary>快递公司列表。</summary>
    [JsonPropertyName("delivery_id")]
    public List<string>? DeliveryId { get; set; }

    /// <summary>是否包邮（必填；不包邮时须在计费方法中指定默认运费）。</summary>
    [JsonPropertyName("shipping_method")]
    public int? ShippingMethod { get; set; }

    /// <summary>条件包邮详情。</summary>
    [JsonPropertyName("all_condition_free_detail")]
    public WxaProductConditionFreeDetail? AllConditionFreeDetail { get; set; }

    /// <summary>具体计费方法。</summary>
    [JsonPropertyName("all_freight_calc_method")]
    public WxaProductFreightCalcMethodsReq? AllFreightCalcMethod { get; set; }

    /// <summary>是否为默认模板。</summary>
    [JsonPropertyName("is_default")]
    public int? IsDefault { get; set; }

    /// <summary>不发货地区。</summary>
    [JsonPropertyName("not_send_area")]
    public WxaProductNotSendArea? NotSendArea { get; set; }
}
