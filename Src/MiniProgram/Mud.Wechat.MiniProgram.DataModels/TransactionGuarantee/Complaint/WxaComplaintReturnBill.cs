// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 退货单信息（<c>returnBill</c>，官方「查询投诉单详情」参数表）。
/// </summary>
/// <remarks>
/// <para>
/// <b>仅在退货流程出现</b>：官方把 <c>returnBill</c> 建为可选对象，投诉单未进入「平台已判定为商责，
/// 待用户退货中」（<c>status</c> 308 / 309）时该字段缺省。
/// </para>
/// <para>
/// <b><see cref="ReturnId"/> 是 <c>bussiSupplyRefund</c> 的入参来源（官方原文）</b>：
/// 「退货单号，需要通过查询投诉单详情接口获取」⇒ 商家提交退款凭证前必须先查本详情拿到 <c>returnId</c>，
/// 不得自行生成。
/// </para>
/// <para>
/// <b>类型口径以参数表为准</b>：官方本页 <c>returnId</c> 在「返回体」参数表为 <c>string</c>，
/// 而「商家提交退款凭证」请求体表把同名 <c>returnId</c> 标为 <c>number</c>。两页口径不一致 ⇒
/// 出参侧照本页 <c>string</c>、入参侧照其页 <c>long?</c>，调用方须原样回传、不要做数值转换。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaComplaintReturnBill
{
    /// <summary>退货 id（<c>returnId</c>，官方类型 string；<c>bussiSupplyRefund</c> 的必填定位键）。</summary>
    [JsonPropertyName("returnId")]
    public string? ReturnId { get; set; }

    /// <summary>运单号（<c>waybillId</c>）。</summary>
    [JsonPropertyName("waybillId")]
    public string? WaybillId { get; set; }

    /// <summary>运力公司（<c>deliveryName</c>）。</summary>
    [JsonPropertyName("deliveryName")]
    public string? DeliveryName { get; set; }

    /// <summary>
    /// 运单状态（<c>orderStatus</c>；官方枚举：<c>0</c> 已下单待揽件 / <c>1</c> 已揽件 / <c>2</c> 运输中 /
    /// <c>3</c> 派件中 / <c>4</c> 已签收 / <c>5</c> 异常 / <c>6</c> 代签收 / <c>7</c> 揽收失败 /
    /// <c>8</c> 签收失败 / <c>10</c> 未下单 / <c>11</c> 已取消 / <c>12</c> 已删除 / <c>13</c> 退件中 /
    /// <c>14</c> 已退件 / <c>15</c> 运力方取消 / <c>99</c> 未确认状态）。
    /// <b>注意官方枚举无 <c>9</c></b>，且 <c>99</c> 为「未确认状态」兜底值。
    /// </summary>
    [JsonPropertyName("orderStatus")]
    public long? OrderStatus { get; set; }
}
