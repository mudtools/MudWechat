// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>拉取保单信息请求体（<c>POST /wxa/business/insurance_freight/getorderlist</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_getorderlist"/>（接口英文名 <c>insurance_freight_getorderlist</c>）。</para>
/// <para><b>全字段可选</b>（官方原文）：不传即按默认口径拉取；<c>limit</c> 默认 100、<b>最大 100</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetOrderListRequest
{
    /// <summary>买家 OpenId（<c>openid</c>，可选；与投保理赔保持一致）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>微信支付单号（<c>order_no</c>，可选）。</summary>
    [JsonPropertyName("order_no")]
    public string? OrderNo { get; set; }

    /// <summary>保单号（<c>policy_no</c>，可选）。</summary>
    [JsonPropertyName("policy_no")]
    public string? PolicyNo { get; set; }

    /// <summary>理赔报案号（<c>report_no</c>，可选）。</summary>
    [JsonPropertyName("report_no")]
    public string? ReportNo { get; set; }

    /// <summary>发货运单号（<c>delivery_no</c>，可选）。</summary>
    [JsonPropertyName("delivery_no")]
    public string? DeliveryNo { get; set; }

    /// <summary>退款运单号（<c>refund_delivery_no</c>，可选）。</summary>
    [JsonPropertyName("refund_delivery_no")]
    public string? RefundDeliveryNo { get; set; }

    /// <summary>查询开始时间（<c>begin_time</c>，可选；秒级时间戳）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>查询结束时间（<c>end_time</c>，可选；秒级时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>保单状态过滤（<c>status_list</c>，可选；枚举见 <see cref="WxaExpressInsuranceOrderItem"/>）。</summary>
    [JsonPropertyName("status_list")]
    public int[]? StatusList { get; set; }

    /// <summary>分页偏移（<c>offset</c>，可选）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>分页大小（<c>limit</c>，可选；默认 100，最大 100）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>排序方式（<c>sort_direct</c>，可选；<c>0</c> 按 create_time 正序，<c>1</c> 倒序）。</summary>
    [JsonPropertyName("sort_direct")]
    public int? SortDirection { get; set; }
}
