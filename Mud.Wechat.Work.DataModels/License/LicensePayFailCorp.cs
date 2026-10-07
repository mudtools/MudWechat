// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 余额支付失败的企业及原因列表项（<c>fail_corp_list</c> 元素，<c>/cgi-bin/license/pay_job_result</c>，官方 FailCorp）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class LicensePayFailCorp
{
    /// <summary>获取或设置企业 corpid。</summary>
    [JsonPropertyName("corpid")]
    public string? Corpid { get; set; }

    /// <summary>获取或设置本企业支付失败原因的错误码（可能的错误码见官方 pay_job_result.errcode 说明）。</summary>
    [JsonPropertyName("errcode")]
    public int? Errcode { get; set; }

    /// <summary>获取或设置本企业支付失败原因的错误码说明。</summary>
    [JsonPropertyName("errmsg")]
    public string? Errmsg { get; set; }
}
