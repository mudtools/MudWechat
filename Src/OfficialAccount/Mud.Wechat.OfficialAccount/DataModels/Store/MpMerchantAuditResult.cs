// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Store;

/// <summary>门店小程序审核结果（官方 <c>data</c>）。</summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>audit_id</c> / <c>status</c>（<c>0</c> 未提交审核 / <c>1</c> 审核成功 / <c>2</c> 审核中 /
/// <c>3</c> 审核失败 / <c>4</c> 管理员拒绝）/ <c>reason</c>（审核状态为 3 或 4 时列出审核失败的原因）。
/// </para>
/// <para>
/// <b>类型冲突口径（标量取官方返回示例）</b>：官方字段表标 <c>reason</c> 为 <c>number</c>，
/// 但返回示例为<b>字符串</b>（<c>""</c>）；「审核失败的原因」语义上亦为可读文本。
/// 按本 SDK 的冲突处置规则「<b>标量类型冲突取官方返回示例</b>」，建模为 <c>string</c>
/// （若按 <c>number</c> 建模则在官方示例形态下必然反序列化失败——字段表口径被示例直接证伪）。
/// </para>
/// <para>官方文档缺陷：<c>audit_id</c> 同时出现在请求参数与返回 <c>data</c> 内，未说明两者是否恒等（照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpMerchantAuditResult
{
    /// <summary>获取或设置审核单 id（官方 <c>audit_id</c>）。</summary>
    [JsonPropertyName("audit_id")]
    public int? AuditId { get; set; }

    /// <summary>获取或设置审核状态（官方 <c>status</c>；取值见 <c>MpMerchantAuditStatuses</c>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置审核失败原因（官方 <c>reason</c>；仅 <c>status</c> 为 3 / 4 时返回；字段表标 number、示例为字符串，取示例口径）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}
