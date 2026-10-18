// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 同城配送模板（查询侧，官方 <c>same_city_template</c>）。
/// </summary>
/// <remarks>
/// <para>查询侧 <c>shipper</c> 与两个 <c>type</c> 均为 string 枚举名；与更新请求侧 number 形态不同源。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductSameCityTemplate
{
    /// <summary>同城配送模板 id。</summary>
    [JsonPropertyName("template_id")]
    public long? TemplateId { get; set; }

    /// <summary>配送方（查询侧枚举名：Shipper_*）。</summary>
    [JsonPropertyName("shipper")]
    public string? Shipper { get; set; }

    /// <summary>配送范围（米）。</summary>
    [JsonPropertyName("scope")]
    public int? Scope { get; set; }

    /// <summary>计价方式。</summary>
    [JsonPropertyName("same_city_valuation")]
    public WxaProductSameCityValuationQuery? SameCityValuation { get; set; }

    /// <summary>特殊时段。</summary>
    [JsonPropertyName("special_time")]
    public WxaProductSpecialTimeQuery? SpecialTime { get; set; }

    /// <summary>配送时间。</summary>
    [JsonPropertyName("delivery_time")]
    public WxaProductDeliveryTime? DeliveryTime { get; set; }
}
