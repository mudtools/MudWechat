// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Ams;

/// <summary>
/// 「获取服务商模板广告单元列表」应答（<c>get_agency_ad_unit_list</c>，官方 <c>total_num</c> 为 string）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/GetAgencyTmplIdList.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Ams")]
public class OpenPlatformAmsAgencyAdUnitListResponse : OpenPlatformAmsResponse
{
    /// <summary>服务商总模板广告单元数据量（官方字段名 <c>total_num</c>，官方为 string）。</summary>
    [JsonPropertyName("total_num")]
    public string? TotalNum { get; set; }

    /// <summary>模板广告单元列表（官方字段名 <c>ad_unit_list</c>）。</summary>
    [JsonPropertyName("ad_unit_list")]
    public List<OpenPlatformAmsTemplateAdUnit>? AdUnitList { get; set; }
}
