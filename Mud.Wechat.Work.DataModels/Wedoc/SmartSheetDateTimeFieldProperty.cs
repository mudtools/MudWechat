// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 日期类型字段属性（官方 DateTimeFieldProperty）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetDateTimeFieldProperty
{
    /// <summary>
    /// 获取或设置日期格式（官方 <c>format</c>）。
    /// 官方示例：<c>yyyy"年"m"月"d"日"</c>、<c>yyyy-mm-dd</c>、<c>yyyy/m/d</c>、<c>m"月"d"日"</c>、<c>yyyy"年"m"月"d"日" dddd</c>、<c>yyyy"年"m"月"d"日" hh:mm</c>、<c>yyyy-mm-dd hh:mm</c>、<c>m/d/yyyy</c>、<c>d/m/yyyy</c>。
    /// </summary>
    [JsonPropertyName("format")]
    public string? Format { get; set; }

    /// <summary>获取或设置新建记录时是否自动填充时间（官方 <c>auto_fill</c>）。</summary>
    [JsonPropertyName("auto_fill")]
    public bool? AutoFill { get; set; }
}
