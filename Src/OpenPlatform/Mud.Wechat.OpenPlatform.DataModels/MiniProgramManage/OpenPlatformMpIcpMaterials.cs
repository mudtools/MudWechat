// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// ICP 备案其他材料（<c>icp_materials</c>，各备案接口共享结构；按官方场景选填）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/applyIcpFiling.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpIcpMaterials
{
    /// <summary>承诺书（官方 <c>commitment_letter</c>，array）。</summary>
    [JsonPropertyName("commitment_letter")]
    public List<string>? CommitmentLetter { get; set; }

    /// <summary>名称改动说明函（官方 <c>business_name_change_letter</c>，array）。</summary>
    [JsonPropertyName("business_name_change_letter")]
    public List<string>? BusinessNameChangeLetter { get; set; }

    /// <summary>党建确认函（官方 <c>party_building_confirmation_letter</c>，array）。</summary>
    [JsonPropertyName("party_building_confirmation_letter")]
    public List<string>? PartyBuildingConfirmationLetter { get; set; }

    /// <summary>承诺视频（官方 <c>promise_video</c>，array）。</summary>
    [JsonPropertyName("promise_video")]
    public List<string>? PromiseVideo { get; set; }

    /// <summary>主体真实性责任书（官方 <c>authenticity_responsibility_letter</c>，array）。</summary>
    [JsonPropertyName("authenticity_responsibility_letter")]
    public List<string>? AuthenticityResponsibilityLetter { get; set; }

    /// <summary>真实性承诺书（官方 <c>authenticity_commitment_letter</c>，array）。</summary>
    [JsonPropertyName("authenticity_commitment_letter")]
    public List<string>? AuthenticityCommitmentLetter { get; set; }

    /// <summary>网站建设方案书（官方 <c>website_construction_proposal</c>，array）。</summary>
    [JsonPropertyName("website_construction_proposal")]
    public List<string>? WebsiteConstructionProposal { get; set; }

    /// <summary>主体其他材料（官方 <c>subject_other_materials</c>，array）。</summary>
    [JsonPropertyName("subject_other_materials")]
    public List<string>? SubjectOtherMaterials { get; set; }

    /// <summary>小程序其他材料（官方 <c>applets_other_materials</c>，array）。</summary>
    [JsonPropertyName("applets_other_materials")]
    public List<string>? AppletsOtherMaterials { get; set; }

    /// <summary>持股证明照片（官方 <c>holding_certificate_photo</c>，array）。</summary>
    [JsonPropertyName("holding_certificate_photo")]
    public List<string>? HoldingCertificatePhoto { get; set; }
}
