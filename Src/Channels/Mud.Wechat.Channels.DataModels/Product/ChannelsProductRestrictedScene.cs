// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>受限场景（官方 <c>scene_info</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductRestrictedScene
{
    /// <summary>获取或设置受限场景（官方 <c>scene</c>，见 <see cref="ChannelsProductRestrictedScenes"/>）。</summary>
    [JsonPropertyName("scene")]
    public int? Scene { get; set; }

    /// <summary>获取或设置策略来源（官方 <c>source</c>，见 <see cref="ChannelsProductRestrictedSources"/>）。</summary>
    [JsonPropertyName("source")]
    public int? Source { get; set; }
}
