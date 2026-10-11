// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Videos;

/// <summary>视频素材（官方 <c>videos/get</c> 的 <c>data.list[]</c> 元素，2026-10-11 L3 核验，35 字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Videos")]
public class AdsVideoInfo
{
    /// <summary>视频 id（官方 <c>video_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("video_id")]
    public long? VideoId { get; set; }

    /// <summary>宽度（官方 <c>width</c>，<c>integer</c>，像素）。</summary>
    [JsonPropertyName("width")]
    public long? Width { get; set; }

    /// <summary>高度（官方 <c>height</c>，<c>integer</c>，像素）。</summary>
    [JsonPropertyName("height")]
    public long? Height { get; set; }

    /// <summary>总帧数（官方 <c>video_frames</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("video_frames")]
    public long? VideoFrames { get; set; }

    /// <summary>帧率（官方 <c>video_fps</c>，<c>float</c>）。</summary>
    [JsonPropertyName("video_fps")]
    public decimal? VideoFps { get; set; }

    /// <summary>视频编码（官方 <c>video_codec</c>）。</summary>
    [JsonPropertyName("video_codec")]
    public string? VideoCodec { get; set; }

    /// <summary>视频码率（官方 <c>video_bit_rate</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("video_bit_rate")]
    public long? VideoBitRate { get; set; }

    /// <summary>音频编码（官方 <c>audio_codec</c>）。</summary>
    [JsonPropertyName("audio_codec")]
    public string? AudioCodec { get; set; }

    /// <summary>音频码率（官方 <c>audio_bit_rate</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("audio_bit_rate")]
    public long? AudioBitRate { get; set; }

    /// <summary>文件大小（官方 <c>file_size</c>，<c>integer</c>，字节）。</summary>
    [JsonPropertyName("file_size")]
    public long? FileSize { get; set; }

    /// <summary>文件类型（官方 <c>type</c>，<c>enum</c>；可选值未逐项核验）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>签名（官方 <c>signature</c>）。</summary>
    [JsonPropertyName("signature")]
    public string? Signature { get; set; }

    /// <summary>系统状态（官方 <c>system_status</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("system_status")]
    public string? SystemStatus { get; set; }

    /// <summary>描述（官方 <c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>预览地址（官方 <c>preview_url</c>）。</summary>
    [JsonPropertyName("preview_url")]
    public string? PreviewUrl { get; set; }

    /// <summary>关键帧地址（官方 <c>key_frame_image_url</c>）。</summary>
    [JsonPropertyName("key_frame_image_url")]
    public string? KeyFrameImageUrl { get; set; }

    /// <summary>创建时间戳（官方 <c>created_time</c>，秒级）。</summary>
    [JsonPropertyName("created_time")]
    public long? CreatedTime { get; set; }

    /// <summary>最后修改时间戳（官方 <c>last_modified_time</c>，秒级）。</summary>
    [JsonPropertyName("last_modified_time")]
    public long? LastModifiedTime { get; set; }

    /// <summary>视频 Profile 名（官方 <c>video_profile_name</c>）。</summary>
    [JsonPropertyName("video_profile_name")]
    public string? VideoProfileName { get; set; }

    /// <summary>音频采样率（官方 <c>audio_sample_rate</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("audio_sample_rate")]
    public long? AudioSampleRate { get; set; }

    /// <summary>最大关键帧间隔（官方 <c>max_keyframe_interval</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("max_keyframe_interval")]
    public long? MaxKeyframeInterval { get; set; }

    /// <summary>最小关键帧间隔（官方 <c>min_keyframe_interval</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("min_keyframe_interval")]
    public long? MinKeyframeInterval { get; set; }

    /// <summary>宽高比（官方 <c>sample_aspect_ratio</c>）。</summary>
    [JsonPropertyName("sample_aspect_ratio")]
    public string? SampleAspectRatio { get; set; }

    /// <summary>音频 Profile 名（官方 <c>audio_profile_name</c>）。</summary>
    [JsonPropertyName("audio_profile_name")]
    public string? AudioProfileName { get; set; }

    /// <summary>扫描类型（官方 <c>scan_type</c>）。</summary>
    [JsonPropertyName("scan_type")]
    public string? ScanType { get; set; }

    /// <summary>图片时长毫秒（官方 <c>image_duration_millisecond</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("image_duration_millisecond")]
    public long? ImageDurationMillisecond { get; set; }

