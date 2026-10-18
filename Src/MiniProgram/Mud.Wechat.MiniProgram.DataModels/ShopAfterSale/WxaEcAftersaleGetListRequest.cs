// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 获取售后列表请求体（<c>POST /shop/ecaftersale/get_list</c>）。
/// </summary>
/// <remarks>
/// <para><b>时间单位是毫秒</b>：<c>begin_create_time</c> / <c>end_create_time</c> 官方标「单位毫秒」，</para>
/// <para>与纠纷单列表的秒级时间戳不同 ⇒ 两域不得套用同一换算。</para>
/// <para><b>翻页</b>：<c>offset</c> 从 0 开始、<c>limit</c> 最大 50，二者均必填。</para>
/// <para><b>敏感值</b>：<c>openid</c> 禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaEcAftersaleGetListRequest
{
    /// <summary>售后单状态（选填），见 <see cref="WxaEcAftersaleStates"/>。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>关联外部订单号（选填）。</summary>
    [JsonPropertyName("out_order_id")]
    public string? OutOrderId { get; set; }

    /// <summary>关联微信侧订单号（选填）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>买家 <c>openid</c>（选填，敏感）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>申请时间起始，单位毫秒（选填）。</summary>
    [JsonPropertyName("begin_create_time")]
    public long? BeginCreateTime { get; set; }

    /// <summary>申请时间结束，单位毫秒（选填）。</summary>
    [JsonPropertyName("end_create_time")]
    public long? EndCreateTime { get; set; }

    /// <summary>本次拉取起始位置，从 0 开始（必填）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>本次拉取大小，最大 50（必填）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }
}
