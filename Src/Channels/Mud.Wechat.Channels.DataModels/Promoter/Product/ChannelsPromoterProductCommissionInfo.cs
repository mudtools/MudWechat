// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>商品跟佣信息（官方 <c>product.commission_info</c> 对象，<c>promoter/get_promote_product_detail</c>）。</summary>
/// <remarks><b>官方文档矛盾照录</b>：<c>status</c> 参数表标注 string，但枚举表值与响应示例均为数字 ⇒ 按 int 建模。</remarks>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterProductCommissionInfo
{
    /// <summary>获取或设置商品带货状态（官方 <c>status</c>，见 <see cref="ChannelsPromoterProductPromotionStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置服务费率（官方 <c>service_ratio</c>，取值范围 [0, 1000000]）。</summary>
    [JsonPropertyName("service_ratio")]
    public long? ServiceRatio { get; set; }

    /// <summary>获取或设置合作开始时间（官方 <c>start_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置合作结束时间（官方 <c>end_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}
