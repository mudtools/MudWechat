// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>电商数据概要（官方 <c>live_ec_data_summary</c> 对象）。</summary>
/// <remarks>金额字段单位均为<b>分</b>；未列出的指标为未定义（官方原文）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveEcDataSummary
{
    /// <summary>获取或设置成交金额（官方 <c>total_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("total_gmv")]
    public long? TotalGmv { get; set; }

    /// <summary>获取或设置成交次数（官方 <c>total_pay_pv</c>）。</summary>
    [JsonPropertyName("total_pay_pv")]
    public long? TotalPayPv { get; set; }

    /// <summary>获取或设置成交人数（官方 <c>total_pay_uv</c>）。</summary>
    [JsonPropertyName("total_pay_uv")]
    public long? TotalPayUv { get; set; }

    /// <summary>获取或设置订单创建次数（官方 <c>total_create_pv</c>）。</summary>
    [JsonPropertyName("total_create_pv")]
    public long? TotalCreatePv { get; set; }

    /// <summary>获取或设置订单创建人数（官方 <c>total_create_uv</c>）。</summary>
    [JsonPropertyName("total_create_uv")]
    public long? TotalCreateUv { get; set; }

    /// <summary>获取或设置退款金额（官方 <c>refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("refund_amount")]
    public long? RefundAmount { get; set; }
}
