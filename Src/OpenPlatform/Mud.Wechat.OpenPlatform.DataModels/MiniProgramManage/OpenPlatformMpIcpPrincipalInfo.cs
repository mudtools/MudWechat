// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// ICP 备案主体负责人信息（各备案接口共享的 <c>principal_info</c> 结构）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/applyIcpFiling.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpIcpPrincipalInfo
{
    /// <summary>负责人姓名（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>负责人手机号（官方 <c>mobile</c>）。</summary>
    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    /// <summary>负责人邮箱（官方 <c>email</c>）。</summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>应急联系方式（官方 <c>emergency_contact</c>）。</summary>
    [JsonPropertyName("emergency_contact")]
    public string? EmergencyContact { get; set; }

    /// <summary>负责人证件类型（官方 <c>certificate_type</c>，number）。</summary>
    [JsonPropertyName("certificate_type")]
    public long? CertificateType { get; set; }

    /// <summary>负责人证件号码（官方 <c>certificate_number</c>）。</summary>
    [JsonPropertyName("certificate_number")]
    public string? CertificateNumber { get; set; }

    /// <summary>证件有效期开始日期（官方 <c>certificate_validity_date_start</c>）。</summary>
    [JsonPropertyName("certificate_validity_date_start")]
    public string? CertificateValidityDateStart { get; set; }

    /// <summary>证件有效期截止日期（官方 <c>certificate_validity_date_end</c>）。</summary>
    [JsonPropertyName("certificate_validity_date_end")]
    public string? CertificateValidityDateEnd { get; set; }

    /// <summary>证件正面照片（官方 <c>certificate_photo_front</c>）。</summary>
    [JsonPropertyName("certificate_photo_front")]
    public string? CertificatePhotoFront { get; set; }

    /// <summary>证件反面照片（官方 <c>certificate_photo_back</c>）。</summary>
    [JsonPropertyName("certificate_photo_back")]
    public string? CertificatePhotoBack { get; set; }

    /// <summary>授权书（官方 <c>authorization_letter</c>）。</summary>
    [JsonPropertyName("authorization_letter")]
    public string? AuthorizationLetter { get; set; }

    /// <summary>人脸核身任务 id（官方 <c>verify_task_id</c>）。</summary>
    [JsonPropertyName("verify_task_id")]
    public string? VerifyTaskId { get; set; }
}
