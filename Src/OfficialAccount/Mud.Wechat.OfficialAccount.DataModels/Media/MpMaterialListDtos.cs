// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Media;

/// <summary>
/// 获取永久素材总数（<c>material/get_materialcount</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<b>GET</b>、无请求体；路径为 <c>get_materialcount</c>
/// （<c>count</c> 前无下划线，照抄官方）。成功响应不含 <c>errcode</c>（缺省 0 视为成功）。
/// </para>
/// <para>
/// <b>数量上限（官方「注意事项」原文）</b>：「图片和图文消息素材（包括单图文和多图文）的总数上限为
/// <b>100000</b>，其他素材的总数上限为 <b>1000</b>」；「永久素材的总数包含公众平台官网素材管理中的素材」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpGetMaterialCountResponse : MpResponse
{
    /// <summary>获取或设置语音总数量（官方 <c>voice_count</c>；上限 1000）。</summary>
    [JsonPropertyName("voice_count")]
    public int VoiceCount { get; set; }

    /// <summary>获取或设置视频总数量（官方 <c>video_count</c>；上限 1000）。</summary>
    [JsonPropertyName("video_count")]
    public int VideoCount { get; set; }

    /// <summary>获取或设置图片总数量（官方 <c>image_count</c>；上限 100000）。</summary>
    [JsonPropertyName("image_count")]
    public int ImageCount { get; set; }

    /// <summary>获取或设置图文总数量（官方 <c>news_count</c>；上限 100000）。</summary>
    [JsonPropertyName("news_count")]
    public int NewsCount { get; set; }
}

/// <summary>获取永久素材列表（<c>material/batchget_material</c>）请求体。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：<b>POST</b>，字段表逐字段核验——<c>type</c>（image/video/voice/news）、
/// <c>offset</c>（从全部素材的该偏移位置开始返回，0 表示从第一个素材返回）、
/// <c>count</c>（返回素材的数量，取值在 <b>1 到 20 之间</b>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpBatchGetMaterialRequest
{
    /// <summary>获取或设置素材的类型（官方 <c>type</c>：<c>MpMediaTypes</c> 四类）。</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>获取或设置偏移位置（官方 <c>offset</c>，0 表示从第一个素材返回）。</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>获取或设置返回数量（官方 <c>count</c>，取值 1~20）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }
}

/// <summary>获取永久素材列表（<c>material/batchget_material</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpBatchGetMaterialResponse : MpResponse
{
    /// <summary>获取或设置该类型的素材总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    /// <summary>获取或设置本次调用获取的素材数量（官方 <c>item_count</c>）。</summary>
    [JsonPropertyName("item_count")]
    public int ItemCount { get; set; }

    /// <summary>获取或设置素材条目列表（官方 <c>item</c>，元素形态按 type 分流——见 <see cref="MpMaterialListItem"/>）。</summary>
    [JsonPropertyName("item")]
    public List<MpMaterialListItem>? Items { get; set; }
}

/// <summary>
/// 永久素材列表条目（官方 <c>item</c> 元素；news 与 image/voice/video 两形态的字段超集 DTO）。
/// </summary>
/// <remarks>
/// <para>
/// <b>两形态（共用裁决，官方同字段表声明）</b>：news 类型条目为
/// <c>{media_id, content.news_item[], update_time}</c>；image/voice/video 类型条目为
/// <c>{media_id, name, update_time, url}</c>。字段集不同 ⇒ 超集建模（未携带者为 null），
/// 由守卫锁定而非拆两个响应 DTO（同响应 <c>item</c> 数组的元素类型必须唯一）。
/// </para>
/// <para>官方「注意事项」：「临时素材无法通过本接口获取」。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpMaterialListItem
{
    /// <summary>获取或设置素材 media_id（官方 <c>media_id</c>）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long UpdateTime { get; set; }

    /// <summary>获取或设置素材名称（官方 <c>name</c>；<b>仅 image/voice/video 形态返回</b>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置素材 URL（官方 <c>url</c>；<b>仅 image/voice/video 形态返回</b>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置图文内容容器（官方 <c>content</c>；<b>仅 news 形态返回</b>）。</summary>
    [JsonPropertyName("content")]
    public MpMaterialNewsContent? Content { get; set; }
}

/// <summary>图文素材内容容器（官方 <c>content.news_item</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpMaterialNewsContent
{
    /// <summary>获取或设置图文消息内的 1 篇或多篇文章（官方 <c>news_item</c>）。</summary>
    [JsonPropertyName("news_item")]
    public List<MpMaterialNewsItem>? NewsItems { get; set; }
}

/// <summary>
/// 按 media_id 寻址永久素材的请求体（<c>material/get_material</c> 与 <c>material/del_material</c> 共用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用裁决（N4：字段集一致才共用，由守卫双向锁定）</b>：两页请求体字段表<b>均为单个
/// <c>media_id</c>（必填）</b>，形态完全一致 ⇒ 共用同一 DTO（两份声明会漂移）。
/// </para>
/// <para>
/// 官方「注意事项」（删除页，逐页核验 2026-10-07）：「请谨慎操作本接口，可以删除官网素材管理模块中的
/// 图文/语音/视频等素材（需先通过获取素材列表获取 media_id）」；「临时素材无法通过本接口删除」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpMediaIdRequest
{
    /// <summary>获取或设置目标素材的 media_id（官方 <c>media_id</c>，必填）。</summary>
    [JsonPropertyName("media_id")]
    public string MediaId { get; set; } = string.Empty;
}

/// <summary>
/// 上传发表内容中的图片（<c>media/uploadimg</c>，官方现名「上传发表内容中的图片」）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<b>POST multipart</b>（表单字段 <c>media</c>）；
/// 图片仅支持 jpg/png，大小必须在 <b>1MB 以下</b>；返回 <c>url</c>（<b>非 media_id</b>，
/// 无 media_id 生命周期），且「不占用公众号的素材库中图片数量的 100000 个的限制」。
/// </para>
/// <para>
/// <b>归属说明</b>：官方索引页把本端点同时列于「群发消息」与「永久素材」分组 ⇒ 只建模一次
/// （落素材域；群发域 XML 交叉引用，不再建第二份声明）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpUploadImageResponse : MpResponse
{
    /// <summary>获取或设置图片 URL（官方 <c>url</c>；用于图文正文，非 media_id 形态）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
