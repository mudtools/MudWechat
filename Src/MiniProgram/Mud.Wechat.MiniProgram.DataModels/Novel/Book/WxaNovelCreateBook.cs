// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Novel;

/// <summary>创建作品请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelCreateBookRequest
{
    /// <summary>作品名（1-30 字）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>作品简介（1-500 字）。</summary>
    [JsonPropertyName("intro")]
    public string? Intro { get; set; }

    /// <summary>封面图 media_id（通过新增临时素材接口上传得到）。</summary>
    [JsonPropertyName("cover_media_id")]
    public string? CoverMediaId { get; set; }

    /// <summary>作者名（1-100 字）。</summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>一级类型 id。</summary>
    [JsonPropertyName("first_category_id")]
    public long? FirstCategoryId { get; set; }

    /// <summary>二级类型 id。</summary>
    [JsonPropertyName("second_category_id")]
    public long? SecondCategoryId { get; set; }

    /// <summary>三级类型 id。</summary>
    [JsonPropertyName("third_category_id")]
    public long? ThirdCategoryId { get; set; }

    /// <summary>完结状态：1 连载中，2 已完结。</summary>
    [JsonPropertyName("complete_status")]
    public long? CompleteStatus { get; set; }

    /// <summary>提供方作品主键，可用于去重（0-255 字节）。</summary>
    [JsonPropertyName("original_id")]
    public string? OriginalId { get; set; }

    /// <summary>章节排序方式：0 追加，1 seq 递增（默认 0）。</summary>
    [JsonPropertyName("chapter_order_method")]
    public long? ChapterOrderMethod { get; set; }

    /// <summary>自定义信息（0-128 字节）。</summary>
    [JsonPropertyName("custom_info")]
    public string? CustomInfo { get; set; }

    /// <summary>题材关键词（最多 3 个，每个 1-4 字）。</summary>
    [JsonPropertyName("keyword_list")]
    public List<string>? KeywordList { get; set; }

    /// <summary>精彩片段，需为本书内容（400-1000 字）。</summary>
    [JsonPropertyName("awesome_paragraph")]
    public string? AwesomeParagraph { get; set; }
}

/// <summary>创建作品响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelCreateBookResponse : WxaResponse
{
    /// <summary>作品 ID（非定长，不超过 64 字节）。</summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }
}
