// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 申诉记录项（<c>getappealrecords</c> 应答 <c>records</c> 元素）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record-management/getAppealRecords.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpAppealRecord
{
    /// <summary>申诉记录 ID（官方 <c>appeal_record_id</c>，string）。</summary>
    [JsonPropertyName("appeal_record_id")]
    public string? AppealRecordId { get; set; }

    /// <summary>申诉时间（官方 <c>appeal_time</c>，number）。</summary>
    [JsonPropertyName("appeal_time")]
    public long? AppealTime { get; set; }

    /// <summary>申诉次数（官方 <c>appeal_count</c>，number）。</summary>
    [JsonPropertyName("appeal_count")]
    public long? AppealCount { get; set; }

    /// <summary>申诉来源（官方 <c>appeal_from</c>，string）。</summary>
    [JsonPropertyName("appeal_from")]
    public string? AppealFrom { get; set; }

    /// <summary>申诉状态（官方 <c>appeal_status</c>，number）。</summary>
    [JsonPropertyName("appeal_status")]
    public long? AppealStatus { get; set; }

    /// <summary>审核时间（官方 <c>audit_time</c>，number）。</summary>
    [JsonPropertyName("audit_time")]
    public long? AuditTime { get; set; }

    /// <summary>审核意见（官方 <c>audit_reason</c>，string）。</summary>
    [JsonPropertyName("audit_reason")]
    public string? AuditReason { get; set; }

    /// <summary>处罚描述（官方 <c>punish_description</c>，string）。</summary>
    [JsonPropertyName("punish_description")]
    public string? PunishDescription { get; set; }

    /// <summary>申诉材料（官方 <c>materials</c>）。</summary>
    [JsonPropertyName("materials")]
    public OpenPlatformMpAppealMaterials? Materials { get; set; }
}
