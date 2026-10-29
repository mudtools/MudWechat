// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.RateActivity;

/// <summary>根据申请单ID查询申请单结果响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012471722"/></summary>
[HttpJsonSerializable(SerializerClassName = "RateActivity")]
public class RateActivityApplicationStateResponse : WechatPayResponse
{
    /// <summary>【申请单状态】申请单状态。可选取值：`APPLICATION_STATE_AUDITING` 审核中，如果查到申请单是审核中的状态，请耐心等待7个工作日，微信支付将会完成审核 / `APPLICATION_STATE_PASSED` 审核通过，如果查到申请单是审核通过状态，表示活动报名成功 / `APPLICATION_STATE_REJECTED` 审核驳回，如果查到申请单是审核驳回状态，请按照驳回原因修改申请资料，重新提交申请（官方字段路径 <c>application_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("application_state")]
    public string? ApplicationState { get; set; }

    /// <summary>【子商户号】该申请单报名优惠费率活动的商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public long? SubMchid { get; set; }

    /// <summary>【驳回参数】当申请单状态为“APPLICATION_STATE_REJECTED”时，返回该字段，标识被驳回的字段名（官方字段路径 <c>reject_parameter</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("reject_parameter")]
    public string? RejectParameter { get; set; }

    /// <summary>【驳回原因】当申请单状态为“APPLICATION_STATE_REJECTED”时，返回该字段，表示驳回原因（官方字段路径 <c>reject_reason</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }

    /// <summary>【活动审核通过时间】申请单审核通过的时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示北京时间2015年05月20日13点29分35秒（官方字段路径 <c>pass_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("pass_time")]
    public string? PassTime { get; set; }

}
