// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Ams;

/// <summary>
/// 「获取服务商模板广告单元列表」请求（<c>get_agency_ad_unit_list</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/GetAgencyTmplIdList.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Ams")]
public class OpenPlatformAmsAgencyAdUnitListRequest
{
    /// <summary>返回第几页数据（官方必填 <c>page</c>，number）。</summary>
    [JsonPropertyName("page")]
    public long Page { get; set; }

    /// <summary>当页返回数据条数（官方必填 <c>page_size</c>，number）。</summary>
    [JsonPropertyName("page_size")]
    public long PageSize { get; set; }

    /// <summary>广告位类型名称（官方必填 <c>ad_slot</c>）。</summary>
    [JsonPropertyName("ad_slot")]
    public string? AdSlot { get; set; }

    /// <summary>模板广告单元 ID（官方选填 <c>tmpl_id</c>，number）。</summary>
    [JsonPropertyName("tmpl_id")]
    public long? TmplId { get; set; }

    /// <summary>是否返回该模板绑定的商户广告单元信息（官方选填 <c>is_return_tmpl_bind_list</c>，number）。</summary>
    [JsonPropertyName("is_return_tmpl_bind_list")]
    public long? IsReturnTmplBindList { get; set; }
}
