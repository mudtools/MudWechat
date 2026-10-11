// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 「查询欠费」响应的明细项（官方 <c>items</c> 项；与统一下单请求体明细同构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxBillItem
{
    /// <summary>获取或设置明细序号（官方 <c>no</c>）。</summary>
    [JsonPropertyName("no")]
    public int No { get; set; }

    /// <summary>获取或设置收费项目 ID（官方 <c>item_id</c>）。</summary>
    [JsonPropertyName("item_id")]
    public string ItemId { get; set; } = string.Empty;

    /// <summary>获取或设置收费项目名称（官方 <c>item_name</c>）。</summary>
    [JsonPropertyName("item_name")]
    public string ItemName { get; set; } = string.Empty;

    /// <summary>获取或设置是否逾期（官方 <c>overdue</c>；0/1，可选）。</summary>
    [JsonPropertyName("overdue")]
    public int? Overdue { get; set; }

    /// <summary>获取或设置滞纳金（官方 <c>penalty</c>，单位分，可选）。</summary>
    [JsonPropertyName("penalty")]
    public int? Penalty { get; set; }

    /// <summary>获取或设置收费金额（官方 <c>fee</c>，单位分）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }
}

/// <summary>
/// 「查询欠费」请求（官方 <c>queryfee</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxQueryFeeRequest
{
    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>，可选）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置专项服务 ID（官方 <c>service_id</c>，可选）。</summary>
    [JsonPropertyName("service_id")]
    public int? ServiceId { get; set; }

    /// <summary>获取或设置银行 ID（官方 <c>bank_id</c>，可选）。</summary>
    [JsonPropertyName("bank_id")]
    public string? BankId { get; set; }

    /// <summary>获取或设置通知书类型（官方 <c>payment_notice_type</c>，可选）。</summary>
    [JsonPropertyName("payment_notice_type")]
    public int? PaymentNoticeType { get; set; }

    /// <summary>获取或设置通知书号（官方 <c>payment_notice_no</c>，必填）。</summary>
    [JsonPropertyName("payment_notice_no")]
    public string PaymentNoticeNo { get; set; } = string.Empty;

    /// <summary>获取或设置执收单位编码（官方 <c>department_code</c>，必填）。</summary>
    [JsonPropertyName("department_code")]
    public string DepartmentCode { get; set; } = string.Empty;

    /// <summary>获取或设置地区编码（官方 <c>region_code</c>，必填）。</summary>
    [JsonPropertyName("region_code")]
    public string RegionCode { get; set; } = string.Empty;
}

/// <summary>
/// 「查询欠费」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxQueryFeeResponse : MpResponse
{
    /// <summary>获取或设置应缴金额（官方 <c>fee</c>，单位分）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }

    /// <summary>获取或设置缴款人姓名（官方 <c>user_name</c>，可选）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>获取或设置执收单位编码（官方 <c>department_code</c>，可选）。</summary>
    [JsonPropertyName("department_code")]
    public string? DepartmentCode { get; set; }

    /// <summary>获取或设置执收单位名称（官方 <c>department_name</c>）。</summary>
    [JsonPropertyName("department_name")]
    public string? DepartmentName { get; set; }

    /// <summary>获取或设置通知书类型（官方 <c>payment_notice_type</c>，可选）。</summary>
    [JsonPropertyName("payment_notice_type")]
    public int? PaymentNoticeType { get; set; }

    /// <summary>获取或设置通知书号（官方 <c>payment_notice_no</c>）。</summary>
    [JsonPropertyName("payment_notice_no")]
    public string? PaymentNoticeNo { get; set; }

    /// <summary>获取或设置通知书生成时间（官方 <c>payment_notice_create_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("payment_notice_create_time")]
    public long PaymentNoticeCreateTime { get; set; }

    /// <summary>获取或设置缴款书过期日期（官方 <c>payment_expire_date</c>；yyyyMMdd 日期串的数值形态）。</summary>
    [JsonPropertyName("payment_expire_date")]
    public long PaymentExpireDate { get; set; }

    /// <summary>获取或设置地区编码（官方 <c>region_code</c>）。</summary>
    [JsonPropertyName("region_code")]
    public string? RegionCode { get; set; }

    /// <summary>获取或设置收费明细列表（官方 <c>items</c>）。</summary>
    [JsonPropertyName("items")]
    public MpNontaxBillItem[]? ItemList { get; set; }
}

