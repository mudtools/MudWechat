// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.MiniStore;

/// <summary>礼物活动基础信息（官方 <c>basic_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreActivityBasicInfo
{
    /// <summary>获取或设置活动开始时间（官方 <c>activity_start_time</c>，单位为秒）。</summary>
    [JsonPropertyName("activity_start_time")]
    public long? ActivityStartTime { get; set; }

    /// <summary>获取或设置活动结束时间（官方 <c>activity_end_time</c>，单位为秒）。</summary>
    [JsonPropertyName("activity_end_time")]
    public long? ActivityEndTime { get; set; }

    /// <summary>获取或设置小店信息（官方 <c>shop_info</c>）。</summary>
    [JsonPropertyName("shop_info")]
    public ChannelsMiniStoreShopInfo? ShopInfo { get; set; }
}
