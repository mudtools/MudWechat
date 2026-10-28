// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 违规记录项（<c>getillegalrecords</c> 应答 <c>records</c> 元素）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record-management/getIllegalRecords.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpIllegalRecord
{
    /// <summary>违规记录 ID（官方 <c>illegal_record_id</c>，string）。</summary>
    [JsonPropertyName("illegal_record_id")]
    public string? IllegalRecordId { get; set; }

    /// <summary>违规发生时间（官方 <c>create_time</c>，number）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>违规原因（官方 <c>illegal_reason</c>，string）。</summary>
    [JsonPropertyName("illegal_reason")]
    public string? IllegalReason { get; set; }

    /// <summary>违规内容（官方 <c>illegal_content</c>，string）。</summary>
    [JsonPropertyName("illegal_content")]
    public string? IllegalContent { get; set; }

    /// <summary>命中规则链接（官方 <c>rule_url</c>，string）。</summary>
    [JsonPropertyName("rule_url")]
    public string? RuleUrl { get; set; }

    /// <summary>命中规则名称（官方 <c>rule_name</c>，string）。</summary>
    [JsonPropertyName("rule_name")]
    public string? RuleName { get; set; }

    /// <summary>处罚详情链接（官方 <c>case_url</c>，string）。</summary>
    [JsonPropertyName("case_url")]
    public string? CaseUrl { get; set; }
}
