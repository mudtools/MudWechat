// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 「分广告位数据」回包的 <c>summary</c> 聚合（无日期/广告位维度，仅各项总量）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpPublisherAdSlotSummary
{
    /// <summary>总拉取量（官方 <c>req_succ_count</c>）。</summary>
    [JsonPropertyName("req_succ_count")]
    public long ReqSuccCount { get; set; }

    /// <summary>总曝光量（官方 <c>exposure_count</c>）。</summary>
    [JsonPropertyName("exposure_count")]
    public long ExposureCount { get; set; }

    /// <summary>总曝光率（官方 <c>exposure_rate</c>）。</summary>
    [JsonPropertyName("exposure_rate")]
    public double ExposureRate { get; set; }

    /// <summary>总点击量（官方 <c>click_count</c>）。</summary>
    [JsonPropertyName("click_count")]
    public long ClickCount { get; set; }

    /// <summary>总点击率（官方 <c>click_rate</c>）。</summary>
    [JsonPropertyName("click_rate")]
    public double ClickRate { get; set; }

    /// <summary>总收入(分)（官方 <c>income</c>）。</summary>
    [JsonPropertyName("income")]
    public long Income { get; set; }

    /// <summary>广告千次曝光收益(分)（官方 <c>ecpm</c>）。</summary>
    [JsonPropertyName("ecpm")]
    public double Ecpm { get; set; }
}
