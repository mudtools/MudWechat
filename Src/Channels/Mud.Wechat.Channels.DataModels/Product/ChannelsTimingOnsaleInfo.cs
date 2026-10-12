// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 商品待开售信息（官方 <c>timing_onsale_info</c> 对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsTimingOnsaleInfo
{
    /// <summary>获取或设置状态（官方 <c>status</c>：0 没有待开售，1 待开售）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置开售时间（官方 <c>onsale_time</c>，秒级时间戳；0 为未配置时间）。</summary>
    [JsonPropertyName("onsale_time")]
    public long? OnsaleTime { get; set; }

    /// <summary>获取或设置是否隐藏价格（官方 <c>is_hide_price</c>：0 不隐藏，1 隐藏）。</summary>
    [JsonPropertyName("is_hide_price")]
    public int? IsHidePrice { get; set; }
}
