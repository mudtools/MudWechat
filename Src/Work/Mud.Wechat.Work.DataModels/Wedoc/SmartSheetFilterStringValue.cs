// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 过滤条件的文本值（官方 Condition 的 <c>string_value</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetFilterStringValue
{
    /// <summary>
    /// 获取或设置取值列表（官方 <c>value</c>）。
    /// 文本、网址、电话、邮箱、地理位置、单选、多选等列类型使用；选项列为选项 ID，其他为文本值。
    /// </summary>
    [JsonPropertyName("value")]
    public List<string>? Value { get; set; }
}
