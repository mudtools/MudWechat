// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 二维变换（官方 Transform2D）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocumentTransform2D
{
    /// <summary>获取或设置图形尺寸（官方 extent，<see cref="WedocDocumentPositiveSize2D"/>）。</summary>
    [JsonPropertyName("extent")]
    public WedocDocumentPositiveSize2D? Extent { get; set; }

    /// <summary>获取或设置旋转角度，单位 1/60000 度（官方 rotation）。</summary>
    [JsonPropertyName("rotation")]
    public long? Rotation { get; set; }
}
