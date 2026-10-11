// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Images;

/// <summary>图片素材（官方 <c>images/get</c> 的 <c>data.list[]</c> 元素，2026-10-11 L3 核验，21 字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Images")]
public class AdsImageInfo
{
    /// <summary>图片 id（官方 <c>image_id</c>）。</summary>
    [JsonPropertyName("image_id")]
    public string? ImageId { get; set; }

    /// <summary>宽度（官方 <c>width</c>，<c>integer</c>，像素）。</summary>
    [JsonPropertyName("width")]
    public long? Width { get; set; }

    /// <summary>高度（官方 <c>height</c>，<c>integer</c>，像素）。</summary>
    [JsonPropertyName("height")]
    public long? Height { get; set; }

    /// <summary>文件大小（官方 <c>file_size</c>，<c>integer</c>，字节）。</summary>
    [JsonPropertyName("file_size")]
    public long? FileSize { get; set; }

    /// <summary>文件类型（官方 <c>type</c>，<c>enum</c>；可选值未逐项核验）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>签名（官方 <c>signature</c>）。</summary>
    [JsonPropertyName("signature")]
    public string? Signature { get; set; }

    /// <summary>描述（官方 <c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>源签名（官方 <c>source_signature</c>）。</summary>
    [JsonPropertyName("source_signature")]
    public string? SourceSignature { get; set; }

    /// <summary>预览地址（官方 <c>preview_url</c>）。</summary>
    [JsonPropertyName("preview_url")]
    public string? PreviewUrl { get; set; }

    /// <summary>来源类型（官方 <c>source_type</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("source_type")]
    public string? SourceType { get; set; }

    /// <summary>图片用途（官方 <c>image_usage</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("image_usage")]
    public string? ImageUsage { get; set; }

    /// <summary>创建时间戳（官方 <c>created_time</c>，秒级）。</summary>
    [JsonPropertyName("created_time")]
    public long? CreatedTime { get; set; }

    /// <summary>最后修改时间戳（官方 <c>last_modified_time</c>，秒级）。</summary>
    [JsonPropertyName("last_modified_time")]
    public long? LastModifiedTime { get; set; }

    /// <summary>商品目录 id（官方 <c>product_catalog_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("product_catalog_id")]
    public long? ProductCatalogId { get; set; }

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

    /// <summary>宽高比（官方 <c>sample_aspect_ratio</c>）。</summary>
    [JsonPropertyName("sample_aspect_ratio")]
    public string? SampleAspectRatio { get; set; }

    /// <summary>相似状态（官方 <c>similarity_status</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("similarity_status")]
    public string? SimilarityStatus { get; set; }

    /// <summary>AIGC 标记（官方 <c>aigc_flag</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("aigc_flag")]
    public string? AigcFlag { get; set; }
}

/// <summary><c>images/get</c> 的 <c>data</c> 载荷。</summary>
[HttpJsonSerializable(SerializerClassName = "Images")]
public class AdsImageListData
{
    /// <summary>图片列表（官方 <c>list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("list")]
    public List<AdsImageInfo>? List { get; set; }

    /// <summary>分页元信息（官方 <c>page_info</c>）。</summary>
    [JsonPropertyName("page_info")]
    public AdsPageInfo? PageInfo { get; set; }
}

/// <summary><c>GET /v3.0/images/get</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Images")]
public class AdsImageGetResponse : AdsResponse<AdsImageListData>
{
}

/// <summary><c>POST /v3.0/images/add</c> 的 <c>data</c> 载荷（multipart 上传应答，9 字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Images")]
public class AdsImageUploadResult
{
    /// <summary>图片 id（官方 <c>image_id</c>）。</summary>
    [JsonPropertyName("image_id")]
    public string? ImageId { get; set; }

    /// <summary>宽度（官方 <c>image_width</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("image_width")]
    public long? ImageWidth { get; set; }

    /// <summary>高度（官方 <c>image_height</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("image_height")]
    public long? ImageHeight { get; set; }

    /// <summary>文件大小（官方 <c>image_file_size</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("image_file_size")]
    public long? ImageFileSize { get; set; }

    /// <summary>文件类型（官方 <c>image_type</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("image_type")]
    public string? ImageType { get; set; }

    /// <summary>签名（官方 <c>image_signature</c>）。</summary>
    [JsonPropertyName("image_signature")]
    public string? ImageSignature { get; set; }

    /// <summary>外部图片 id（官方 <c>outer_image_id</c>）。</summary>
    [JsonPropertyName("outer_image_id")]
    public string? OuterImageId { get; set; }

    /// <summary>预览地址（官方 <c>preview_url</c>）。</summary>
    [JsonPropertyName("preview_url")]
    public string? PreviewUrl { get; set; }

    /// <summary>描述（官方 <c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary><c>POST /v3.0/images/add</c> 的闭合应答（multipart 通道，手写传输）。</summary>
[HttpJsonSerializable(SerializerClassName = "Images")]
public class AdsImageUploadResponse : AdsResponse<AdsImageUploadResult>
{
}

/// <summary>
/// <c>POST /v3.0/images/update</c> 的请求体（4 键；必填 <c>image_id</c> / <c>description</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Images")]
public class AdsImageUpdateRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>组织 id（官方 <c>organization_id</c>）。</summary>
    [JsonPropertyName("organization_id")]
    public long? OrganizationId { get; set; }

    /// <summary>图片 id（官方 <c>image_id</c>，必填）。</summary>
    [JsonPropertyName("image_id")]
    public string? ImageId { get; set; }

    /// <summary>描述（官方 <c>description</c>，必填 —— 官方把整支定义为「改描述」）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary><c>images/update|delete</c> 共用的 <c>data</c> 载荷（两支应答同构，均只有图片 id）。</summary>
[HttpJsonSerializable(SerializerClassName = "Images")]
public class AdsImageIdData
{
    /// <summary>图片 id（官方 <c>image_id</c>）。</summary>
    [JsonPropertyName("image_id")]
    public string? ImageId { get; set; }
}

/// <summary><c>POST /v3.0/images/update</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Images")]
public class AdsImageUpdateResponse : AdsResponse<AdsImageIdData>
{
}

/// <summary><c>POST /v3.0/images/delete</c> 的请求体（3 键）。</summary>
[HttpJsonSerializable(SerializerClassName = "Images")]
public class AdsImageDeleteRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>组织 id（官方 <c>organization_id</c>）。</summary>
    [JsonPropertyName("organization_id")]
    public long? OrganizationId { get; set; }

    /// <summary>图片 id（官方 <c>image_id</c>，必填）。</summary>
    [JsonPropertyName("image_id")]
    public string? ImageId { get; set; }
}

/// <summary><c>POST /v3.0/images/delete</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Images")]
public class AdsImageDeleteResponse : AdsResponse<AdsImageIdData>
{
}
