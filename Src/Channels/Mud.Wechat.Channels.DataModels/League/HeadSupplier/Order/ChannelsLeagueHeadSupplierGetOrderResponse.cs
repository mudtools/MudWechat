// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>获取佣金单详情响应（官方 <c>headsupplier/order/get</c> 返回体）。</summary>
/// <remarks>
/// 官方根字段名拼写为 <c>commssion_order</c>（漏字母 i）⇒ JSON 名<b>照抄官方原文</b>，
/// C# 属性名按正确拼写 <c>CommissionOrder</c> 暴露，由守卫 CLG5 锁定该差异。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetOrderResponse
{
    /// <summary>获取或设置佣金单结构（官方 <c>commssion_order</c>，官方原文拼写漏字母 i）。</summary>
    [JsonPropertyName("commssion_order")]
    public ChannelsLeagueHeadSupplierCommissionOrder? CommissionOrder { get; set; }
}
