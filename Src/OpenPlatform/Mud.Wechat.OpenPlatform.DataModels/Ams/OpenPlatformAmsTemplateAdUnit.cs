// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Ams;

/// <summary>
/// 服务商模板广告单元（官方字段 <c>ad_unit_list</c> 内层，<c>get_agency_ad_unit_list</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/GetAgencyTmplIdList.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Ams")]
public class OpenPlatformAmsTemplateAdUnit
{
    /// <summary>模板 ID（官方字段名 <c>tmpl_id</c>）。</summary>
    [JsonPropertyName("tmpl_id")]
    public string? TmplId { get; set; }

    /// <summary>广告单元名称（官方字段名 <c>ad_unit_name</c>）。</summary>
    [JsonPropertyName("ad_unit_name")]
    public string? AdUnitName { get; set; }

    /// <summary>广告位 ID（官方字段名 <c>slot_id</c>）。</summary>
    [JsonPropertyName("slot_id")]
    public string? SlotId { get; set; }

    /// <summary>模板绑定总数（官方字段名 <c>tmpl_bind_total_num</c>，number）。</summary>
    [JsonPropertyName("tmpl_bind_total_num")]
    public long TmplBindTotalNum { get; set; }

    /// <summary>模板绑定的广告单元列表（官方字段名 <c>tmpl_bind_list</c>）。</summary>
    [JsonPropertyName("tmpl_bind_list")]
    public List<OpenPlatformAmsAdUnitRef>? TmplBindList { get; set; }
}
