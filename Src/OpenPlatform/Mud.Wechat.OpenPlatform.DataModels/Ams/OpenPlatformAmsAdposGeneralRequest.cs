// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Ams;

/// <summary>
/// 「获取广告位汇总数据」请求（<c>agency_get_adpos_genenral</c>，官方拼写为 <c>genenral</c> 非 <c>general</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-data/GetAdposGenenral.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Ams")]
public class OpenPlatformAmsAdposGeneralRequest
{
    /// <summary>返回第几页数据（官方必填 <c>page</c>，number）。</summary>
    [JsonPropertyName("page")]
    public long Page { get; set; }

    /// <summary>当页返回数据条数（官方必填 <c>page_size</c>，number）。</summary>
    [JsonPropertyName("page_size")]
    public long PageSize { get; set; }

    /// <summary>数据开始时间（官方必填 <c>start_date</c>，yyyy-mm-dd）。</summary>
    [JsonPropertyName("start_date")]
    public string? StartDate { get; set; }

    /// <summary>数据结束时间（官方必填 <c>end_date</c>，yyyy-mm-dd）。</summary>
    [JsonPropertyName("end_date")]
    public string? EndDate { get; set; }

    /// <summary>广告位类型名称（官方选填 <c>ad_slot</c>；缺省返回全部类型）。</summary>
    [JsonPropertyName("ad_slot")]
    public string? AdSlot { get; set; }
}
