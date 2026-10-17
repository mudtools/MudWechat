// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Ams;

/// <summary>
/// 「更新广告单元」请求（<c>agency_update_adunit</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/AgencyUpdateAdunit.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Ams")]
public class OpenPlatformAmsAdUnitUpdateRequest
{
    /// <summary>广告单元名称（官方必填 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>广告单元 ID（官方必填 <c>ad_unit_id</c>；创建端点无此必填项，是 update 与 create 的请求体差异）。</summary>
    [JsonPropertyName("ad_unit_id")]
    public string? AdUnitId { get; set; }

    /// <summary>广告单元开关状态（官方必填 <c>status</c>，<b>字符串</b>枚举：<c>AD_UNIT_STATUS_ON</c> / <c>AD_UNIT_STATUS_OFF</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>原生模板样式（官方选填 <c>tmpl_type</c>，number）。</summary>
    [JsonPropertyName("tmpl_type")]
    public long? TmplType { get; set; }

    /// <summary>自定义模板广告位 ID（官方选填 <c>tmpl_id</c>，number）。</summary>
    [JsonPropertyName("tmpl_id")]
    public long? TmplId { get; set; }

    /// <summary>激励视频解锁时长（官方选填 <c>unlock_reward_duration</c>，number，可选 15/30）。</summary>
    [JsonPropertyName("unlock_reward_duration")]
    public long? UnlockRewardDuration { get; set; }
}