/// <summary>
/// 非税支付订单明细项（官方 <c>items</c> 项；统一下单 / 微信支付各请求共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxOrderBillItem
{
    /// <summary>获取或设置明细序号（官方 <c>no</c>）。</summary>
    [JsonPropertyName("no")]
    public int No { get; set; }

    /// <summary>获取或设置收费项目 ID（官方 <c>item_id</c>）。</summary>
    [JsonPropertyName("item_id")]
    public string ItemId { get; set; } = string.Empty;

    /// <summary>获取或设置收费项目名称（官方 <c>item_name</c>）。</summary>
    [JsonPropertyName("item_name")]
    public string ItemName { get; set; } = string.Empty;

    /// <summary>获取或设置是否逾期（官方 <c>overdue</c>；0/1，可选）。</summary>
    [JsonPropertyName("overdue")]
    public int? Overdue { get; set; }

    /// <summary>获取或设置滞纳金（官方 <c>penalty</c>，单位分，可选）。</summary>
    [JsonPropertyName("penalty")]
    public int? Penalty { get; set; }

    /// <summary>获取或设置收费金额（官方 <c>fee</c>，单位分）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }
}

/// <summary>
/// 「统一下单」请求（官方 <c>unifiedorder</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxUnifiedOrderRequest
{
    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>，可选）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置专项服务 ID（官方 <c>service_id</c>，可选）。</summary>
    [JsonPropertyName("service_id")]
    public int? ServiceId { get; set; }

    /// <summary>获取或设置支付方式（官方 <c>trade_type</c>，可选）。</summary>
    [JsonPropertyName("trade_type")]
    public string? TradeType { get; set; }

    /// <summary>获取或设置银行 ID（官方 <c>bank_id</c>，可选）。</summary>
    [JsonPropertyName("bank_id")]
    public string? BankId { get; set; }

    /// <summary>获取或设置银行账户（官方 <c>bank_account</c>，可选；银行通道专用）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>获取或设置商户号（官方 <c>mch_id</c>，可选）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>获取或设置用户 OpenId（官方 <c>openid</c>，可选）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置小程序 <c>appid</c>（官方 <c>app_appid</c>，可选；小程序支付场景）。</summary>
    [JsonPropertyName("app_appid")]
    public string? AppAppId { get; set; }

    /// <summary>获取或设置商品描述（官方 <c>desc</c>，必填）。</summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; } = string.Empty;

    /// <summary>获取或设置订单总金额（官方 <c>fee</c>，单位分，必填）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }

    /// <summary>获取或设置支付完成跳转地址（官方 <c>return_url</c>，可选）。</summary>
    [JsonPropertyName("return_url")]
    public string? ReturnUrl { get; set; }

    /// <summary>获取或设置终端 IP（官方 <c>ip</c>，必填）。</summary>
    [JsonPropertyName("ip")]
    public string Ip { get; set; } = string.Empty;

    /// <summary>获取或设置商户订单号（官方 <c>order_no</c>，可选；不传由微信生成）。</summary>
    [JsonPropertyName("order_no")]
    public string? OrderNo { get; set; }

    /// <summary>获取或设置通知书类型（官方 <c>payment_notice_type</c>，可选）。</summary>
    [JsonPropertyName("payment_notice_type")]
    public int? PaymentNoticeType { get; set; }

    /// <summary>获取或设置通知书号（官方 <c>payment_notice_no</c>，可选）。</summary>
    [JsonPropertyName("payment_notice_no")]
    public string? PaymentNoticeNo { get; set; }

    /// <summary>获取或设置通知书生成时间（官方 <c>payment_notice_create_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("payment_notice_create_time")]
    public long PaymentNoticeCreateTime { get; set; }

    /// <summary>获取或设置缴款书过期日期（官方 <c>payment_expire_date</c>，yyyyMMdd）。</summary>
    [JsonPropertyName("payment_expire_date")]
    public string PaymentExpireDate { get; set; } = string.Empty;

    /// <summary>获取或设置缴款人姓名（官方 <c>user_name</c>，可选）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>获取或设置执收单位编码（官方 <c>department_code</c>，必填）。</summary>
    [JsonPropertyName("department_code")]
    public string DepartmentCode { get; set; } = string.Empty;

    /// <summary>获取或设置执收单位名称（官方 <c>department_name</c>，必填）。</summary>
    [JsonPropertyName("department_name")]
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>获取或设置地区编码（官方 <c>region_code</c>，必填）。</summary>
    [JsonPropertyName("region_code")]
    public string RegionCode { get; set; } = string.Empty;

    /// <summary>获取或设置场景（官方 <c>scene</c>，必填）。</summary>
    [JsonPropertyName("scene")]
    public string Scene { get; set; } = string.Empty;

    /// <summary>获取或设置订单明细列表（官方 <c>items</c>，必填）。</summary>
    [JsonPropertyName("items")]
    public List<MpNontaxOrderBillItem> ItemList { get; set; } = new();

    /// <summary>获取或设置是否自动调起支付（官方 <c>auto_call_pay</c>，可选）。</summary>
    [JsonPropertyName("auto_call_pay")]
    public bool? AutoCallPay { get; set; }
}

