// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// ICP 备案小程序信息（<c>icp_applets</c>，各备案接口共享结构；官方复用主体负责人叶子结构 <see cref="OpenPlatformMpIcpPrincipalInfo"/>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/applyIcpFiling.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpIcpApplets
{
    /// <summary>小程序基础信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public OpenPlatformMpIcpAppletBaseInfo? BaseInfo { get; set; }

    /// <summary>小程序负责人信息（官方 <c>principal_info</c>）。</summary>
    [JsonPropertyName("principal_info")]
    public OpenPlatformMpIcpPrincipalInfo? PrincipalInfo { get; set; }
}
