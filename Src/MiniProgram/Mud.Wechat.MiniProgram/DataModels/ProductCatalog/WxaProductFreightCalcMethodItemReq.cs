// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 运费模板计费项（请求侧，官方 <c>freight_calc_method_list[]</c>）。
/// </summary>
/// <remarks>
/// <para>请求侧 <c>is_default</c> 官方类型为 boolean（原文拼作 <c>boolen</c>）；为默认运费时 <c>address_info</c> 必须为空。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductFreightCalcMethodItemReq
{
    /// <summary>指定地区（默认运费时为空）。</summary>
    [JsonPropertyName("address_info")]
    public WxaProductAddressInfo? AddressInfo { get; set; }

    /// <summary>是否为默认运费（请求侧 boolean）。</summary>
    [JsonPropertyName("is_default")]
    public bool? IsDefault { get; set; }

    /// <summary>快递公司 id。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>首段运费满足重量（kg）。</summary>
    [JsonPropertyName("first_val_amount")]
    public double? FirstValAmount { get; set; }

    /// <summary>首段运费金额（分）。</summary>
    [JsonPropertyName("first_price")]
    public long? FirstPrice { get; set; }

    /// <summary>续费单位重量（kg）。</summary>
    [JsonPropertyName("second_val_amount")]
    public double? SecondValAmount { get; set; }

    /// <summary>续费每单位增加金额（分）。</summary>
    [JsonPropertyName("second_price")]
    public long? SecondPrice { get; set; }
}