/// <summary>
/// 「统一下单」响应的小程序调起信息（官方 <c>miniprogram</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxMiniProgramLaunch
{
    /// <summary>获取或设置小程序 <c>appid</c>（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置小程序页面路径（官方 <c>pagepath</c>）。</summary>
    [JsonPropertyName("pagepath")]
    public string? PagePath { get; set; }

    /// <summary>获取或设置小程序查询字符串（官方 <c>query_string</c>）。</summary>
    [JsonPropertyName("query_string")]
    public string? QueryString { get; set; }

    /// <summary>获取或设置业务类型（官方 <c>business_type</c>）。</summary>
    [JsonPropertyName("business_type")]
    public string? BusinessType { get; set; }
}

/// <summary>
/// 「统一下单」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxUnifiedOrderResponse : MpResponse
{
    /// <summary>获取或设置非税订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置支付跳转地址（官方 <c>pay_url</c>，可选；H5 支付形态）。</summary>
    [JsonPropertyName("pay_url")]
    public string? PayUrl { get; set; }

    /// <summary>获取或设置小程序调起信息（官方 <c>miniprogram</c>；小程序支付形态）。</summary>
    [JsonPropertyName("miniprogram")]
    public MpNontaxMiniProgramLaunch? MiniProgram { get; set; }
}

/// <summary>
/// 「查询订单」请求（官方 <c>getorder</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxGetOrderRequest
{
    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>，可选）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置专项服务 ID（官方 <c>service_id</c>，可选）。</summary>
    [JsonPropertyName("service_id")]
    public int? ServiceId { get; set; }

    /// <summary>获取或设置非税订单号（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;
}

/// <summary>
/// 「查询订单」响应的部分退款记录（官方 <c>partial_refund_info</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxRefundRecord
{
    /// <summary>获取或设置商户退款单号（官方 <c>refund_out_id</c>）。</summary>
    [JsonPropertyName("refund_out_id")]
    public string? RefundOutId { get; set; }

    /// <summary>获取或设置微信退款单号（官方 <c>refund_order_id</c>）。</summary>
    [JsonPropertyName("refund_order_id")]
    public string? RefundOrderId { get; set; }

    /// <summary>获取或设置退款金额（官方 <c>refund_fee</c>，单位分）。</summary>
    [JsonPropertyName("refund_fee")]
    public int RefundFee { get; set; }

    /// <summary>获取或设置退款原因（官方 <c>refund_reason</c>）。</summary>
    [JsonPropertyName("refund_reason")]
    public string? RefundReason { get; set; }

    /// <summary>获取或设置退款状态（官方 <c>refund_status</c>）。</summary>
    [JsonPropertyName("refund_status")]
    public string? RefundStatus { get; set; }

    /// <summary>获取或设置退款完成时间（官方 <c>refund_finish_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("refund_finish_time")]
    public long? RefundFinishTime { get; set; }
}

/// <summary>
/// 「查询订单」响应的通知尝试记录（官方 <c>notify_history</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxNotifyHistory
{
    /// <summary>获取或设置通知的 <c>appid</c>（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置通知名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置通知尝试明细（官方 <c>notify_detail</c>）。</summary>
    [JsonPropertyName("notify_detail")]
    public MpNontaxNotifyAttempt[]? NotifyDetail { get; set; }

    /// <summary>获取或设置通知次数（官方 <c>notify_cnt</c>）。</summary>
    [JsonPropertyName("notify_cnt")]
    public int NotifyCount { get; set; }
}

