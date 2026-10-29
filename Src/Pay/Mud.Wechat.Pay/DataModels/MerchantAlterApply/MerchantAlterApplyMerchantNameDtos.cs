// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MerchantAlterApply;

/// <summary>
/// 商家简称变更申请的包体（官方 <c>POST /v3/mchalterapply/merchantnamealterapplyment</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016166071"/>，2026-10-14 逐字段核验。</para>
/// <para><b>本族定位键是 <c>sub_mchid</c>，不是主体/超管两族的 <c>merchant_code</c></b>：
/// 三族同域同前缀却用两个不同的商户字段名（查询支的路径段 likewise 是
/// <c>merchant/{sub_mchid}</c> vs <c>merchant/{merchant_code}</c>）⇒ 照抄另两族的请求即错
/// （守卫 MAA-B3 三族各自断言）。</para>
/// <para><b>本族无加密字段</b>：<c>merchant_shortname</c> 是明文商家简称，该页<b>未</b>把
/// <c>Wechatpay-Serial</c> 列入请求头参数表（主体族与超管族两页则列为必填）⇒ 不得因「同域」给它加加密头。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantAlterApply")]
public class MerchantAlterApplyMerchantNameSubmitRequest
{
    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string(32)）：本服务商负责进件的特约商户号，<b>非</b> <c>merchant_code</c>。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>业务申请编号（<c>out_request_no</c>，必填 <b>string(124)</b>）：与主体族的 64 不同，照页。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>商家简称（<c>merchant_shortname</c>，必填 string(64)）：明文提交，本族无加密字段。</summary>
    [JsonPropertyName("merchant_shortname")]
    public string? MerchantShortname { get; set; }
}

/// <summary>
/// 商家简称变更申请单的查询应答（<b>按申请单号与按「二级商户号 + 业务申请编号」两支共用</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016166074"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4016166085"/>，2026-10-14 逐字段核验。</para>
/// <para><b>本族应答没有 <c>audit_reject_detail</c> 列表</b>（主体族与超管族都有）：
/// 简称变更只改一个字段，官方只回总体驳回原因 ⇒ 三族查询应答三型分建，直接依据就是这一处字段缺失
/// （守卫 MAA-B6）。</para>
/// <para><b><c>state</c> 值清单本页印 5 值</b>，与本族撤销页只印 1 值不同 ⇒ 建模按 5 值集承载，
/// 撤销支不另建窄枚举（守卫 MAA-B4）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantAlterApply")]
public class MerchantAlterApplyMerchantNameQueryResponse : WechatPayResponse
{
    /// <summary>申请单号（<c>apply_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("apply_id")]
    public string? ApplyId { get; set; }

    /// <summary>业务申请编号（<c>out_request_no</c>，必填 string(64)）：数字/字母/下划线，建议前缀为服务商商户号。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string(32)）：本族定位键，另两族为 <c>merchant_code</c>。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>申请单状态（<c>state</c>，必填 string）：本族 5 值，见 <see cref="MerchantAlterApplyMerchantNameStates"/>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>总体驳回原因（<c>audit_reject_reason</c>，选填 string）：仅 <c>REJECTED</c> 状态返回；本族无逐项明细。</summary>
    [JsonPropertyName("audit_reject_reason")]
    public string? AuditRejectReason { get; set; }
}
