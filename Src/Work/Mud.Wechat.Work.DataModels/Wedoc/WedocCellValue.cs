// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 单元格数据内容（官方 CellValue；暂时只支持文本、链接，一个对象中只能选填一个字段）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocCellValue
{
    /// <summary>获取或设置文本内容（官方 text）。</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>获取或设置超链接内容（官方 link，<see cref="WedocLink"/>）。</summary>
    [JsonPropertyName("link")]
    public WedocLink? Link { get; set; }
}