/// <summary>
/// 「查询订单」响应的单次通知尝试（官方 <c>notify_detail</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxNotifyAttempt
{
    /// <summary>获取或设置错误码（官方 <c>errcode</c>）。</summary>
    [JsonPropertyName("errcode")]
    public int ErrorCode { get; set; }

    /// <summary>获取或设置错误信息（官方 <c>errmsg</c>，可选）。</summary>
    [JsonPropertyName("errmsg")]
    public string? ErrorMessage { get; set; }

    /// <summary>获取或设置返回码（官方 <c>ret</c>）。</summary>
    [JsonPropertyName("ret")]
    public int Ret { get; set; }

    /// <summary>获取或设置返回信息（官方 <c>ret_errmsg</c>，可选）。</summary>
    [JsonPropertyName("ret_errmsg")]
    public string? RetErrorMessage { get; set; }

    /// <summary>获取或设置通知地址（官方 <c>url</c>，可选）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置加密的通知报文（官方 <c>wxnontaxstr</c>）。</summary>
    [JsonPropertyName("wxnontaxstr")]
    public string? WxNontaxString { get; set; }

    /// <summary>获取或设置通知状态（官方 <c>status</c>）。</summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>获取或设置耗时（官方 <c>cost_time</c>，毫秒）。</summary>
    [JsonPropertyName("cost_time")]
    public int CostTime { get; set; }

    /// <summary>获取或设置通知时间（官方 <c>notify_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("notify_time")]
    public long NotifyTime { get; set; }

    /// <summary>获取或设置第三方响应（官方 <c>third_resp</c>，可选）。</summary>
    [JsonPropertyName("third_resp")]
    public string? ThirdResp { get; set; }

    /// <summary>获取或设置第三方响应数据（官方 <c>third_resp_data</c>，可选）。</summary>
    [JsonPropertyName("third_resp_data")]
    public string? ThirdRespData { get; set; }
}

/// <summary>
/// 「查询订单」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxGetOrderResponse : MpResponse
{
    /// <summary>获取或设置非税订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置用户 OpenId（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置商品描述（官方 <c>desc</c>）。</summary>
    [JsonPropertyName("desc")]
    public string? Description { get; set; }

    /// <summary>获取或设置支付方式（官方 <c>fee_type</c>）。</summary>
    [JsonPropertyName("fee_type")]
    public int FeeType { get; set; }

    /// <summary>获取或设置订单总金额（官方 <c>fee</c>，单位分）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }

    /// <summary>获取或设置订单状态（官方 <c>status</c>）。</summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>获取或设置微信支付订单号（官方 <c>trans_id</c>，可选）。</summary>
    [JsonPropertyName("trans_id")]
    public string? TransactionId { get; set; }

    /// <summary>获取或设置银行 ID（官方 <c>bank_id</c>，可选）。</summary>
    [JsonPropertyName("bank_id")]
    public string? BankId { get; set; }

    /// <summary>获取或设置银行名称（官方 <c>bank_name</c>，可选）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>获取或设置银行账户（官方 <c>bank_account</c>，可选）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>获取或设置商户退款单号（官方 <c>refund_out_id</c>，可选）。</summary>
    [JsonPropertyName("refund_out_id")]
    public string? RefundOutId { get; set; }

    /// <summary>获取或设置微信退款单号（官方 <c>refund_order_id</c>，可选）。</summary>
    [JsonPropertyName("refund_order_id")]
    public string? RefundOrderId { get; set; }

    /// <summary>获取或设置退款原因（官方 <c>refund_reason</c>，可选）。</summary>
    [JsonPropertyName("refund_reason")]
    public string? RefundReason { get; set; }

    /// <summary>获取或设置退款完成时间（官方 <c>refund_finish_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("refund_finish_time")]
    public long? RefundFinishTime { get; set; }

    /// <summary>获取或设置部分退款记录（官方 <c>partial_refund_info</c>，可选）。</summary>
    [JsonPropertyName("partial_refund_info")]
    public MpNontaxRefundRecord[]? PartialRefundInfo { get; set; }

    /// <summary>获取或设置商户订单号（官方 <c>order_no</c>，可选）。</summary>
    [JsonPropertyName("order_no")]
    public string? OrderNo { get; set; }

    /// <summary>获取或设置执收单位编码（官方 <c>department_code</c>，可选）。</summary>
    [JsonPropertyName("department_code")]
    public string? DepartmentCode { get; set; }

    /// <summary>获取或设置执收单位名称（官方 <c>department_name</c>）。</summary>
    [JsonPropertyName("department_name")]
    public string? DepartmentName { get; set; }

    /// <summary>获取或设置通知书类型（官方 <c>payment_notice_type</c>，可选）。</summary>
    [JsonPropertyName("payment_notice_type")]
    public int? PaymentNoticeType { get; set; }

    /// <summary>获取或设置通知书号（官方 <c>payment_notice_no</c>，可选）。</summary>
    [JsonPropertyName("payment_notice_no")]
    public string? PaymentNoticeNo { get; set; }

    /// <summary>获取或设置缴费信息来源（官方 <c>payment_info_source</c>）。</summary>
    [JsonPropertyName("payment_info_source")]
    public int PaymentInfoSource { get; set; }

    /// <summary>获取或设置票据类型编码（官方 <c>bill_type_code</c>，可选）。</summary>
    [JsonPropertyName("bill_type_code")]
    public string? BillTypeCode { get; set; }

    /// <summary>获取或设置票据号（官方 <c>bill_no</c>，可选）。</summary>
    [JsonPropertyName("bill_no")]
    public string? BillNo { get; set; }

    /// <summary>获取或设置地区编码（官方 <c>region_code</c>）。</summary>
    [JsonPropertyName("region_code")]
    public string? RegionCode { get; set; }

    /// <summary>获取或设置场景（官方 <c>scene</c>）。</summary>
    [JsonPropertyName("scene")]
    public string? Scene { get; set; }

    /// <summary>获取或设置订单明细列表（官方 <c>items</c>）。</summary>
    [JsonPropertyName("items")]
    public MpNontaxOrderBillItem[]? ItemList { get; set; }

    /// <summary>获取或设置通知历史（官方 <c>notify_history</c>）。</summary>
    [JsonPropertyName("notify_history")]
    public MpNontaxNotifyHistory[]? NotifyHistory { get; set; }

    /// <summary>获取或设置支付完成时间（官方 <c>pay_finish_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("pay_finish_time")]
    public long? PayFinishTime { get; set; }

    /// <summary>获取或设置下单时间（官方 <c>create_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }
}

