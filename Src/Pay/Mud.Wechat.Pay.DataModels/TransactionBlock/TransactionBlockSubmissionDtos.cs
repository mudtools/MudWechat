// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.TransactionBlock;

/// <summary>查询交易拦截申诉详情响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014940449"/></summary>
[HttpJsonSerializable(SerializerClassName = "TransactionBlock")]
public class TransactionBlockSubmissionInfoResponse : WechatPayResponse
{
    /// <summary>【交易拦截申诉ID】 请求成功时返回，交易拦截申诉记录的主键，唯一定义此资源的标识（官方字段路径 <c>block_submission_id</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("block_submission_id")]
    public string? BlockSubmissionId { get; set; }

    /// <summary>【提交时间】 请求成功时返回，遵循rfc3339标准格式：yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>submit_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("submit_time")]
    public string? SubmitTime { get; set; }

    /// <summary>【审核结果】 请求成功时返回，申诉审核结果。可选取值：`EMPTY` 审核未完成 / `PASS` 业务审核通过 / `REJECT` 业务审核驳回（官方字段路径 <c>review_result</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("review_result")]
    public string? ReviewResult { get; set; }

    /// <summary>【审核时间】 请求成功且审核完成时返回，遵循rfc3339标准格式：yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>review_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("review_time")]
    public string? ReviewTime { get; set; }

    /// <summary>【审核驳回原因】 请求成功时返回，审核不通过的原因（官方字段路径 <c>review_reject_reason</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("review_reject_reason")]
    public string? ReviewRejectReason { get; set; }

}

/// <summary>发起交易拦截申诉请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014940446"/></summary>
[HttpJsonSerializable(SerializerClassName = "TransactionBlock")]
public class TransactionBlockSubmissionCreateRequest
{
    /// <summary>【商户提交资料内容】 提交资料的内容，基于交易拦截记录查询接口返回的收集资料与加密要求，提交资料内容，具体加密规则请参考 资料格式说明 文档（官方字段路径 <c>submit_data</c>，<b>必填</b>，类型 <c>string(4000000)</c>）。</summary>
    [JsonPropertyName("submit_data")]
    public string? SubmitData { get; set; }

    /// <summary>【商户侧申诉单号】 商户侧的申诉单号，用于记录和对账（官方字段路径 <c>merchant_submit_id</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("merchant_submit_id")]
    public string? MerchantSubmitId { get; set; }

}
