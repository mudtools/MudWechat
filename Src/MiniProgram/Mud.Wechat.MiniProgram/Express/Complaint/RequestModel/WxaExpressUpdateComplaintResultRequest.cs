// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>运力返回用户投诉处理结果请求体（<c>POST /cgi-bin/express/delivery/scatter/update_complaint_result</c>，散单寄件）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterupdatecomplainresult"/>
/// （接口英文名 <c>scatterupdatecomplainresult</c>，2026-10-12 核验）。成功仅返回 <c>errcode</c>/<c>errmsg</c>。
/// </para>
/// <para>
/// <b>官方原文</b>：<c>token</c> 为<b>商户侧下单事件中推送的 Token 字段</b>——与用户评价反馈
/// （<see cref="WxaExpressSingleWaybillEvaluateFeedbackRequest"/>）同源，<b>不得</b>用应用级令牌替代。
/// 本接口<b>不支持</b>第三方平台代调用。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUpdateComplaintResultRequest
{
    /// <summary>商户侧下单事件中推送的 Token 字段（<c>token</c>，必填）。</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>运单 ID（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>处理结果（<c>result</c>，必填）。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>处理结果说明（<c>desc</c>，必填）。</summary>
    [JsonPropertyName("desc")]
    public string? Description { get; set; }
}
