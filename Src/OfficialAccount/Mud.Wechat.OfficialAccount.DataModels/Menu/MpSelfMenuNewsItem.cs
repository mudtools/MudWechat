// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Menu;

/// <summary>图文消息条目（<c>news_info.list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpSelfMenuNewsItem
{
    /// <summary>标题。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>摘要。</summary>
    [JsonPropertyName("digest")]
    public string? Digest { get; set; }

    /// <summary>作者。</summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>是否显示封面（<c>0</c> 不显示 / <c>1</c> 显示）。</summary>
    [JsonPropertyName("show_cover")]
    public int ShowCover { get; set; }

    /// <summary>封面图片的 URL。</summary>
    [JsonPropertyName("cover_url")]
    public string? CoverUrl { get; set; }

    /// <summary>正文的 URL。</summary>
    [JsonPropertyName("content_url")]
    public string? ContentUrl { get; set; }

    /// <summary>原文的 URL（若置空则无查看原文入口）。</summary>
    [JsonPropertyName("source_url")]
    public string? SourceUrl { get; set; }
}
