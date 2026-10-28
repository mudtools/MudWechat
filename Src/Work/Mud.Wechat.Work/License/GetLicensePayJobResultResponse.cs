// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 获取订单支付结果响应体（<c>/cgi-bin/license/pay_job_result</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：顶层 <c>errcode</c> 表示接口调用是否成功（而非支付是否成功），
/// 支付失败时该错误码也会返回 0，支付是否成功须以 <see cref="Status"/> 与
/// <see cref="PayJobResult"/> 判定。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "License")]
public class GetLicensePayJobResultResponse : WechatWorkResponse
{
    /// <summary>获取或设置支付任务结果：<c>1</c>-支付成功 / <c>2</c>-支付任务执行中，稍后再试 / <c>3</c>-支付失败。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置支付结果的信息（仅在支付失败时返回）。</summary>
    [JsonPropertyName("pay_job_result")]
    public LicensePayJobResult? PayJobResult { get; set; }
}
