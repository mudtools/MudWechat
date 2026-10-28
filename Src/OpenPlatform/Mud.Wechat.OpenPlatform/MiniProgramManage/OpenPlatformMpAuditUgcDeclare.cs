// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「提交审核」用户生成内容声明（<c>submit_audit</c> 的 <c>ugc_declare</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/submitAudit.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpAuditUgcDeclare
{
    /// <summary>场景枚举值（官方必填 <c>scene</c>，number）。</summary>
    [JsonPropertyName("scene")]
    public long? Scene { get; set; }

    /// <summary>审核方式（官方必填 <c>method</c>，number）。</summary>
    [JsonPropertyName("method")]
    public long? Method { get; set; }

    /// <summary>其它场景说明（官方选填 <c>other_scene_desc</c>，string）。</summary>
    [JsonPropertyName("other_scene_desc")]
    public string? OtherSceneDesc { get; set; }

    /// <summary>是否有审核团队（官方选填 <c>has_audit_team</c>，number）。</summary>
    [JsonPropertyName("has_audit_team")]
    public long? HasAuditTeam { get; set; }

    /// <summary>审核机制描述（官方必填 <c>audit_desc</c>，string）。</summary>
    [JsonPropertyName("audit_desc")]
    public string? AuditDesc { get; set; }
}
