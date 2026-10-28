// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 运费险信息（官方 <c>InsuranceInfo</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrderInsuranceInfo
{
    /// <summary>运费险类型，见 <see cref="WxaProductOrderInsuranceTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>运费险价格，单位<b>分</b>。</summary>
    [JsonPropertyName("insurance_price")]
    public long? InsurancePrice { get; set; }
}
