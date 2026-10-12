// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>运力回传用户评价反馈请求体（<c>POST /cgi-bin/express/delivery/single_waybill/evaluate_feedback</c>，散单寄件）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterevaluatefeedback"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressSingleWaybillEvaluateFeedbackRequest
{
    /// <summary>商户侧下单事件中推送的 Token 字段（<c>token</c>，必填）。</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>运单 ID（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>评价处理结果（<c>result</c>，必填；0 已处理，1 不处理）。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>处理结果描述（<c>desc</c>，可选）。</summary>
    [JsonPropertyName("desc")]
    public string? Description { get; set; }
}
