// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 条件包邮项（官方 <c>condition_free_detail_list[]</c>）。
/// </summary>
/// <remarks>
/// <para><c>address_infos</c> 结构同 <c>address_info</c>；<c>min_piece</c> / <c>min_weight</c> / <c>min_amount</c> 按计费方式取值。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductConditionFreeDetailItem
{
    /// <summary>包邮地区（结构同 address_info）。</summary>
    [JsonPropertyName("address_infos")]
    public WxaProductAddressInfo? AddressInfos { get; set; }

    /// <summary>包邮最低件数。</summary>
    [JsonPropertyName("min_piece")]
    public int? MinPiece { get; set; }

    /// <summary>包邮最低重量（kg）。</summary>
    [JsonPropertyName("min_weight")]
    public double? MinWeight { get; set; }

    /// <summary>包邮最低金额（分）。</summary>
    [JsonPropertyName("min_amount")]
    public long? MinAmount { get; set; }

    /// <summary>计费方式对应选项是否已经设置。</summary>
    [JsonPropertyName("valuation_flag")]
    public int? ValuationFlag { get; set; }

    /// <summary>金额是否设置。</summary>
    [JsonPropertyName("amount_flag")]
    public int? AmountFlag { get; set; }
}
