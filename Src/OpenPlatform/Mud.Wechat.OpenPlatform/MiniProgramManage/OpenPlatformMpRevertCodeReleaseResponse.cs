// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「回退发布版本 / 获取历史版本」（<c>revertcoderelease</c>）应答。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/revertCodeRelease.html"/>。
/// </summary>
/// <remarks>仅当 query <c>action=get_history_version</c> 时返回 <see cref="VersionList"/>；回退动作本身仅回 <c>errcode/errmsg</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpRevertCodeReleaseResponse : OpenPlatformResponse
{
    /// <summary>历史版本列表（官方 <c>version_list</c>，最多 5 个版本）。</summary>
    [JsonPropertyName("version_list")]
    public List<OpenPlatformMpHistoryVersion>? VersionList { get; set; }
}
