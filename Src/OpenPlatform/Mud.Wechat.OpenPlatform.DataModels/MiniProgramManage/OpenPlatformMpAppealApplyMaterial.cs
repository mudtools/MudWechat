// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 申诉方提交材料（<c>getappealrecords</c> 应答 <c>materials.appeal_material</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record-management/getAppealRecords.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpAppealApplyMaterial
{
    /// <summary>申诉理由（官方 <c>reason</c>，string）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>证明材料 ID 列表（官方 <c>proof_material_ids</c>，array）。</summary>
    [JsonPropertyName("proof_material_ids")]
    public List<string>? ProofMaterialIds { get; set; }
}
