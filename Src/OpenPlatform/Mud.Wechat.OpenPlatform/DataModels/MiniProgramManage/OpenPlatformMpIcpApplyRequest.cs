// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「提交备案申请」请求（<c>apply_icp_filing</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/applyIcpFiling.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpIcpApplyRequest
{
    /// <summary>备案主体信息（官方 <c>icp_subject</c>）。</summary>
    [JsonPropertyName("icp_subject")]
    public OpenPlatformMpIcpSubject? IcpSubject { get; set; }

    /// <summary>备案小程序信息（官方 <c>icp_applets</c>）。</summary>
    [JsonPropertyName("icp_applets")]
    public OpenPlatformMpIcpApplets? IcpApplets { get; set; }

    /// <summary>备案其他材料（官方 <c>icp_materials</c>，按场景选填）。</summary>
    [JsonPropertyName("icp_materials")]
    public OpenPlatformMpIcpMaterials? IcpMaterials { get; set; }
}
