// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>查询运单轨迹应答（<c>open_msg</c> 族 <c>query_follow_trace</c> 与 <c>query_trace</c> 同形复用）。</summary>
/// <remarks>
/// <para><c>waybill_info.status</c>：0 无轨迹（未揽收），1 快递已揽收，2 运输中，3 已签收，4 派件中，5 退回件，6 签收异常，7 拒收，8 揽件异常，9 变更配送地址，10 派件延迟，11 已退回，12 已寄出，13 转投。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressTraceResponse : WxaResponse
{
    /// <summary>运单信息（<c>waybill_info</c>），见 <see cref="WxaExpressTraceWaybill"/>。</summary>
    [JsonPropertyName("waybill_info")]
    public WxaExpressTraceWaybill? WaybillInfo { get; set; }

    /// <summary>店铺信息（<c>shop_info</c>），见 <see cref="WxaExpressTraceShop"/>。</summary>
    [JsonPropertyName("shop_info")]
    public WxaExpressTraceShop? ShopInfo { get; set; }

    /// <summary>快递信息（<c>delivery_info</c>），见 <see cref="WxaExpressTraceDelivery"/>。</summary>
    [JsonPropertyName("delivery_info")]
    public WxaExpressTraceDelivery? DeliveryInfo { get; set; }
}
