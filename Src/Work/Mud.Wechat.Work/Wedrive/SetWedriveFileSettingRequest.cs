// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 分享设置（文件权限）请求体（<c>/cgi-bin/wedrive/file_setting</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class SetWedriveFileSettingRequest
{
    /// <summary>获取或设置文件 fileid（官方必填）。</summary>
    [JsonPropertyName("fileid")]
    public string? Fileid { get; set; }

    /// <summary>
    /// 获取或设置权限范围（官方必填）：1 - 指定人；2 - 企业内；3 - 企业外；
    /// 4 - 企业内需管理员审批（仅有管理员时可设置）；5 - 企业外需管理员审批（仅有管理员时可设置）。
    /// </summary>
    [JsonPropertyName("auth_scope")]
    public ulong? AuthScope { get; set; }

    /// <summary>
    /// 获取或设置权限信息：普通文档 1 - 仅浏览（可下载）、4 - 仅预览（仅专业版企业可设置）；
    /// 微文档 1 - 仅浏览（可下载）；不填充则保持原有状态。
    /// </summary>
    [JsonPropertyName("auth")]
    public ulong? Auth { get; set; }
}
