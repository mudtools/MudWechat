// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 优惠券族（领取 / 创建 / 删除 / 过期 / 信息更新 / 作废 / 用户券过期 / 返还 / 核销）。
// 事件键：channels_ec_coupon_*、channels_ec_user_coupon_*。
// 注意：channels_ec_user_coupon_use 的官方参数表 Event 行误写为 unuse（复制粘贴错误），
// 以页面标题「优惠券核销通知」、头部事件英文名与示例三者一致值为准。
// 各事件 coupon_info/use_info 的时间字段名互不相同（create_time/delete_time/expire_time/change_time/
// invalid_time/unuse_time/use_time），JSON 形态各异，按官方逐事件建模、不共用嵌套类。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>创建优惠券事件主体（官方 <c>coupon_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsCouponCreateInfo
{
    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }
}
