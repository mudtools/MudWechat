// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// ICP 备案主体信息（<c>icp_subject</c>，各备案接口共享结构）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/applyIcpFiling.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpIcpSubject
{
    /// <summary>主体基础信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public OpenPlatformMpIcpSubjectBaseInfo? BaseInfo { get; set; }

    /// <summary>个人主体信息（官方 <c>personal_info</c>，个人主体时填写）。</summary>
    [JsonPropertyName("personal_info")]
    public OpenPlatformMpIcpPersonalInfo? PersonalInfo { get; set; }

    /// <summary>组织主体证件信息（官方 <c>organize_info</c>，组织主体时填写）。</summary>
    [JsonPropertyName("organize_info")]
    public OpenPlatformMpIcpOrganizeInfo? OrganizeInfo { get; set; }

    /// <summary>主体负责人信息（官方 <c>principal_info</c>）。</summary>
    [JsonPropertyName("principal_info")]
    public OpenPlatformMpIcpPrincipalInfo? PrincipalInfo { get; set; }

    /// <summary>主体法人信息（官方 <c>legal_person_info</c>）。</summary>
    [JsonPropertyName("legal_person_info")]
    public OpenPlatformMpIcpLegalPersonInfo? LegalPersonInfo { get; set; }
}
