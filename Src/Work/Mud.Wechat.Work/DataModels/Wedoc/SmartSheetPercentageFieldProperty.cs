// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 百分数类型字段属性（官方 PercentageFieldProperty）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetPercentageFieldProperty
{
    /// <summary>获取或设置小数点的位数，即数字精度（官方 <c>decimal_places</c>）。</summary>
    [JsonPropertyName("decimal_places")]
    public int? DecimalPlaces { get; set; }

    /// <summary>获取或设置是否使用千位符（官方 <c>use_separate</c>），设置后使用英文逗号分隔千分位，例如 <c>1,000</c>。</summary>
    [JsonPropertyName("use_separate")]
    public bool? UseSeparate { get; set; }
}