    /// <summary>音频时长毫秒（官方 <c>audio_duration_millisecond</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("audio_duration_millisecond")]
    public long? AudioDurationMillisecond { get; set; }

    /// <summary>来源类型（官方 <c>source_type</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("source_type")]
    public string? SourceType { get; set; }

    /// <summary>商品外部 id（官方 <c>product_outer_id</c>）。</summary>
    [JsonPropertyName("product_outer_id")]
    public string? ProductOuterId { get; set; }

    /// <summary>来源引用 id（官方 <c>source_reference_id</c>）。</summary>
    [JsonPropertyName("source_reference_id")]
    public string? SourceReferenceId { get; set; }

    /// <summary>归属账户 id（官方 <c>owner_account_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("owner_account_id")]
    public long? OwnerAccountId { get; set; }

    /// <summary>状态（官方 <c>status</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>相似状态（官方 <c>similarity_status</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("similarity_status")]
    public string? SimilarityStatus { get; set; }

    /// <summary>AIGC 标记（官方 <c>aigc_flag</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("aigc_flag")]
    public string? AigcFlag { get; set; }

    /// <summary>封面图 id（官方 <c>cover_id</c>）。</summary>
    [JsonPropertyName("cover_id")]
    public string? CoverId { get; set; }
}

/// <summary><c>videos/get</c> 的 <c>data</c> 载荷。</summary>
[HttpJsonSerializable(SerializerClassName = "Videos")]
public class AdsVideoListData
{
    /// <summary>视频列表（官方 <c>list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("list")]
    public List<AdsVideoInfo>? List { get; set; }

    /// <summary>分页元信息（官方 <c>page_info</c>）。</summary>
    [JsonPropertyName("page_info")]
    public AdsPageInfo? PageInfo { get; set; }
}

/// <summary><c>GET /v3.0/videos/get</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Videos")]
public class AdsVideoGetResponse : AdsResponse<AdsVideoListData>
{
}

/// <summary><c>POST /v3.0/videos/add</c> 的 <c>data</c> 载荷（multipart 上传应答，2 字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Videos")]
public class AdsVideoUploadResult
{
    /// <summary>视频 id（官方 <c>video_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("video_id")]
    public long? VideoId { get; set; }

    /// <summary>封面图 id（官方 <c>cover_image_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("cover_image_id")]
    public long? CoverImageId { get; set; }
}

/// <summary><c>POST /v3.0/videos/add</c> 的闭合应答（multipart 通道，手写传输）。</summary>
[HttpJsonSerializable(SerializerClassName = "Videos")]
public class AdsVideoUploadResponse : AdsResponse<AdsVideoUploadResult>
{
}

/// <summary>
/// <c>POST /v3.0/videos/update</c> 的请求体（4 键；必填 <c>video_id</c> / <c>description</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Videos")]
public class AdsVideoUpdateRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>组织 id（官方 <c>organization_id</c>）。</summary>
    [JsonPropertyName("organization_id")]
    public long? OrganizationId { get; set; }

    /// <summary>视频 id（官方 <c>video_id</c>，必填）。</summary>
    [JsonPropertyName("video_id")]
    public long? VideoId { get; set; }

    /// <summary>描述（官方 <c>description</c>，必填 —— 官方把整支定义为「改描述」）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary><c>videos/update|delete</c> 共用的 <c>data</c> 载荷（两支应答同构，均只有视频 id）。</summary>
[HttpJsonSerializable(SerializerClassName = "Videos")]
public class AdsVideoIdData
{
    /// <summary>视频 id（官方 <c>video_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("video_id")]
    public long? VideoId { get; set; }
}

/// <summary><c>POST /v3.0/videos/update</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Videos")]
public class AdsVideoUpdateResponse : AdsResponse<AdsVideoIdData>
{
}

/// <summary><c>POST /v3.0/videos/delete</c> 的请求体（3 键）。</summary>
[HttpJsonSerializable(SerializerClassName = "Videos")]
public class AdsVideoDeleteRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，必填）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>组织 id（官方 <c>organization_id</c>）。</summary>
    [JsonPropertyName("organization_id")]
    public long? OrganizationId { get; set; }

    /// <summary>视频 id（官方 <c>video_id</c>，必填）。</summary>
    [JsonPropertyName("video_id")]
    public long? VideoId { get; set; }
}

/// <summary><c>POST /v3.0/videos/delete</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Videos")]
public class AdsVideoDeleteResponse : AdsResponse<AdsVideoIdData>
{
}
