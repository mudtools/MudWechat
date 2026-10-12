// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 售后管理（Aftersale）域「售后操作类」DTO（aftersale/acceptapply、aftersale/rejectapply、
// aftersale/acceptexchangereship、aftersale/rejectexchangereship、aftersale/merchantreship、
// aftersale/merchantupdatereshipexpress、aftersale/merchantupdateaftersale、aftersale/genaftersaleorder、
// aftersale/refundpricediff、aftersale/getexchangeableskulist、aftersale/applyvirtualtelnum、
// aftersale/handlefastexchangereceipt、aftersale/uploadrefundcertificate）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Aftersale。

namespace Mud.Wechat.Channels.DataModels.Aftersale;

/// <summary>拒绝售后（<c>aftersale/rejectapply</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsRejectAftersaleRequest
{
    /// <summary>获取或设置售后单号（官方 <c>after_sale_order_id</c>，必填）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public string AfterSaleOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置拒绝原因具体描述（官方 <c>reject_reason</c>，可使用默认描述或自定义）。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }

    /// <summary>获取或设置拒绝原因枚举值（官方 <c>reject_reason_type</c>，必填）。</summary>
    [JsonPropertyName("reject_reason_type")]
    public int RejectReasonType { get; set; }
}
