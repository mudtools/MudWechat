// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 剧目信息（官方 <c>Res.drama_info</c>，<c>getdrama</c> 与 <c>listdramas</c> 共用同一字段面）。
/// </summary>
/// <remarks>
/// <para><b>返回的图片链接均为临时链接</b>（官方原文「不应将其保存下来」）。</para>
/// <para><b>驳回理由不在本页</b>（官方原文）：剧目审核失败或驳回时，每一集的具体驳回理由与证据截图须走「获取媒资列表」/「获取媒资详细信息」。</para>
/// <para><b>本类型不含提审侧字段</b>（如 <c>media_id_list</c>、<c>qualification_*</c>）⇒ 不要拿它回填提审请求。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_getdrama.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodDramaInfo
{
    /// <summary>drama_id（官方类型 <c>number</c>）：剧目id。</summary>
    [JsonPropertyName("drama_id")]
    public long? DramaId { get; set; }

    /// <summary>create_time（官方类型 <c>number</c>）：创建时间，时间戳。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>name（官方类型 <c>string</c>）：剧名。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>cover_url（官方类型 <c>string</c>）：剧目海报链接，根据提审时提交的cover_material_id转存得到。</summary>
    [JsonPropertyName("cover_url")]
    public string? CoverUrl { get; set; }

    /// <summary>media_count（官方类型 <c>number</c>）：剧集数目。</summary>
    [JsonPropertyName("media_count")]
    public long? MediaCount { get; set; }

    /// <summary>producer（官方类型 <c>string</c>）：制作方 。</summary>
    [JsonPropertyName("producer")]
    public string? Producer { get; set; }

    /// <summary>playwright（官方类型 <c>string</c>）：编剧。</summary>
    [JsonPropertyName("playwright")]
    public string? Playwright { get; set; }

    /// <summary>description（官方类型 <c>string</c>）：剧目简介。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>production_license（官方类型 <c>string</c>）：广播电视节目制作经营许可证。</summary>
    [JsonPropertyName("production_license")]
    public string? ProductionLicense { get; set; }

    /// <summary>audit_detail（官方类型 <c>object</c>）：审核状态。</summary>
    [JsonPropertyName("audit_detail")]
    public WxaVodDramaAuditDetail? AuditDetail { get; set; }

    /// <summary>media_list（官方类型 <c>objarray</c>）：剧集信息列表。</summary>
    [JsonPropertyName("media_list")]
    public List<WxaVodDramaMediaItem>? MediaList { get; set; }

    /// <summary>expedited（官方类型 <c>number</c>）：1表示审核加急，0或空为非加急审核</summary>
    [JsonPropertyName("expedited")]
    public long? Expedited { get; set; }

    /// <summary>recommendations（官方类型 <c>string</c>）：剧目推荐语。</summary>
    [JsonPropertyName("recommendations")]
    public string? Recommendations { get; set; }

    /// <summary>promotion_poster（官方类型 <c>string</c>）：推广海报。</summary>
    [JsonPropertyName("promotion_poster")]
    public string? PromotionPoster { get; set; }

    /// <summary>actor_list（官方类型 <c>object</c>）：演员信息。</summary>
    [JsonPropertyName("actor_list")]
    public WxaVodActorList? ActorList { get; set; }

    /// <summary>status（官方类型 <c>number</c>）：剧目状态。0为正常可播；1为审核中；2为审核失败；3为平台下架</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }
}
