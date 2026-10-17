// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>优惠券额外信息（官方 <c>ext_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterCouponExtInfo
{
    /// <summary>获取或设置优惠券失效时间（官方 <c>valid_time</c>，官方参数表说明文字与 <c>invalid_time</c> 互换，照录）。</summary>
    [JsonPropertyName("valid_time")]
    public long? ValidTime { get; set; }

    /// <summary>获取或设置优惠券有效时间（官方 <c>invalid_time</c>，官方参数表说明文字与 <c>valid_time</c> 互换，照录）。</summary>
    [JsonPropertyName("invalid_time")]
    public long? InvalidTime { get; set; }
}
