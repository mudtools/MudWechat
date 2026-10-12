// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>订单扩展信息（官方 <c>ext_info</c> 对象，<c>dropship/get</c> 与 <c>supplyorder/get</c>）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：代发单与供货单两侧的 <c>ext_info</c> 参数表字段集完全一致
/// （<c>customer_notes</c>/<c>merchant_notes</c>/<c>present_notes</c>）⇒ 共用本 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsOrderExtInfo
{
    /// <summary>获取或设置用户备注（官方 <c>customer_notes</c>）。</summary>
    [JsonPropertyName("customer_notes")]
    public string? CustomerNotes { get; set; }

    /// <summary>获取或设置商家备注（官方 <c>merchant_notes</c>）。</summary>
    [JsonPropertyName("merchant_notes")]
    public string? MerchantNotes { get; set; }

    /// <summary>获取或设置礼物备注（官方 <c>present_notes</c>）。</summary>
    [JsonPropertyName("present_notes")]
    public string? PresentNotes { get; set; }
}
