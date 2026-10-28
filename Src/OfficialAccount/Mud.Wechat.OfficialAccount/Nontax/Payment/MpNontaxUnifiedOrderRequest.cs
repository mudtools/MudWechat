// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

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
