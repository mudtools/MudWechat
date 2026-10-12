// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>商户提现（<c>submitwithdraw</c>）响应。</summary>
/// <remarks>
/// <para>官方契约：调<b>每天最多允许提现一次</b>；成功后将触发「提现回调」与「提现二维码回调」事件。</para>
/// <para>
/// <b>官方文档描述互换（照录）</b>：官方参数表把 <c>withdraw_id</c> 的说明写为「二维码ticket,可用于获取
/// 二维码和查询二维码状态」、把 <c>qrcode_ticket</c> 写为「提现单号」——径自矛盾。字段名照抄官方原文，
/// 语义以字面名为准（<c>withdraw_id</c> = 提现单号，<c>qrcode_ticket</c> = 二维码 ticket，供
/// <c>qrcode/get</c> / <c>qrcode/check</c> 使用）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsSubmitWithdrawResponse : ChannelsResponse
{
    /// <summary>获取或设置提现单号（官方 <c>withdraw_id</c>；官方说明原文照录为「二维码ticket」——字段语义以字面名为准）。</summary>
    [JsonPropertyName("withdraw_id")]
    public string? WithdrawId { get; set; }

    /// <summary>获取或设置二维码 ticket（官方 <c>qrcode_ticket</c>；官方说明原文照录为「提现单号」——字段语义以字面名为准）。</summary>
    [JsonPropertyName("qrcode_ticket")]
    public string? QrcodeTicket { get; set; }
}
