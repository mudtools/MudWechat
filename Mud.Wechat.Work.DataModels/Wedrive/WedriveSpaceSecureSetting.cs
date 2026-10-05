// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 微盘空间安全设置（新版获取空间信息 <c>/cgi-bin/wedrive/new_space_info</c> 响应的
/// space_info.secure_setting 对象）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约差异：本结构为只读回显形态；写入口「安全设置」（<c>/cgi-bin/wedrive/space_setting</c>）
/// 仅承载其中可写的 6 个字段，且「禁止分享到企业外」官方请求字段名为 <c>ban_share_external</c>、
/// 响应回显字段名为 <c>enable_share_external</c>，两侧字段名不同构。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class WedriveSpaceSecureSetting
{
    /// <summary>获取或设置是否启用水印（仅专业版企业可设置）：false - 关；true - 开。</summary>
    [JsonPropertyName("enable_watermark")]
    public bool? EnableWatermark { get; set; }

    /// <summary>获取或设置是否仅空间管理员可添加成员：false - 关；true - 开。</summary>
    [JsonPropertyName("add_member_only_admin")]
    public bool? AddMemberOnlyAdmin { get; set; }

    /// <summary>获取或设置是否开启空间邀请链接：false - 关；true - 开。</summary>
    [JsonPropertyName("enable_share_url")]
    public bool? EnableShareUrl { get; set; }

    /// <summary>获取或设置通过链接加入空间是否无需审批：false - 关；true - 开。</summary>
    [JsonPropertyName("share_url_no_approve")]
    public bool? ShareUrlNoApprove { get; set; }

    /// <summary>
    /// 获取或设置邀请链接默认权限：1 - 仅下载；2 - 可编辑；4 - 仅预览；5 - 可上传下载；200 - 自定义权限。
    /// </summary>
    [JsonPropertyName("share_url_no_approve_default_auth")]
    public ulong? ShareUrlNoApproveDefaultAuth { get; set; }

    /// <summary>获取或设置是否允许文件分享到企业外：false - 禁止；true - 允许。</summary>
    [JsonPropertyName("enable_share_external")]
    public bool? EnableShareExternal { get; set; }

    /// <summary>获取或设置是否仅管理员可分享文件到企业外：false - 关；true - 开。</summary>
    [JsonPropertyName("enable_share_external_admin")]
    public bool? EnableShareExternalAdmin { get; set; }

    /// <summary>获取或设置是否允许添加企业外部成员进空间：false - 关；true - 开。</summary>
    [JsonPropertyName("enable_space_add_external_member")]
    public bool? EnableSpaceAddExternalMember { get; set; }

    /// <summary>获取或设置是否仅管理员可添加企业外部成员进空间：false - 关；true - 开。</summary>
    [JsonPropertyName("enable_space_add_external_member_admin")]
    public bool? EnableSpaceAddExternalMemberAdmin { get; set; }

    /// <summary>获取或设置是否开启保密模式：false - 关；true - 开。</summary>
    [JsonPropertyName("enable_confidential_mode")]
    public bool? EnableConfidentialMode { get; set; }

    /// <summary>获取或设置文件默认可查看范围：1 - 仅成员；2 - 企业内。</summary>
    [JsonPropertyName("default_file_scope")]
    public ulong? DefaultFileScope { get; set; }

    /// <summary>获取或设置是否仅空间管理员可新建文件/文件夹：false - 关；true - 开。</summary>
    [JsonPropertyName("create_file_only_admin")]
    public bool? CreateFileOnlyAdmin { get; set; }
}
