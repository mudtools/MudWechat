// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 商品头图视频（官方 <c>head_videos</c> 对象：<c>video_url</c>，最多 1 个）。
/// </summary>
/// <remarks>
/// 官方契约：不传递该字段不会删除商品原有视频；如需删除视频，请显式传入一个空的 <c>video_url</c> 字符串。
/// 官方变更日志：<c>video_url</c> 字段类型已从 array 更新为 string。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductHeadVideo
{
    /// <summary>获取或设置头图视频 url（官方 <c>video_url</c>；传空字符串表示删除视频）。</summary>
    [JsonPropertyName("video_url")]
    public string? VideoUrl { get; set; }
}
