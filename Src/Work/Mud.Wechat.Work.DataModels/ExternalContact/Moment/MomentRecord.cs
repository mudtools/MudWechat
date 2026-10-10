// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Moment;

/// <summary>
/// 朋友圈发表记录中的图片附件（<c>moment_list[].image[]</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class MomentImageMedia
{
    /// <summary>
    /// 获取或设置图片的素材 id。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}

/// <summary>
/// 朋友圈发表记录中的视频附件（<c>moment_list[].video</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class MomentVideoMedia
{
    /// <summary>
    /// 获取或设置视频的素材 id。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>
    /// 获取或设置视频封面图的素材 id。
    /// </summary>
    [JsonPropertyName("thumb_media_id")]
    public string? ThumbMediaId { get; set; }
}

/// <summary>
/// 朋友圈发表记录中的图文（链接）附件（<c>moment_list[].link</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class MomentLinkMedia
{
    /// <summary>
    /// 获取或设置图文的标题。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置图文的链接 url。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// 朋友圈发表记录中的地理位置信息（<c>moment_list[].location</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class MomentLocation
{
    /// <summary>
    /// 获取或设置地理位置的纬度。
    /// </summary>
    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    /// <summary>
    /// 获取或设置地理位置的经度。
    /// </summary>
    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    /// <summary>
    /// 获取或设置地理位置的名称。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

/// <summary>
/// 朋友圈发表记录（<c>get_moment_list</c> 响应中 <c>moment_list[]</c> 的元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class MomentRecord
{
    /// <summary>
    /// 获取或设置朋友圈 id。
    /// </summary>
    [JsonPropertyName("moment_id")]
    public string? MomentId { get; set; }

    /// <summary>
    /// 获取或设置朋友圈创建人的 userid（企业创建的朋友圈不返回该字段）。
    /// </summary>
    [JsonPropertyName("creator")]
    public string? Creator { get; set; }

    /// <summary>
    /// 获取或设置朋友圈的创建时间（Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>
    /// 获取或设置朋友圈创建类型：0 - 企业创建，1 - 个人创建。
    /// </summary>
    [JsonPropertyName("create_type")]
    public int? CreateType { get; set; }

    /// <summary>
    /// 获取或设置朋友圈可见类型：0 - 部分可见，1 - 公开。
    /// </summary>
    [JsonPropertyName("visible_type")]
    public int? VisibleType { get; set; }

    /// <summary>
    /// 获取或设置朋友圈的文本内容。
    /// </summary>
    [JsonPropertyName("text")]
    public MomentText? Text { get; set; }

    /// <summary>
    /// 获取或设置朋友圈的图片附件列表。
    /// </summary>
    [JsonPropertyName("image")]
    public List<MomentImageMedia>? Image { get; set; }

    /// <summary>
    /// 获取或设置朋友圈的视频附件。
    /// </summary>
    [JsonPropertyName("video")]
    public MomentVideoMedia? Video { get; set; }

    /// <summary>
    /// 获取或设置朋友圈的图文（链接）附件。
    /// </summary>
    [JsonPropertyName("link")]
    public MomentLinkMedia? Link { get; set; }

    /// <summary>
    /// 获取或设置朋友圈的地理位置信息。
    /// </summary>
    [JsonPropertyName("location")]
    public MomentLocation? Location { get; set; }
}