/// <summary>
/// 「退款」请求（官方 <c>refund</c>）。
/// </summary>
/// <remarks>部分退款时 <see cref="RefundFee"/> 必填；全额退款可缺省。</remarks>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxRefundRequest
{
    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>，可选）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置非税订单号（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置退款原因（官方 <c>reason</c>，必填）。</summary>
    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>获取或设置退款金额（官方 <c>refund_fee</c>，单位分，可选；全额退款可缺省）。</summary>
    [JsonPropertyName("refund_fee")]
    public int? RefundFee { get; set; }

    /// <summary>获取或设置商户退款单号（官方 <c>refund_out_id</c>，可选；部分退款必填）。</summary>
    [JsonPropertyName("refund_out_id")]
    public string? RefundOutId { get; set; }
}

/// <summary>
/// 「退款」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxRefundResponse : MpResponse
{
    /// <summary>获取或设置微信退款单号（官方 <c>refund_order_id</c>）。</summary>
    [JsonPropertyName("refund_order_id")]
    public string? RefundOrderId { get; set; }
}

/// <summary>
/// 「微信支付（被扫）」请求（官方 <c>micropay</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxMicroPayRequest
{
    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>，可选）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置银行 ID（官方 <c>bank_id</c>，可选）。</summary>
    [JsonPropertyName("bank_id")]
    public string? BankId { get; set; }

    /// <summary>获取或设置银行账户（官方 <c>bank_account</c>，可选；银行通道专用）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>获取或设置商户号（官方 <c>mch_id</c>，可选）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>获取或设置商品描述（官方 <c>desc</c>，必填）。</summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; } = string.Empty;

    /// <summary>获取或设置订单总金额（官方 <c>fee</c>，单位分，必填）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }

    /// <summary>获取或设置商户订单号（官方 <c>order_no</c>，可选）。</summary>
    [JsonPropertyName("order_no")]
    public string? OrderNo { get; set; }

    /// <summary>获取或设置通知书类型（官方 <c>payment_notice_type</c>，可选）。</summary>
    [JsonPropertyName("payment_notice_type")]
    public int? PaymentNoticeType { get; set; }

    /// <summary>获取或设置通知书号（官方 <c>payment_notice_no</c>，可选）。</summary>
    [JsonPropertyName("payment_notice_no")]
    public string? PaymentNoticeNo { get; set; }

    /// <summary>获取或设置通知书生成时间（官方 <c>payment_notice_create_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("payment_notice_create_time")]
    public long PaymentNoticeCreateTime { get; set; }

    /// <summary>获取或设置缴款书过期日期（官方 <c>payment_expire_date</c>，yyyyMMdd）。</summary>
    [JsonPropertyName("payment_expire_date")]
    public string PaymentExpireDate { get; set; } = string.Empty;

    /// <summary>获取或设置缴款人姓名（官方 <c>user_name</c>，可选）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>获取或设置执收单位编码（官方 <c>department_code</c>，必填）。</summary>
    [JsonPropertyName("department_code")]
    public string DepartmentCode { get; set; } = string.Empty;

    /// <summary>获取或设置执收单位名称（官方 <c>department_name</c>，必填）。</summary>
    [JsonPropertyName("department_name")]
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>获取或设置地区编码（官方 <c>region_code</c>，必填）。</summary>
    [JsonPropertyName("region_code")]
    public string RegionCode { get; set; } = string.Empty;

    /// <summary>获取或设置扫码授权码（官方 <c>auth_code</c>，必填；用户付款码）。</summary>
    [JsonPropertyName("auth_code")]
    public string AuthCode { get; set; } = string.Empty;

    /// <summary>获取或设置订单明细列表（官方 <c>items</c>，必填）。</summary>
    [JsonPropertyName("items")]
    public List<MpNontaxOrderBillItem> ItemList { get; set; } = new();
}

