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

/// <summary>商家协商（<c>aftersale/merchantupdateaftersale</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsMerchantUpdateAftersaleRequest
{
    /// <summary>获取或设置售后单号（官方 <c>after_sale_order_id</c>，必填）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public string AfterSaleOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置协商修改后的售后类型（官方 <c>type</c>，必填）：1 退款；2 退货退款；3 换货；4 补寄。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>获取或设置金额（官方 <c>amount</c>，单位分；换货 / 补寄时不填）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>获取或设置协商描述（官方 <c>merchant_update_desc</c>，必填）。</summary>
    [JsonPropertyName("merchant_update_desc")]
    public string MerchantUpdateDesc { get; set; } = string.Empty;

    /// <summary>获取或设置协商原因（官方 <c>update_reason_type</c>，必填，枚举值见官方文档）。</summary>
    [JsonPropertyName("update_reason_type")]
    public int UpdateReasonType { get; set; }

    /// <summary>
    /// 获取或设置协商类型（官方 <c>merchant_update_type</c>，必填）：1 已协商一致邀请买家取消售后；2 邀请买家核实与补充凭证；3 修改买家售后申请。
    /// </summary>
    [JsonPropertyName("merchant_update_type")]
    public int MerchantUpdateType { get; set; }

    /// <summary>
    /// 获取或设置协商凭证 id 列表（官方 <c>media_ids</c>，必填；当 <c>update_reason_type</c> 对应 need_image 为 1 时必填）。
    /// </summary>
    [JsonPropertyName("media_ids")]
    public List<string> MediaIds { get; set; } = new();

    /// <summary>获取或设置商家指定新换的 sku（官方 <c>new_sku_id</c>，type=EXCHANGE 时必填）。</summary>
    [JsonPropertyName("new_sku_id")]
    public long? NewSkuId { get; set; }

    /// <summary>获取或设置商家指定新换的数量（官方 <c>product_cnt</c>，type=EXCHANGE 时必填，协商换货场景必须填 1）。</summary>
    [JsonPropertyName("product_cnt")]
    public long? ProductCnt { get; set; }
}
