// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Ams;

/// <summary>
/// 「按日期区间取结算数据」请求（page + page_size + start_date + end_date）——
/// <c>agency_get_settlement</c>（operationams）与 <c>get_agency_settled_revenue</c>（getdefaultamsinfo）
/// 官方请求体逐字相同 ⇒ 共用本 DTO（令牌面不同：前者 authorizer、后者 component）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Ams")]
public class OpenPlatformAmsRevenuePageRequest
{
    /// <summary>数据返回页数（官方必填 <c>page</c>，number）。</summary>
    [JsonPropertyName("page")]
    public long Page { get; set; }

    /// <summary>每页返回数据条数（官方必填 <c>page_size</c>，number）。</summary>
    [JsonPropertyName("page_size")]
    public long PageSize { get; set; }

    /// <summary>数据开始时间（官方必填 <c>start_date</c>，yyyy-mm-dd）。</summary>
    [JsonPropertyName("start_date")]
    public string? StartDate { get; set; }

    /// <summary>数据结束时间（官方必填 <c>end_date</c>，yyyy-mm-dd）。</summary>
    [JsonPropertyName("end_date")]
    public string? EndDate { get; set; }
}
