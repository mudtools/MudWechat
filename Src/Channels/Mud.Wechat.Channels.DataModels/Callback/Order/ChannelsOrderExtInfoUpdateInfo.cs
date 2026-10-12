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

/// <summary>订单其他信息更新事件主体（官方 <c>order_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsOrderExtInfoUpdateInfo
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>
    /// 获取或设置更新类型（官方 <c>type</c>）：1 联盟佣金信息；2 商家主动地址修改或通过用户修改地址申请；
    /// 3 商家备注修改；4 用户发起申请修改收货地址（特殊条件下需商家审批）；5 订单虚拟号码信息更新；
    /// 6 分享员信息更新；7 用户催发货；8 商家修改礼物单备注；9 用户发起发货前更换 sku 请求；
    /// 10 商家同意用户发货前更换 sku 请求；11 用户收礼时更换 sku；12 商家修改订单价格；13 真实号审核结果；
    /// 14 分配代发；15 取消代发；16 买家已同意修改，请按新地址发货；17 买家已拒绝修改，请按原地址发货；
    /// 18 买家超时未确认，修改申请已失效；19 买家已主动修改地址，请按新地址发货；20 该订单已发货，修改申请已失效；
    /// 21 用户同意发货协商申请；22 用户拒绝发货协商申请；23 发货协商超时；24 分配供货单；
    /// 25 用户预约具体发货时间；26 用户预约暂不发货；27 限制发货；28 取消限制发货。
    /// </summary>
    [JsonPropertyName("type")]
    public long? Type { get; set; }
}
