// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Search;

/// <summary>搜一搜数据推送的视频信息（<c>video[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Search")]
public class WxaSearchVideo
{
    /// <summary>视频标题（<c>video_title</c>）。</summary>
    [JsonPropertyName("video_title")]
    public string? VideoTitle { get; set; }

    /// <summary>视频时长（<c>video_length</c>，单位：秒）。</summary>
    [JsonPropertyName("video_length")]
    public long? VideoLength { get; set; }

    /// <summary>视频封面图 URL（<c>video_img</c>）。</summary>
    [JsonPropertyName("video_img")]
    public string? VideoImageUrl { get; set; }
}
