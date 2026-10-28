// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 春节不打烊信息（官方 <c>item.product_info.product_spring_festival_delivery_config</c>，2026-02-04 新增）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierSpringFestivalDeliveryConfig
{
    /// <summary>获取或设置春节可发货时段列表（官方 <c>delivery_time_list</c>；为空则春节不发货）。</summary>
    [JsonPropertyName("delivery_time_list")]
    public List<ChannelsLeagueHeadSupplierTimeRange>? DeliveryTimeList { get; set; }

    /// <summary>获取或设置春节打烊期间延迟发货的时间（官方 <c>predict_delivery_time</c>，时间戳）。</summary>
    [JsonPropertyName("predict_delivery_time")]
    public long? PredictDeliveryTime { get; set; }

    /// <summary>获取或设置发货时效（官方 <c>send_time_hours</c>，单位：小时；仅春节可发货时段有效）。</summary>
    [JsonPropertyName("send_time_hours")]
    public int? SendTimeHours { get; set; }

    /// <summary>获取或设置是否不受春节影响（官方 <c>unaffected_by_newyear</c>，boolean；如预售、虚拟、定制商品）。</summary>
    [JsonPropertyName("unaffected_by_newyear")]
    public bool? UnaffectedByNewYear { get; set; }
}
