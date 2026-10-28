// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 同城阶梯计费（官方 <c>tiered_billing_valuation</c>）。
/// </summary>
/// <remarks>
/// <para>仅 <c>same_city_valuation.type=2</c> 时使用；距离单位米、金额单位分。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductTieredBillingValuation
{
    /// <summary>最低起送金额（分）。</summary>
    [JsonPropertyName("min_send_price")]
    public long? MinSendPrice { get; set; }

    /// <summary>首段距离（米）。</summary>
    [JsonPropertyName("first_val_meter")]
    public int? FirstValMeter { get; set; }

    /// <summary>首段金额（分）。</summary>
    [JsonPropertyName("first_price")]
    public long? FirstPrice { get; set; }

    /// <summary>每增加距离（米）。</summary>
    [JsonPropertyName("second_val_meter")]
    public int? SecondValMeter { get; set; }

    /// <summary>每增加距离的金额（分）。</summary>
    [JsonPropertyName("second_price")]
    public long? SecondPrice { get; set; }
}
