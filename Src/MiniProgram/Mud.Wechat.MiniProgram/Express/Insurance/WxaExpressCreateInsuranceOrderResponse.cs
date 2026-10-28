// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>发货时投保响应（<c>POST /wxa/business/insurance_freight/createorder</c>）。</summary>
/// <remarks>官方错误码要点：<c>41009</c>（missing openid）。<c>estimate_amount</c> 为保险公司<b>预估</b>理赔金额（单位：分）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCreateInsuranceOrderResponse : WxaResponse
{
    /// <summary>保单号（<c>policy_no</c>）。</summary>
    [JsonPropertyName("policy_no")]
    public string? PolicyNo { get; set; }

    /// <summary>保险止期（<c>insurance_end_date</c>；格式 <c>yyyy-mm-dd hh24:mi:ss</c>）。</summary>
    [JsonPropertyName("insurance_end_date")]
    public string? InsuranceEndDate { get; set; }

    /// <summary>保险公司预估理赔金额（<c>estimate_amount</c>；单位：分）。</summary>
    [JsonPropertyName("estimate_amount")]
    public long EstimateAmount { get; set; }

    /// <summary>保费（<c>premium</c>；单位：分）。</summary>
    [JsonPropertyName("premium")]
    public long Premium { get; set; }
}
