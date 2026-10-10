// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 安全设置（修改空间权限的安全设置）请求体（<c>/cgi-bin/wedrive/space_setting</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约差异：仅支持设置由本应用创建的空间；未填充的可选字段保持原有状态
/// （<c>DefaultIgnoreCondition = WhenWritingNull</c> 保证未填充字段不会被序列化）。
/// </para>
/// <para>
/// 「禁止文件分享到企业外」官方请求字段名为 <c>ban_share_external</c>，而新版获取空间信息
/// 回显字段名为 <c>enable_share_external</c>，两侧不同构，勿「顺手修正」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class SetWedriveSpaceSettingRequest
{
    /// <summary>获取或设置空间 spaceid（官方必填）。</summary>
    [JsonPropertyName("spaceid")]
    public string? Spaceid { get; set; }

    /// <summary>获取或设置是否启用水印（仅专业版企业可设置）：false - 关；true - 开；不填充保持原有状态。</summary>
    [JsonPropertyName("enable_watermark")]
    public bool? EnableWatermark { get; set; }

    /// <summary>获取或设置是否开启保密模式：false - 关；true - 开；不填充保持原有状态。</summary>
    [JsonPropertyName("enable_confidential_mode")]
    public bool? EnableConfidentialMode { get; set; }

    /// <summary>获取或设置通过链接加入空间是否无需审批：false - 关；true - 开；不填充保持原有状态。</summary>
    [JsonPropertyName("share_url_no_approve")]
    public bool? ShareUrlNoApprove { get; set; }

    /// <summary>
    /// 获取或设置邀请链接默认权限：1 - 仅下载；2 - 可编辑；4 - 仅预览；5 - 可上传下载；200 - 自定义权限；
    /// 不填充保持原有状态。
    /// </summary>
    [JsonPropertyName("share_url_no_approve_default_auth")]
    public ulong? ShareUrlNoApproveDefaultAuth { get; set; }

    /// <summary>获取或设置文件默认可查看范围：1 - 仅成员；2 - 企业内；不填充保持原有状态。</summary>
    [JsonPropertyName("default_file_scope")]
    public ulong? DefaultFileScope { get; set; }

    /// <summary>获取或设置是否禁止文件分享到企业外：false - 关；true - 开；不填充保持原有状态。</summary>
    [JsonPropertyName("ban_share_external")]
    public bool? BanShareExternal { get; set; }
}
