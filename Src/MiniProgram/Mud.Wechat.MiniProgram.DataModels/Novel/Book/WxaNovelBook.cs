// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Novel;

/// <summary>编辑作品请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelUpdateBookRequest
{
    /// <summary>作品 ID。</summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }

    /// <summary>作品名（1-30 字）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>作品简介（1-500 字）。</summary>
    [JsonPropertyName("intro")]
    public string? Intro { get; set; }

    /// <summary>封面图 media_id。</summary>
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

    /// <summary>按照预期顺序传入章节 ID。</summary>
    [JsonPropertyName("chapter_id_list")]
    public List<string>? ChapterIdList { get; set; }

    /// <summary>是否需要分卷。</summary>
    [JsonPropertyName("need_volume")]
    public bool? NeedVolume { get; set; }

    /// <summary>分卷信息。</summary>
    [JsonPropertyName("volume_list")]
    public List<WxaNovelVolumeInfo>? VolumeList { get; set; }

    /// <summary>章节排序方式：0 追加，1 seq 递增。</summary>
    [JsonPropertyName("chapter_order_method")]
    public long? ChapterOrderMethod { get; set; }

    /// <summary>自定义信息（0-128 字节）。</summary>
    [JsonPropertyName("custom_info")]
    public string? CustomInfo { get; set; }

    /// <summary>是否更新题材关键词。</summary>
    [JsonPropertyName("update_keyword")]
    public bool? UpdateKeyword { get; set; }

    /// <summary>题材关键词（最多 3 个，每个 1-4 字）。</summary>
    [JsonPropertyName("keyword_list")]
    public List<string>? KeywordList { get; set; }

    /// <summary>精彩片段，需为本书内容（400-1000 字）。</summary>
    [JsonPropertyName("awesome_paragraph")]
    public string? AwesomeParagraph { get; set; }
}

/// <summary>分卷信息。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelVolumeInfo
{
    /// <summary>分卷名（1-100 字）。</summary>
    [JsonPropertyName("volume_title")]
    public string? VolumeTitle { get; set; }

    /// <summary>分卷起始章节下标，取值范围 [0, 章节总数)。</summary>
    [JsonPropertyName("start_index")]
    public long? StartIndex { get; set; }

    /// <summary>分卷截止章节下标，取值范围 [起始章节下标, 章节总数)。</summary>
    [JsonPropertyName("end_index")]
    public long? EndIndex { get; set; }
}

/// <summary>删除作品请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelDeleteBookRequest
{
    /// <summary>作品 ID。</summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }
}

/// <summary>查询作品列表请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelListBookRequest
{
    /// <summary>分页拉取的最大返回结果数（1-100，默认 100）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>分页拉取的起始偏移量（默认 0）。offset 和 last_id 二选一，优先使用 last_id。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>分页 id。首次调用填 0，后续调用填上次返回的 last_id。offset 和 last_id 二选一，优先使用 last_id。</summary>
    [JsonPropertyName("last_id")]
    public long? LastId { get; set; }

    /// <summary>true 编辑版信息，false 发布版信息（默认 false）。</summary>
    [JsonPropertyName("need_edited_data")]
    public bool? NeedEditedData { get; set; }
}

/// <summary>查询作品列表响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelListBookResponse : WxaResponse
{
    /// <summary>作品信息列表。</summary>
    [JsonPropertyName("book_list")]
    public List<WxaNovelBookSummary>? BookList { get; set; }

    /// <summary>作品总数。</summary>
    [JsonPropertyName("total_cnt")]
    public long? TotalCnt { get; set; }

    /// <summary>分页 id。仅当请求参数有设置 last_id 且作品信息列表不为空时才有该字段。</summary>
    [JsonPropertyName("last_id")]
    public long? LastId { get; set; }
}

/// <summary>作品摘要信息（列表项）。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelBookSummary
{
    /// <summary>作品 ID。</summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }

    /// <summary>作品名。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>作品简介。</summary>
    [JsonPropertyName("intro")]
    public string? Intro { get; set; }

    /// <summary>封面图 URL。</summary>
    [JsonPropertyName("cover_url")]
    public string? CoverUrl { get; set; }

    /// <summary>作者名。</summary>
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

    /// <summary>上传场景：1 本地上传，2 API 上传。</summary>
    [JsonPropertyName("upload_scene")]
    public long? UploadScene { get; set; }

    /// <summary>章节数量。</summary>
    [JsonPropertyName("chapter_cnt")]
    public long? ChapterCnt { get; set; }

    /// <summary>分卷数量。</summary>
    [JsonPropertyName("volume_cnt")]
    public long? VolumeCnt { get; set; }

    /// <summary>作品总字数。</summary>
    [JsonPropertyName("total_word_cnt")]
    public long? TotalWordCnt { get; set; }

    /// <summary>审核信息（未发起审核不返回该字段）。</summary>
    [JsonPropertyName("audit_info")]
    public WxaNovelAuditInfo? AuditInfo { get; set; }

    /// <summary>创建时间戳。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>提供方作品主键。</summary>
    [JsonPropertyName("original_id")]
    public string? OriginalId { get; set; }

    /// <summary>章节排序方式：0 追加，1 seq 递增。</summary>
    [JsonPropertyName("chapter_order_method")]
    public long? ChapterOrderMethod { get; set; }

    /// <summary>自定义信息。</summary>
    [JsonPropertyName("custom_info")]
    public string? CustomInfo { get; set; }

    /// <summary>管控状态：0 正常，1 下架。</summary>
    [JsonPropertyName("ban_status")]
    public long? BanStatus { get; set; }
}

