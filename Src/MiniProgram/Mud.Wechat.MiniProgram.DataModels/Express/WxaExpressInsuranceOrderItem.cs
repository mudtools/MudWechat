// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>保单项（<c>list</c> 元素，<c>getorderlist</c> 响应）。</summary>
/// <remarks>
/// 保单状态 <c>status</c>：2 保障中、4 理赔中、5 理赔成功、6 理赔失败、7 投保过期；
/// <c>is_home_pick_up</c>：<c>0</c> 否、<c>1</c> 是。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressInsuranceOrderItem
{
    /// <summary>微信支付单号（<c>order_no</c>）。</summary>
    [JsonPropertyName("order_no")]
    public string? OrderNo { get; set; }

    /// <summary>保单号（<c>policy_no</c>）。</summary>
    [JsonPropertyName("policy_no")]
    public string? PolicyNo { get; set; }

    /// <summary>理赔报案号（<c>report_no</c>）。</summary>
    [JsonPropertyName("report_no")]
    public string? ReportNo { get; set; }

    /// <summary>发货运单号（<c>delivery_no</c>）。</summary>
    [JsonPropertyName("delivery_no")]
    public string? DeliveryNo { get; set; }

    /// <summary>退款运单号（<c>refund_delivery_no</c>）。</summary>
    [JsonPropertyName("refund_delivery_no")]
    public string? RefundDeliveryNo { get; set; }

    /// <summary>保费（<c>premium</c>；单位：分）。</summary>
    [JsonPropertyName("premium")]
    public long Premium { get; set; }

    /// <summary>预估理赔金额（<c>estimate_amount</c>；单位：分）。</summary>
    [JsonPropertyName("estimate_amount")]
    public long EstimateAmount { get; set; }

    /// <summary>保单状态（<c>status</c>；枚举见类说明）。</summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>理赔打款失败原因（<c>pay_fail_reason</c>）。</summary>
    [JsonPropertyName("pay_fail_reason")]
    public string? PayFailReason { get; set; }

    /// <summary>理赔款打给用户的时间（<c>pay_finish_time</c>；秒级时间戳）。</summary>
    [JsonPropertyName("pay_finish_time")]
    public long PayFinishTime { get; set; }

    /// <summary>是否上门取件（<c>is_home_pick_up</c>；<c>0</c> 否，<c>1</c> 是）。</summary>
    [JsonPropertyName("is_home_pick_up")]
    public int IsHomePickUp { get; set; }

    /// <summary>保险止期（<c>insurance_end_date</c>；格式 <c>yyyy-mm-dd hh24:mi:ss</c>）。</summary>
    /// <remarks>本字段未列入官方「返回体参数表」，但出现在官方返回示例中，故一并承载。</remarks>
    [JsonPropertyName("insurance_end_date")]
    public string? InsuranceEndDate { get; set; }
}
