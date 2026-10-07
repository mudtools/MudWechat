// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 余额支付订单任务的支付结果信息（<c>pay_job_result</c>，<c>/cgi-bin/license/pay_job_result</c>，官方 PayJobResult）。
/// </summary>
/// <remarks>
/// <para>官方口径：仅在支付失败时返回。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "License")]
public class LicensePayJobResult
{
    /// <summary>获取或设置支付失败原因的错误码（可能包含账户原因和企业原因，详见官方 pay_job_result.errcode 说明）。</summary>
    [JsonPropertyName("errcode")]
    public int? Errcode { get; set; }

    /// <summary>获取或设置支付失败原因的错误码描述。</summary>
    [JsonPropertyName("errmsg")]
    public string? Errmsg { get; set; }

    /// <summary>
    /// 获取或设置支付失败原因属于企业原因时的企业列表。
    /// <para>官方口径：单企业下单时列表长度为 1。</para>
    /// </summary>
    [JsonPropertyName("fail_corp_list")]
    public List<LicensePayFailCorp>? FailCorpList { get; set; }
}
