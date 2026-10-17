// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 线上版版本信息（<c>getversioninfo</c> 的 <c>release_info</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/getVersionInfo.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpReleaseInfo
{
    /// <summary>线上版发布时间（官方 <c>release_time</c>，number）。</summary>
    [JsonPropertyName("release_time")]
    public long ReleaseTime { get; set; }

    /// <summary>线上版用户版本号（官方 <c>release_version</c>）。</summary>
    [JsonPropertyName("release_version")]
    public string? ReleaseVersion { get; set; }

    /// <summary>线上版描述（官方 <c>release_desc</c>）。</summary>
    [JsonPropertyName("release_desc")]
    public string? ReleaseDesc { get; set; }
}
