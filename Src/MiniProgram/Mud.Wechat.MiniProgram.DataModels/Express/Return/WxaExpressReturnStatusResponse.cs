// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>
/// 退货 ID 状态应答（<c>status</c> / <c>waybill_id</c> / <c>order_status</c> / <c>delivery_name</c> / <c>delivery_id</c>，
/// <c>no_worry_return/get</c> 与 <c>return/get</c> 两端点同形复用）。
/// </summary>
/// <remarks>
/// <para><c>status</c>：0 用户未填写退货信息，1 预约上门取件，2 填写自行寄回运单号；<c>order_status</c> 为运单状态（0 已下单待揽件 … 99 未知）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressReturnStatusResponse : WxaResponse
{
    /// <summary>退货 ID 状态（<c>status</c>；0 未填写退货信息，1 预约上门取件，2 填写自行寄回运单号）。</summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>运单号（<c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>运单状态（<c>order_status</c>；0 已下单待揽件，1 已揽件，2 运输中，3 派件中，4 已签收，5 异常，6 代签收，7 揽收失败，8 签收失败，11 已取消，13 退件中，14 已退件，99 未知）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }

    /// <summary>运力公司编码（<c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>运力公司名称（<c>delivery_name</c>）。</summary>
    [JsonPropertyName("delivery_name")]
    public string? DeliveryName { get; set; }
}