/// <summary>
/// 「微信支付（被扫）」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxMicroPayResponse : MpResponse
{
    /// <summary>获取或设置非税订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

/// <summary>
/// 「查询订单列表」请求（官方 <c>getorderlist</c>；<c>order_no</c> 与 <c>payment_notice_no</c> 二选一）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxGetOrderListRequest
{
    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>，可选）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置商户订单号（官方 <c>order_no</c>，可选）。</summary>
    [JsonPropertyName("order_no")]
    public string? OrderNo { get; set; }

    /// <summary>获取或设置通知书号（官方 <c>payment_notice_no</c>，可选）。</summary>
    [JsonPropertyName("payment_notice_no")]
    public string? PaymentNoticeNo { get; set; }

    /// <summary>获取或设置执收单位编码（官方 <c>department_code</c>，必填）。</summary>
    [JsonPropertyName("department_code")]
    public string DepartmentCode { get; set; } = string.Empty;

    /// <summary>获取或设置地区编码（官方 <c>region_code</c>，必填）。</summary>
    [JsonPropertyName("region_code")]
    public string RegionCode { get; set; } = string.Empty;
}

/// <summary>
/// 「查询订单列表」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxGetOrderListResponse : MpResponse
{
    /// <summary>获取或设置非税订单号列表（官方 <c>order_id_list</c>）。</summary>
    [JsonPropertyName("order_id_list")]
    public string[]? OrderIdList { get; set; }

    /// <summary>获取或设置已支付订单号（官方 <c>paid_order_id</c>，可选）。</summary>
    [JsonPropertyName("paid_order_id")]
    public string? PaidOrderId { get; set; }
}

/// <summary>
/// 「通知不一致订单」请求（官方 <c>notifyinconsistentorder</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxNotifyInconsistentOrderRequest
{
    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>，可选）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置非税订单号（官方 <c>order_id</c>，可选）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

/// <summary>
/// 「模拟通知」请求（官方 <c>mocknotification</c>；<c>mockqueryfee</c> 复用同形请求）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxMockRequest
{
    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>，可选）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置回调通知地址（官方 <c>url</c>，必填）。</summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>获取或设置协议版本（官方 <c>version</c>；当前固定 1）。</summary>
    [JsonPropertyName("version")]
    public int Version { get; set; } = 1;
}

/// <summary>
/// 「获取实名信息」请求（官方 <c>getrealname</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxGetRealNameRequest
{
    /// <summary>获取或设置实名信息 token（官方 <c>wx_realname_token</c>，必填）。</summary>
    [JsonPropertyName("wx_realname_token")]
    public string WxRealNameToken { get; set; } = string.Empty;
}

/// <summary>
/// 「获取实名信息」响应（字段均为<strong>加密后</strong>数据与算法标识，解密由调用方执行）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxGetRealNameResponse : MpResponse
{
    /// <summary>获取或设置密钥加密算法（官方 <c>key_encrypt_type</c>）。</summary>
    [JsonPropertyName("key_encrypt_type")]
    public string? KeyEncryptType { get; set; }

    /// <summary>获取或设置加密后的密钥（官方 <c>key</c>）。</summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>获取或设置数据加密算法（官方 <c>data_encrypt_type</c>）。</summary>
    [JsonPropertyName("data_encrypt_type")]
    public string? DataEncryptType { get; set; }

    /// <summary>获取或设置加密后的实名数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public string? Data { get; set; }
}
