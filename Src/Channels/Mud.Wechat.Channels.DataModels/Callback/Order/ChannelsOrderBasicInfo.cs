// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 订单族（下单 / 取消 / 支付 / 待发货 / 发货 / 确认收货 / 结算 / 其他信息更新）。
// 事件键：channels_ec_order_*（注意：其他信息更新的 Event 值为 channels_ec_order_ext_info_update，
// 其官方页面文件名为 channels_ec_order_ext_info.html）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 订单事件基础主体（官方 <c>order_info</c>，仅 <c>order_id</c> 单字段）。
/// </summary>
/// <remarks>「订单下单」与「订单待发货」两事件的 <c>order_info</c> 字段集相同（仅 order_id），按工作单共用本类。</remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsOrderBasicInfo
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>；官方示例 3705115058471208928 超 int32，建模 long）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }
}
