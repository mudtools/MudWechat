// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 微盘空间信息（获取空间信息 <c>/cgi-bin/wedrive/space_info</c> 响应的 space_info 对象，旧版结构）。
/// </summary>
/// <remarks>
/// <para>
/// 新版「获取空间信息」（<c>/cgi-bin/wedrive/new_space_info</c>）在同结构上额外返回安全设置
/// secure_setting，见 <see cref="WedriveNewSpaceInfo"/>；两版结构不同构，本 SDK 分别以两个类型承载。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class WedriveSpaceInfo
{
    /// <summary>获取或设置空间 spaceid。</summary>
    [JsonPropertyName("spaceid")]
    public string? Spaceid { get; set; }

    /// <summary>获取或设置空间名称。</summary>
    [JsonPropertyName("space_name")]
    public string? SpaceName { get; set; }

    /// <summary>获取或设置空间成员及部门权限列表。</summary>
    [JsonPropertyName("auth_list")]
    public WedriveSpaceAuthList? AuthList { get; set; }

    /// <summary>获取或设置空间类型：0 - 普通空间。</summary>
    [JsonPropertyName("space_sub_type")]
    public ulong? SpaceSubType { get; set; }
}
