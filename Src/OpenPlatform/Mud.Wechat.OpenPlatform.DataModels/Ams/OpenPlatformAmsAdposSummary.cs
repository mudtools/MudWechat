// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Ams;

/// <summary>
/// 广告位汇总数据的汇总项（官方 <c>summary</c> 数组内层，<c>agency_get_adpos_genenral</c>，无 slot/date 维度）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-data/GetAdposGenenral.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Ams")]
public class OpenPlatformAmsAdposSummary
{
    /// <summary>总拉取量（官方字段名 <c>req_succ_count</c>，number）。</summary>
    [JsonPropertyName("req_succ_count")]
    public long ReqSuccCount { get; set; }

    /// <summary>总曝光量（官方字段名 <c>exposure_count</c>，number）。</summary>
    [JsonPropertyName("exposure_count")]
    public long ExposureCount { get; set; }

    /// <summary>总曝光率（官方字段名 <c>exposure_rate</c>，number）。</summary>
    [JsonPropertyName("exposure_rate")]
    public double ExposureRate { get; set; }

    /// <summary>总点击量（官方字段名 <c>click_count</c>，number）。</summary>
    [JsonPropertyName("click_count")]
    public long ClickCount { get; set; }

    /// <summary>总点击率（官方字段名 <c>click_rate</c>，number）。</summary>
    [JsonPropertyName("click_rate")]
    public double ClickRate { get; set; }

    /// <summary>小程序分账收入合计（分，官方字段名 <c>publisher_income</c>，number）。</summary>
    [JsonPropertyName("publisher_income")]
    public long PublisherIncome { get; set; }

    /// <summary>广告千次曝光收益（分，官方字段名 <c>ecpm</c>，number）。</summary>
    [JsonPropertyName("ecpm")]
    public long Ecpm { get; set; }
}
