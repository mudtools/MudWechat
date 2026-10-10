// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 微盘文件安全配置（获取文件权限信息响应的 secure_setting 对象）。
/// </summary>
/// <remarks>
/// <para>
/// 与空间安全设置（<see cref="WedriveSpaceSecureSetting"/>）字段集不同构，分类型承载；
/// 「禁止分享到企业外部」本对象字段名作 <c>ban_share_external</c>（与空间安全设置同名、
/// 与文件分享范围 enable_* 命名形态不同）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class WedriveFileSecureSetting
{
    /// <summary>获取或设置是否开启只读备份。</summary>
    [JsonPropertyName("enable_readonly_copy")]
    public bool? EnableReadonlyCopy { get; set; }

    /// <summary>获取或设置是否只允许管理员进行修改。</summary>
    [JsonPropertyName("modify_only_by_admin")]
    public bool? ModifyOnlyByAdmin { get; set; }

    /// <summary>获取或设置是否开启只读评论。</summary>
    [JsonPropertyName("enable_readonly_comment")]
    public bool? EnableReadonlyComment { get; set; }

    /// <summary>获取或设置是否禁止分享到企业外部。</summary>
    [JsonPropertyName("ban_share_external")]
    public bool? BanShareExternal { get; set; }
}
