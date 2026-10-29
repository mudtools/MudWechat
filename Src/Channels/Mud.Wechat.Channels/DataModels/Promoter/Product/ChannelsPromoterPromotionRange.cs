// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>区间条件（官方 <c>min</c>/<c>max</c> 对象，<c>promoter/get_promote_product_list</c> 的 <c>spu_item_condition</c> 八个区间字段字段集一致 ⇒ 共用）。</summary>
/// <remarks>官方请求示例中 <c>commission_rate_range</c> 的 min/max 写成字符串（<c>"0"</c> / <c>"100000"</c>），参数表标注 number——按参数表建模。</remarks>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterPromotionRange
{
    /// <summary>获取或设置区间最小值（官方 <c>min</c>）。</summary>
    [JsonPropertyName("min")]
    public long? Min { get; set; }

    /// <summary>获取或设置区间最大值（官方 <c>max</c>）。</summary>
    [JsonPropertyName("max")]
    public long? Max { get; set; }
}
