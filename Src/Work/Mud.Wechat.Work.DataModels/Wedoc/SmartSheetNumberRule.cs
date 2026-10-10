// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 自动编号的自定义规则（官方 NumberRule）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetNumberRule
{
    /// <summary>
    /// 获取或设置规则类型（官方 <c>type</c>）。
    /// 官方取值：<c>NUMBER_RULE_TYPE_INCR</c> 自增 ID、<c>NUMBER_RULE_TYPE_FIXED_CHAR</c> 固定字符、<c>NUMBER_RULE_TYPE_TIME</c> 创建时间。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置规则值（官方 <c>value</c>）。
    /// 存放创建时间格式（官方 <c>CreateTimeFormat</c>：<c>YYYYMMDD</c> / <c>YYYYMM</c> / <c>MMDD</c> / <c>YYYY</c> / <c>MM</c> / <c>DD</c>）或固定字符、自增数字位数。
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
