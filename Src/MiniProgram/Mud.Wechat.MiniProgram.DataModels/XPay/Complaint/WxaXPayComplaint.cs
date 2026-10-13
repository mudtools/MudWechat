// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 投诉单（官方 <c>Res.complaints</c> 元素，17 字段；详情端点同形共用）。
/// </summary>
/// <remarks>
/// <para><b>列表与详情两处共用裁决</b>：<c>get_complaint_list</c> 的 <c>complaints</c> 元素子表与<c>get_complaint_detail</c> 的 <c>complaint</c> 子表字段名集逐字段相同（仅类型列标注详略不同）⇒ 共用本类型，由 <c>sameAs</c> 断言取证。</para>
/// <para><b>三支嵌套字段官方类型列不一</b>：列表侧标 <c>objarray</c>、详情侧标 <c>array</c>，而官方示例证明三支都是对象数组 ⇒ 统一具名为 <see cref="WxaXPayComplaintOrderInfo"/> / <see cref="WxaXPayComplaintMedia"/> / <see cref="WxaXPayServiceOrderInfo"/>。</para>
/// <para><b>敏感值（本域最重）</b>：<c>payer_phone</c> 是投诉人联系方式、<c>payer_openid</c> 是用户标识 ⇒ <b>严禁</b>写进日志、遥测或异常消息（与 <c>permanent_code</c> 同级处置）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_complaint_list.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayComplaint
{
    /// <summary>complaint_id（官方类型 <c>string</c>）：投诉id</summary>
    [JsonPropertyName("complaint_id")]
    public string? ComplaintId { get; set; }

    /// <summary>complaint_time（官方类型 <c>string</c>）：投诉时间 格式为yyyy-mm-dd'T'HH:MM:ssXXX，其中XXX为时区偏移，例如：2023-11-28T11:11:49+08:00</summary>
    [JsonPropertyName("complaint_time")]
    public string? ComplaintTime { get; set; }

    /// <summary>complaint_detail（官方类型 <c>string</c>）：投诉内容</summary>
    [JsonPropertyName("complaint_detail")]
    public string? ComplaintDetail { get; set; }

    /// <summary>complaint_state（官方类型 <c>string</c>）：投诉状态 PENDING-待处理；PROCESSING-处理中；PROCESSED-已处理完成</summary>
    [JsonPropertyName("complaint_state")]
    public string? ComplaintState { get; set; }

    /// <summary>payer_phone（官方类型 <c>string</c>）：投诉人联系方式</summary>
    [JsonPropertyName("payer_phone")]
    public string? PayerPhone { get; set; }

    /// <summary>payer_openid（官方类型 <c>string</c>）：投诉人在商户AppID下的唯一标识</summary>
    [JsonPropertyName("payer_openid")]
    public string? PayerOpenId { get; set; }

    /// <summary>complaint_order_info（官方类型 <c>objarray</c>）：投诉单关联订单信息</summary>
    [JsonPropertyName("complaint_order_info")]
    public List<WxaXPayComplaintOrderInfo>? ComplaintOrderInfo { get; set; }

    /// <summary>complaint_full_refunded（官方类型 <c>boolean</c>）：投诉单下所有订单是否已全部全额退款</summary>
    [JsonPropertyName("complaint_full_refunded")]
    public bool? ComplaintFullRefunded { get; set; }

    /// <summary>incoming_user_response（官方类型 <c>boolean</c>）：投诉单是否有待回复的用户留言</summary>
    [JsonPropertyName("incoming_user_response")]
    public bool? IncomingUserResponse { get; set; }

    /// <summary>user_complaint_times（官方类型 <c>number</c>）：用户投诉次数。用户首次发起投诉记为1次，用户每有一次继续投诉就加1</summary>
    [JsonPropertyName("user_complaint_times")]
    public long? UserComplaintTimes { get; set; }

    /// <summary>complaint_media_list（官方类型 <c>objarray</c>）：用户上传的投诉相关资料，包括图片凭证等</summary>
    [JsonPropertyName("complaint_media_list")]
    public List<WxaXPayComplaintMedia>? ComplaintMediaList { get; set; }

    /// <summary>problem_description（官方类型 <c>string</c>）：用户发起投诉前选择的faq标题</summary>
    [JsonPropertyName("problem_description")]
    public string? ProblemDescription { get; set; }

    /// <summary>problem_type（官方类型 <c>string</c>）：问题类型为申请退款的单据是需要最高优先处理的单据。REFUND: 申请退款；SERVICE_NOT_WORK: 服务权益未生效；OTHERS: 其他类型</summary>
    [JsonPropertyName("problem_type")]
    public string? ProblemType { get; set; }

    /// <summary>apply_refund_amount（官方类型 <c>number</c>）：当问题类型为申请退款时, 有值, (单位:分)</summary>
    [JsonPropertyName("apply_refund_amount")]
    public long? ApplyRefundAmount { get; set; }

    /// <summary>user_tag_list（官方类型 <c>array</c>）：用户标签列表，每一项内容为string。TRUSTED: 此类用户满足极速退款条件；HIGH_RISK: 高风险投诉，请按照运营要求优先妥善处理</summary>
    [JsonPropertyName("user_tag_list")]
    public List<string>? UserTagList { get; set; }

    /// <summary>service_order_info（官方类型 <c>objarray</c>）：投诉单关联服务单信息</summary>
    [JsonPropertyName("service_order_info")]
    public List<WxaXPayServiceOrderInfo>? ServiceOrderInfo { get; set; }
}
