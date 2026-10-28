// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 文档表格单元格四边边框（官方 Borders）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocumentBorders
{
    /// <summary>获取或设置上边框（官方 top，<see cref="WedocDocumentBorderProperty"/>）。</summary>
    [JsonPropertyName("top")]
    public WedocDocumentBorderProperty? Top { get; set; }

    /// <summary>获取或设置左边框（官方 left，<see cref="WedocDocumentBorderProperty"/>）。</summary>
    [JsonPropertyName("left")]
    public WedocDocumentBorderProperty? Left { get; set; }

    /// <summary>获取或设置下边框（官方 bottom，<see cref="WedocDocumentBorderProperty"/>）。</summary>
    [JsonPropertyName("bottom")]
    public WedocDocumentBorderProperty? Bottom { get; set; }

    /// <summary>获取或设置右边框（官方 right，<see cref="WedocDocumentBorderProperty"/>）。</summary>
    [JsonPropertyName("right")]
    public WedocDocumentBorderProperty? Right { get; set; }
}
