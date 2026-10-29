// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopSharer;

/// <summary>
/// 直播间订单支付信息（<c>order_detail.pay_info</c>）。
/// </summary>
/// <remarks>
/// <para><b>表与示例不一致（留档）</b>：官方回包示例含 <c>pay_method</c>（如「微信支付」）但参数表未列该键；
/// 表内 <c>pay_method_type</c> 示例同样出现 ⇒ 两支都建模：<c>pay_method_type</c> 以表为准，
/// <c>pay_method</c> 以示例为准确认为在册键。</para>
/// <para><b>敏感面（个人金融信息，AGENTS §8）</b>：<c>transaction_id</c>（微信支付单号）与 <c>prepay_id</c>
/// 禁止写入日志、遥测或异常消息；宿主落库与截图同样受此约束。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerPayInfo
{
    /// <summary>支付方式名称回显（<c>pay_method</c>，string；官方示例含、参数表未列）。禁落日志。</summary>
    [JsonPropertyName("pay_method")]
    public string? PayMethod { get; set; }

    /// <summary>支付方式类型（<c>pay_method_type</c>，number）：<c>0</c> 微信支付、<c>1</c> 货到付款、<c>2</c> 商家会员储蓄卡（默认 0）。</summary>
    [JsonPropertyName("pay_method_type")]
    public int? PayMethodType { get; set; }

    /// <summary>预支付 ID（<c>prepay_id</c>，string）。支付凭据，禁落日志。</summary>
    [JsonPropertyName("prepay_id")]
    public string? PrepayId { get; set; }

    /// <summary>预付款时间（<c>prepay_time</c>，string，拿到 prepay_id 的时间）。</summary>
    [JsonPropertyName("prepay_time")]
    public string? PrepayTime { get; set; }

    /// <summary>支付 ID（<c>transaction_id</c>，string，微信支付单号）。支付凭据，禁落日志。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>付款时间（<c>pay_time</c>，string，拿到 transaction_id 的时间）。</summary>
    [JsonPropertyName("pay_time")]
    public string? PayTime { get; set; }
}
