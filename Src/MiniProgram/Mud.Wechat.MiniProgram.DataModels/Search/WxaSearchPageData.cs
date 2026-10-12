// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Search;

/// <summary>搜一搜数据推送的页面结构化数据（<c>data_list[]</c>）。</summary>
/// <remarks>字段以官方页面为准；SDK 不做本地校验。</remarks>
[HttpJsonSerializable(SerializerClassName = "Search")]
public class WxaSearchPageData
{
    /// <summary>数据结构类型（<c>@type</c>，必填；取值与结构说明以官方页面为准）。</summary>
    [JsonPropertyName("@type")]
    public string? Type { get; set; }

    /// <summary>更新类型（<c>update</c>，必填；取值以官方页面为准）。</summary>
    [JsonPropertyName("update")]
    public long? Update { get; set; }

    /// <summary>数据方自定义 ID（<c>content_id</c>）。</summary>
    [JsonPropertyName("content_id")]
    public string? ContentId { get; set; }

    /// <summary>页面类型（<c>page_type</c>，取值以官方页面为准）。</summary>
    [JsonPropertyName("page_type")]
    public long? PageType { get; set; }

    /// <summary>H5 链接（<c>h5_url</c>，仅 H5 类型数据使用）。</summary>
    [JsonPropertyName("h5_url")]
    public string? H5Url { get; set; }

    /// <summary>标题（<c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>摘要列表（<c>abstract</c>）。</summary>
    [JsonPropertyName("abstract")]
    public List<string>? AbstractList { get; set; }

    /// <summary>HTTP Referrer（<c>referer</c>，不会在搜索结果中展示）。</summary>
    [JsonPropertyName("referer")]
    public string? Referer { get; set; }

    /// <summary>封面图 URL（<c>cover_img_url</c>）。</summary>
    [JsonPropertyName("cover_img_url")]
    public string? CoverImageUrl { get; set; }

    /// <summary>正文（<c>mainbody</c>，纯文本）。</summary>
    [JsonPropertyName("mainbody")]
    public string? MainBody { get; set; }

    /// <summary>作者信息（<c>author</c>），见 <see cref="WxaSearchAuthor"/>。</summary>
    [JsonPropertyName("author")]
    public WxaSearchAuthor? Author { get; set; }

    /// <summary>视频列表（<c>video</c>），见 <see cref="WxaSearchVideo"/>。</summary>
    [JsonPropertyName("video")]
    public List<WxaSearchVideo>? VideoList { get; set; }

    /// <summary>发布时间戳（<c>time_publish</c>，Unix 秒级时间戳）。</summary>
    [JsonPropertyName("time_publish")]
    public long? PublishTimestamp { get; set; }

    /// <summary>更新时间戳（<c>time_modify</c>，Unix 秒级时间戳）。</summary>
    [JsonPropertyName("time_modify")]
    public long? ModifyTimestamp { get; set; }

    /// <summary>补充字段（<c>extra_info</c>，键值对随结构类型而异，透传）。</summary>
    [JsonPropertyName("extra_info")]
    public Dictionary<string, string>? ExtraInfo { get; set; }
}
