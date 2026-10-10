// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Pay;

/// <summary>
/// 获取对外收款记录响应体（<c>/cgi-bin/externalpay/get_bill_list</c>）。
/// <para>官方业务限制：无 next_cursor 返回时表示数据已全部拉取完。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class GetPayBillListResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置分页游标；无更多数据时不返回该字段。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>
    /// 获取或设置交易单详情列表。
    /// </summary>
    [JsonPropertyName("bill_list")]
    public List<PayBillItem>? BillList { get; set; }
}

/// <summary>
/// 对外收款记录交易单详情（<see cref="GetPayBillListResponse.BillList"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayBillItem
{
    /// <summary>
    /// 获取或设置交易单号。
    /// </summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>
    /// 获取或设置交易类型：0 - 收款记录、1 - 退款记录。
    /// </summary>
    [JsonPropertyName("bill_type")]
    public int? BillType { get; set; }

    /// <summary>
    /// 获取或设置交易状态（退款记录不返回）：1 - 已完成、3 - 已完成有退款。
    /// </summary>
    [JsonPropertyName("trade_state")]
    public int? TradeState { get; set; }

    /// <summary>
    /// 获取或设置交易时间（Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("pay_time")]
    public long? PayTime { get; set; }

    /// <summary>
    /// 获取或设置商户单号（退款记录返回对应收款记录的商户单号）。
    /// </summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>
    /// 获取或设置退款单号（退款记录返回该字段）。
    /// </summary>
    [JsonPropertyName("out_refund_no")]
    public string? OutRefundNo { get; set; }

    /// <summary>
    /// 获取或设置付款人的 userid。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserId { get; set; }

    /// <summary>
    /// 获取或设置收款总金额（单位分）。
    /// </summary>
    [JsonPropertyName("total_fee")]
    public long? TotalFee { get; set; }

    /// <summary>
    /// 获取或设置收款记录为收款成员 userid；退款记录为退款成员 userid。
    /// </summary>
    [JsonPropertyName("payee_userid")]
    public string? PayeeUserId { get; set; }

    /// <summary>
    /// 获取或设置收款方式：0 - 聊天中收款、1 - 收款码、2 - 直播间、3 - 产品图册、
    /// 14 - 转账、15 - 小程序（部分灰度企业）。
    /// </summary>
    [JsonPropertyName("payment_type")]
    public int? PaymentType { get; set; }

    /// <summary>
    /// 获取或设置收款商户号 ID。
    /// </summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>
    /// 获取或设置备注（退款记录为退款备注）。
    /// </summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>
    /// 获取或设置商品信息详情（退款记录不返回）。
    /// </summary>
    [JsonPropertyName("commodity_list")]
    public List<PayBillCommodity>? CommodityList { get; set; }

    /// <summary>
    /// 获取或设置退款总金额（单位分）。
    /// </summary>
    [JsonPropertyName("total_refund_fee")]
    public long? TotalRefundFee { get; set; }

    /// <summary>
    /// 获取或设置退款单据列表（退款记录不返回）。
    /// </summary>
    [JsonPropertyName("refund_list")]
    public List<PayBillRefund>? RefundList { get; set; }

    /// <summary>
    /// 获取或设置联系人信息（第三方应用不可获取；退款记录不返回）。
    /// </summary>
    [JsonPropertyName("contact_info")]
    public PayBillContact? ContactInfo { get; set; }

    /// <summary>
    /// 获取或设置小程序信息（收款方式为小程序时返回）。
    /// </summary>
    [JsonPropertyName("miniprogram_info")]
    public PayBillMiniprogramInfo? MiniprogramInfo { get; set; }
}

/// <summary>
/// 对外收款商品信息（<see cref="PayBillItem.CommodityList"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayBillCommodity
{
    /// <summary>
    /// 获取或设置商品描述。
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 获取或设置商品数量。
    /// </summary>
    [JsonPropertyName("amount")]
    public int? Amount { get; set; }
}

/// <summary>
/// 对外收款退款单据（<see cref="PayBillItem.RefundList"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayBillRefund
{
    /// <summary>
    /// 获取或设置退款单号。
    /// </summary>
    [JsonPropertyName("out_refund_no")]
    public string? OutRefundNo { get; set; }

    /// <summary>
    /// 获取或设置退款发起人 userid。
    /// </summary>
    [JsonPropertyName("refund_userid")]
    public string? RefundUserId { get; set; }

    /// <summary>
    /// 获取或设置退款备注。
    /// </summary>
    [JsonPropertyName("refund_comment")]
    public string? RefundComment { get; set; }

    /// <summary>
    /// 获取或设置退款发起时间（Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("refund_reqtime")]
    public long? RefundReqtime { get; set; }

    /// <summary>
    /// 获取或设置退款状态：0 - 已申请退款、1 - 退款处理中、2 - 退款成功、3 - 退款关闭、
    /// 4 - 退款异常、5 - 审批中、6 - 审批失败、7 - 审批取消。
    /// </summary>
    [JsonPropertyName("refund_status")]
    public int? RefundStatus { get; set; }

    /// <summary>
    /// 获取或设置退款金额（单位分）。
    /// </summary>
    [JsonPropertyName("refund_fee")]
    public long? RefundFee { get; set; }
}

/// <summary>
/// 对外收款联系人信息（<see cref="PayBillItem.ContactInfo"/>；
/// 第三方应用不可获取）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayBillContact
{
    /// <summary>
    /// 获取或设置联系人姓名。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置联系人电话。
    /// </summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>
    /// 获取或设置联系人地址。
    /// </summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }
}

/// <summary>
/// 对外收款小程序信息（<see cref="PayBillItem.MiniprogramInfo"/>，
/// 收款方式为小程序时返回）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayBillMiniprogramInfo
{
    /// <summary>
    /// 获取或设置小程序 appid。
    /// </summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>
    /// 获取或设置小程序名称。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
