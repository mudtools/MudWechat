// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Novel;

/// <summary>上传章节请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelCreateChapterRequest
{
    /// <summary>作品 ID。</summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }

    /// <summary>章节信息。</summary>
    [JsonPropertyName("chapter")]
    public WxaNovelChapterInfo? Chapter { get; set; }
}

/// <summary>章节信息（上传/批量上传用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelChapterInfo
{
    /// <summary>章节标题（1-80 字）。</summary>
    [JsonPropertyName("chapter_title")]
    public string? ChapterTitle { get; set; }

    /// <summary>章节内容（1-20000 字）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>提供方章节主键，可用于去重（0-255 字节）。</summary>
    [JsonPropertyName("original_id")]
    public string? OriginalId { get; set; }

    /// <summary>章节相对顺序，可非连续递增（默认 0）。</summary>
    [JsonPropertyName("seq")]
    public long? Seq { get; set; }

    /// <summary>自定义信息（0-128 字节）。</summary>
    [JsonPropertyName("custom_info")]
    public string? CustomInfo { get; set; }
}

/// <summary>上传章节响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelCreateChapterResponse : WxaResponse
{
    /// <summary>章节 ID（非定长，不超过 64 字节）。</summary>
    [JsonPropertyName("chapter_id")]
    public string? ChapterId { get; set; }
}

/// <summary>批量上传章节请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelBatchCreateChapterRequest
{
    /// <summary>作品 ID。</summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }

    /// <summary>章节信息列表，单次最多上传 10 章。</summary>
    [JsonPropertyName("chapter_list")]
    public List<WxaNovelChapterInfo>? ChapterList { get; set; }
}

/// <summary>批量上传章节响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelBatchCreateChapterResponse : WxaResponse
{
    /// <summary>章节 ID 列表。</summary>
    [JsonPropertyName("chapter_id")]
    public List<string>? ChapterId { get; set; }

    /// <summary>冲突的提供方主键列表。</summary>
    [JsonPropertyName("conflict_original_id_list")]
    public List<string>? ConflictOriginalIdList { get; set; }
}

/// <summary>删除章节请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelDeleteChapterRequest
{
    /// <summary>作品 ID。</summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }

    /// <summary>章节 ID。</summary>
    [JsonPropertyName("chapter_id")]
    public string? ChapterId { get; set; }
}

/// <summary>替换章节请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelReplaceChapterRequest
{
    /// <summary>作品 ID。</summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }

    /// <summary>章节 ID。</summary>
    [JsonPropertyName("chapter_id")]
    public string? ChapterId { get; set; }

    /// <summary>新章节标题（1-80 字）。</summary>
    [JsonPropertyName("new_chapter_title")]
    public string? NewChapterTitle { get; set; }

    /// <summary>新章节内容（1-20000 字）。</summary>
    [JsonPropertyName("new_content")]
    public string? NewContent { get; set; }
}

/// <summary>替换章节响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelReplaceChapterResponse : WxaResponse
{
    /// <summary>替换后新章节 ID。</summary>
    [JsonPropertyName("new_chapter_id")]
    public string? NewChapterId { get; set; }
}

/// <summary>查询章节列表请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelListChapterRequest
{
    /// <summary>作品 ID。</summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }

    /// <summary>true 编辑版信息，false 发布版信息（默认 false）。</summary>
    [JsonPropertyName("need_edited_data")]
    public bool? NeedEditedData { get; set; }

    /// <summary>分页拉取的最大返回结果数（默认 10，最大值 100）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>分页拉取的起始偏移量（默认 0）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>分卷下标，若设置则只返回指定卷的章节列表。</summary>
    [JsonPropertyName("volume_index")]
    public long? VolumeIndex { get; set; }
}

/// <summary>查询章节列表响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelListChapterResponse : WxaResponse
{
    /// <summary>章节信息列表。</summary>
    [JsonPropertyName("chapter_list")]
    public List<WxaNovelChapterDetail>? ChapterList { get; set; }

    /// <summary>章节总数。</summary>
    [JsonPropertyName("total_cnt")]
    public long? TotalCnt { get; set; }
}

/// <summary>查询章节详情请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelGetChapterRequest
{
    /// <summary>作品 ID。</summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }

    /// <summary>章节 ID。</summary>
    [JsonPropertyName("chapter_id")]
    public string? ChapterId { get; set; }

    /// <summary>true 编辑版信息，false 发布版信息（默认 false）。</summary>
    [JsonPropertyName("need_edited_data")]
    public bool? NeedEditedData { get; set; }
}

/// <summary>查询章节详情响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelGetChapterResponse : WxaResponse
{
    /// <summary>章节信息。</summary>
    [JsonPropertyName("chapter")]
    public WxaNovelChapterDetail? Chapter { get; set; }
}

/// <summary>章节详细信息。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelChapterDetail
{
    /// <summary>章节 ID。</summary>
    [JsonPropertyName("chapter_id")]
    public string? ChapterId { get; set; }

    /// <summary>章节标题。</summary>
    [JsonPropertyName("chapter_title")]
    public string? ChapterTitle { get; set; }

    /// <summary>字数。</summary>
    [JsonPropertyName("word_cnt")]
    public long? WordCnt { get; set; }

    /// <summary>创建时间戳。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>审核信息。</summary>
    [JsonPropertyName("audit_info")]
    public WxaNovelAuditInfo? AuditInfo { get; set; }

    /// <summary>所属分卷下标，-1 表示不属于任何分卷。</summary>
    [JsonPropertyName("volume_index")]
    public long? VolumeIndex { get; set; }

    /// <summary>提供方章节主键。</summary>
    [JsonPropertyName("original_id")]
    public string? OriginalId { get; set; }

    /// <summary>章节相对顺序。</summary>
    [JsonPropertyName("seq")]
    public long? Seq { get; set; }

    /// <summary>自定义信息。</summary>
    [JsonPropertyName("custom_info")]
    public string? CustomInfo { get; set; }

    /// <summary>管控状态：0 正常，1 下架。</summary>
    [JsonPropertyName("ban_status")]
    public long? BanStatus { get; set; }
}

/// <summary>调整章节顺序请求（仅当章节排序方式为"追加"时生效）。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelReorderChapterRequest
{
    /// <summary>作品 ID。</summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }

    /// <summary>章节 ID。</summary>
    [JsonPropertyName("chapter_id")]
    public string? ChapterId { get; set; }

    /// <summary>目标章节 ID。</summary>
    [JsonPropertyName("target_chapter_id")]
    public string? TargetChapterId { get; set; }

    /// <summary>排序操作：1 交换，2 插入到目标章节之前，3 插入到目标章节之后。</summary>
    [JsonPropertyName("operation")]
    public long? Operation { get; set; }
}

/// <summary>调整章节相对顺序请求（仅当章节排序方式为"seq 递增"时生效）。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelUpdateChapterSeqRequest
{
    /// <summary>作品 ID。</summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }

    /// <summary>章节 seq 列表。</summary>
    [JsonPropertyName("chapter_seq_list")]
    public List<WxaNovelChapterSeqItem>? ChapterSeqList { get; set; }
}

/// <summary>章节 seq 项。</summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelChapterSeqItem
{
    /// <summary>章节 ID。</summary>
    [JsonPropertyName("chapter_id")]
    public string? ChapterId { get; set; }

    /// <summary>章节相对顺序，可非连续递增。</summary>
    [JsonPropertyName("seq")]
    public long? Seq { get; set; }
}
