// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Living;

/// <summary>
/// 创建预约直播请求体（<c>/cgi-bin/living/create</c>；三种应用类型请求形态一致）。
/// </summary>
/// <remarks>
/// <para>官方限制：直播标题最多支持 20 个 utf8 字符；直播简介最多 100 个 utf8 字符，
/// 仅对通用直播、小班课、大班课、企业培训生效（活动直播简介通过 <see cref="ActivityDetail"/> 的简介控制）；
/// 大班课和小班课仅 k12 学校和 IT 行业类型能够发起。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Living")]
public class CreateLivingRequest
{
    /// <summary>获取或设置直播发起者的 userid（官方必填）。</summary>
    [JsonPropertyName("anchor_userid")]
    public string? AnchorUserid { get; set; }

    /// <summary>获取或设置直播的标题（官方必填；最多支持 20 个 utf8 字符）。</summary>
    [JsonPropertyName("theme")]
    public string? Theme { get; set; }

    /// <summary>获取或设置直播开始时间的 Unix 时间戳（官方必填；单位秒）。</summary>
    [JsonPropertyName("living_start")]
    public long? LivingStart { get; set; }

    /// <summary>获取或设置直播持续时长（官方必填；单位秒）。</summary>
    [JsonPropertyName("living_duration")]
    public int? LivingDuration { get; set; }

    /// <summary>
    /// 获取或设置直播类型：0 - 通用直播；1 - 小班课；2 - 大班课；3 - 企业培训；4 - 活动直播。默认 0。
    /// <para>官方限制：大班课和小班课仅 k12 学校和 IT 行业类型能够发起。</para>
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置直播简介（最多 100 个 utf8 字符）。
    /// <para>仅对通用直播、小班课、大班课、企业培训生效；活动直播简介通过活动直播详情的简介控制。</para>
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置授权方安装的应用 agentid（仅旧的第三方多应用套件需要填此参数）。</summary>
    [JsonPropertyName("agentid")]
    public int? Agentid { get; set; }

    /// <summary>获取或设置直播开始前多久提醒用户（相对 living_start 前的秒数），默认为 0。</summary>
    [JsonPropertyName("remind_time")]
    public int? RemindTime { get; set; }

    /// <summary>获取或设置活动直播特定参数：直播间封面图的 mediaId。</summary>
    [JsonPropertyName("activity_cover_mediaid")]
    public string? ActivityCoverMediaid { get; set; }

    /// <summary>获取或设置活动直播特定参数：直播分享卡片图的 mediaId。</summary>
    [JsonPropertyName("activity_share_mediaid")]
    public string? ActivityShareMediaid { get; set; }

    /// <summary>获取或设置活动直播特定参数：活动直播详情信息（详见 <see cref="LivingActivityDetail"/>）。</summary>
    [JsonPropertyName("activity_detail")]
    public LivingActivityDetail? ActivityDetail { get; set; }
}
