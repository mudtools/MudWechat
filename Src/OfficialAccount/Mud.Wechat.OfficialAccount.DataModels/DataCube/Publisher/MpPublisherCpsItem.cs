// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 「返佣商品数据」(<c>publisher_cps_general</c>) 的 <c>list</c> 元素（按日期粒度，无广告位维度）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpPublisherCpsItem
{
    /// <summary>日期（官方 <c>date</c>，格式 yyyy-MM-dd）。</summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>曝光量（官方 <c>exposure_count</c>）。</summary>
    [JsonPropertyName("exposure_count")]
    public long ExposureCount { get; set; }

    /// <summary>点击量（官方 <c>click_count</c>）。</summary>
    [JsonPropertyName("click_count")]
    public long ClickCount { get; set; }

    /// <summary>点击率（官方 <c>click_rate</c>）。</summary>
    [JsonPropertyName("click_rate")]
    public double ClickRate { get; set; }

    /// <summary>订单量（官方 <c>order_count</c>）。</summary>
    [JsonPropertyName("order_count")]
    public long OrderCount { get; set; }

    /// <summary>下单率（官方 <c>order_rate</c>）。</summary>
    [JsonPropertyName("order_rate")]
    public double OrderRate { get; set; }

    /// <summary>订单金额(分)（官方 <c>total_fee</c>）。</summary>
    [JsonPropertyName("total_fee")]
    public long TotalFee { get; set; }

    /// <summary>预估收入(分)（官方 <c>total_commission</c>）。</summary>
    [JsonPropertyName("total_commission")]
    public long TotalCommission { get; set; }
}
