// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>打印成功通知 / 子件追加 共用的打印请求字段形态（<c>ewaybill/biz/order/print</c> / <c>addsuborder</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillPrintRequest
{
    /// <summary>获取或设置电子面单订单 id（官方 <c>ewaybill_order_id</c>，全局唯一 id，必填）。</summary>
    [JsonPropertyName("ewaybill_order_id")]
    public string EwaybillOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string WaybillId { get; set; } = string.Empty;

    /// <summary>获取或设置补打标记（官方 <c>re_print</c>，必填）：1 补打单成功；0 首次打单成功。</summary>
    [JsonPropertyName("re_print")]
    public int RePrint { get; set; }
}
