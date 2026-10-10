// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// RGBA 颜色（官方 Color；TextFormat 的 color，各通道取值 [0,255]）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocColor
{
    /// <summary>获取或设置红色分量，取值 [0,255]（官方 red）。</summary>
    [JsonPropertyName("red")]
    public long? Red { get; set; }

    /// <summary>获取或设置绿色分量，取值 [0,255]（官方 green）。</summary>
    [JsonPropertyName("green")]
    public long? Green { get; set; }

    /// <summary>获取或设置蓝色分量，取值 [0,255]（官方 blue）。</summary>
    [JsonPropertyName("blue")]
    public long? Blue { get; set; }

    /// <summary>获取或设置 alpha 通道，取值 [0,255]，默认 255 完全不透明（官方 alpha）。</summary>
    [JsonPropertyName("alpha")]
    public long? Alpha { get; set; }
}