/// <summary>查询作品详情请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelGetBookRequest
{
    /// <summary>作品 ID，与提供方作品主键二选一，优先使用该字段。</summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }

    /// <summary>true 编辑版信息，false 发布版信息（默认 false）。</summary>
    [JsonPropertyName("need_edited_data")]
    public bool? NeedEditedData { get; set; }

    /// <summary>提供方作品主键，与作品 ID 二选一。</summary>
    [JsonPropertyName("original_id")]
    public string? OriginalId { get; set; }
}

/// <summary>查询作品详情响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelGetBookResponse : WxaResponse
{
    /// <summary>作品信息。</summary>
    [JsonPropertyName("book")]
    public WxaNovelBookDetail? Book { get; set; }
}

/// <summary>作品详细信息。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelBookDetail
{
    /// <summary>作品 ID。</summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }

    /// <summary>作品名。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>作品简介。</summary>
    [JsonPropertyName("intro")]
    public string? Intro { get; set; }

    /// <summary>封面图 URL。</summary>
    [JsonPropertyName("cover_url")]
    public string? CoverUrl { get; set; }

    /// <summary>作者名。</summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>一级类型 id。</summary>
    [JsonPropertyName("first_category_id")]
    public long? FirstCategoryId { get; set; }

    /// <summary>一级类型名。</summary>
    [JsonPropertyName("first_category_name")]
    public string? FirstCategoryName { get; set; }

    /// <summary>二级类型 id。</summary>
    [JsonPropertyName("second_category_id")]
    public long? SecondCategoryId { get; set; }

    /// <summary>二级类型名。</summary>
    [JsonPropertyName("second_category_name")]
    public string? SecondCategoryName { get; set; }

    /// <summary>三级类型 id。</summary>
    [JsonPropertyName("third_category_id")]
    public long? ThirdCategoryId { get; set; }

    /// <summary>三级类型名。</summary>
    [JsonPropertyName("third_category_name")]
    public string? ThirdCategoryName { get; set; }

    /// <summary>完结状态：1 连载中，2 已完结。</summary>
    [JsonPropertyName("complete_status")]
    public long? CompleteStatus { get; set; }

    /// <summary>上传场景：1 本地上传，2 API 上传。</summary>
    [JsonPropertyName("upload_scene")]
    public long? UploadScene { get; set; }

    /// <summary>章节数。</summary>
    [JsonPropertyName("chapter_cnt")]
    public long? ChapterCnt { get; set; }

    /// <summary>分卷数。</summary>
    [JsonPropertyName("volume_cnt")]
    public long? VolumeCnt { get; set; }

    /// <summary>分卷信息。</summary>
    [JsonPropertyName("volume_list")]
    public List<WxaNovelVolumeInfo>? VolumeList { get; set; }

    /// <summary>作品总字数。</summary>
    [JsonPropertyName("total_word_cnt")]
    public long? TotalWordCnt { get; set; }

    /// <summary>审核信息（未发起审核不返回该字段）。</summary>
    [JsonPropertyName("audit_info")]
    public WxaNovelAuditInfo? AuditInfo { get; set; }

    /// <summary>创建时间戳。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>提供方作品主键。</summary>
    [JsonPropertyName("original_id")]
    public string? OriginalId { get; set; }

    /// <summary>章节排序方式：0 追加，1 seq 递增。</summary>
    [JsonPropertyName("chapter_order_method")]
    public long? ChapterOrderMethod { get; set; }

    /// <summary>自定义信息。</summary>
    [JsonPropertyName("custom_info")]
    public string? CustomInfo { get; set; }

    /// <summary>管控状态：0 正常，1 下架。</summary>
    [JsonPropertyName("ban_status")]
    public long? BanStatus { get; set; }
}

/// <summary>审核信息。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelAuditInfo
{
    /// <summary>审核状态：0 未提审，1 审核中，2 审核不通过，3 审核通过。</summary>
    [JsonPropertyName("audit_status")]
    public long? AuditStatus { get; set; }

    /// <summary>提审时间戳。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>审核时间戳。</summary>
    [JsonPropertyName("audit_time")]
    public long? AuditTime { get; set; }

    /// <summary>审核原因。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>修改建议（审核不通过时才会有该字段）。</summary>
    [JsonPropertyName("suggestion")]
    public string? Suggestion { get; set; }
}

/// <summary>作品提审请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelAuditBookRequest
{
    /// <summary>作品 ID。</summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }
}
