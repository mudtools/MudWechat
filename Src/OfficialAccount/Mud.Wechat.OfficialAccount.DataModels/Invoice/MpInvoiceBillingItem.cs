// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>
/// 电子发票开票明细项（官方 <c>invoicedetail_list</c> 项；字段名照抄官方拼音缩写）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceBillingItem
{
    /// <summary>获取或设置明细行性质（官方 <c>fphxz</c>；0=正常行，1=折扣行，2=被折扣行）。</summary>
    [JsonPropertyName("fphxz")]
    public int Type { get; set; }

    /// <summary>获取或设置商品编码（官方 <c>spbm</c>）。</summary>
    [JsonPropertyName("spbm")]
    public string GoodsCode { get; set; } = string.Empty;

    /// <summary>获取或设置项目名称（官方 <c>xmmc</c>）。</summary>
    [JsonPropertyName("xmmc")]
    public string Name { get; set; } = string.Empty;

    /// <summary>获取或设置单位（官方 <c>dw</c>）。</summary>
    [JsonPropertyName("dw")]
    public string Unit { get; set; } = string.Empty;

    /// <summary>获取或设置规格型号（官方 <c>ggxh</c>）。</summary>
    [JsonPropertyName("ggxh")]
    public string Specification { get; set; } = string.Empty;

    /// <summary>获取或设置项目数量（官方 <c>xmsl</c>）。</summary>
    [JsonPropertyName("xmsl")]
    public int Count { get; set; }

    /// <summary>获取或设置项目单价（官方 <c>xmdj</c>，单位元）。</summary>
    [JsonPropertyName("xmdj")]
    public decimal Price { get; set; }

    /// <summary>获取或设置项目金额（官方 <c>xmje</c>，单位元）。</summary>
    [JsonPropertyName("xmje")]
    public decimal Amount { get; set; }

    /// <summary>获取或设置税率（官方 <c>sl</c>）。</summary>
    [JsonPropertyName("sl")]
    public decimal TaxRate { get; set; }

    /// <summary>获取或设置税额（官方 <c>se</c>，单位元）。</summary>
    [JsonPropertyName("se")]
    public decimal Tax { get; set; }
}
