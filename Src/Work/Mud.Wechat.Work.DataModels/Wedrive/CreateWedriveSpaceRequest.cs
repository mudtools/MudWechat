// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 新建空间请求体（<c>/cgi-bin/wedrive/space_create</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class CreateWedriveSpaceRequest
{
    /// <summary>获取或设置空间标题（官方必填）。</summary>
    [JsonPropertyName("space_name")]
    public string? SpaceName { get; set; }

    /// <summary>
    /// 获取或设置空间其他成员信息。
    /// <para>应用空间管理员（auth = 7）最多可指定 3 个，且不支持设置部门；「可预览」权限（auth = 4）仅专业版微盘企业可设置。</para>
    /// </summary>
    [JsonPropertyName("auth_info")]
    public List<WedriveSpaceAclMember>? AuthInfo { get; set; }

    /// <summary>获取或设置创建空间类型：0 - 普通（目前只支持 0）。</summary>
    [JsonPropertyName("space_sub_type")]
    public ulong? SpaceSubType { get; set; }
}
