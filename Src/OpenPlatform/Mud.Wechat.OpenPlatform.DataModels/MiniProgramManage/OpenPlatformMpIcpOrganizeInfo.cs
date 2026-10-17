// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// ICP 备案组织主体证件信息（<c>icp_subject.organize_info</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/applyIcpFiling.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpIcpOrganizeInfo
{
    /// <summary>组织证件类型（官方 <c>certificate_type</c>，number）。</summary>
    [JsonPropertyName("certificate_type")]
    public long? CertificateType { get; set; }

    /// <summary>组织证件号码（官方 <c>certificate_number</c>）。</summary>
    [JsonPropertyName("certificate_number")]
    public string? CertificateNumber { get; set; }

    /// <summary>组织证件地址（官方 <c>certificate_address</c>）。</summary>
    [JsonPropertyName("certificate_address")]
    public string? CertificateAddress { get; set; }

    /// <summary>组织证件照片（官方 <c>certificate_photo</c>）。</summary>
    [JsonPropertyName("certificate_photo")]
    public string? CertificatePhoto { get; set; }
}
