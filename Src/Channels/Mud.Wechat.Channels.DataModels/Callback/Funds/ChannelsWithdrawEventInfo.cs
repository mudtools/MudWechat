// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 资金族（结算账户变更 / 提现 / 提现二维码）。
// 事件键：channels_ec_acct_notify、channels_ec_withdraw_notify、qrcode_status。
// 注意：本族官方参数表与示例均无 FromUserName 字段（信封缺省，属性保留以兼容公共形态）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>提现事件主体（官方 <c>withdraw_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsWithdrawEventInfo
{
    /// <summary>获取或设置提现单号（官方 <c>withdraw_id</c>）。</summary>
    [JsonPropertyName("withdraw_id")]
    public string? WithdrawId { get; set; }

    /// <summary>获取或设置提现事件（官方 <c>event</c>）：1 发起提现，生成二维码；2 扫码验证成功，申请提现；3 提现成功；4 提现失败。</summary>
    [JsonPropertyName("event")]
    public long? Event { get; set; }
}
