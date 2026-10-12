// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>电商概览数据（官方 <c>data</c> 对象，<c>getfinderoverall</c>，官方类型 number）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassFinderOverallData
{
    /// <summary>获取或设置成交金额（官方 <c>pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_gmv")]
    public long? PayGmv { get; set; }

    /// <summary>获取或设置直播成交金额（官方 <c>live_pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("live_pay_gmv")]
    public long? LivePayGmv { get; set; }

    /// <summary>获取或设置短视频成交金额（官方 <c>feed_pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("feed_pay_gmv")]
    public long? FeedPayGmv { get; set; }

    /// <summary>获取或设置橱窗成交金额（官方 <c>window_pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("window_pay_gmv")]
    public long? WindowPayGmv { get; set; }

    /// <summary>获取或设置商品分享支付金额（官方 <c>product_pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("product_pay_gmv")]
    public long? ProductPayGmv { get; set; }

    /// <summary>获取或设置其他渠道成交金额（官方 <c>other_pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("other_pay_gmv")]
    public long? OtherPayGmv { get; set; }
}
