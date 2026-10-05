// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 获取文件权限信息响应体（<c>/cgi-bin/wedrive/get_file_permission</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class GetWedriveFilePermissionResponse : WechatWorkResponse
{
    /// <summary>获取或设置文件分享设置（详见 <see cref="WedriveFileShareRange"/>）。</summary>
    [JsonPropertyName("share_range")]
    public WedriveFileShareRange? ShareRange { get; set; }

    /// <summary>获取或设置文件安全配置（详见 <see cref="WedriveFileSecureSetting"/>）。</summary>
    [JsonPropertyName("secure_setting")]
    public WedriveFileSecureSetting? SecureSetting { get; set; }

    /// <summary>获取或设置从文件父路径继承的权限（详见 <see cref="WedriveFileInheritFatherAuth"/>）。</summary>
    [JsonPropertyName("inherit_father_auth")]
    public WedriveFileInheritFatherAuth? InheritFatherAuth { get; set; }

    /// <summary>
    /// 获取或设置文档成员及其他授权列表（fileid 为文档时返回）。
    /// <para>官方契约陷阱：本字段官方参数表列 obj、返回示例实为数组（元素 type / userid / auth），
    /// 本 SDK 按数组承载（对齐 <see cref="WedriveSpaceAclMember"/> 共用结构）。</para>
    /// </summary>
    [JsonPropertyName("file_member_list")]
    public List<WedriveSpaceAclMember>? FileMemberList { get; set; }

    /// <summary>
    /// 获取或设置分享指定的部门/成员列表。
    /// <para>官方参数表列为 obj、元素字段 type（后续将废弃）/ userid / departmentid（后续将废弃）、
    /// 未出现在返回示例中，本 SDK 按同族成员列表形态以数组承载。</para>
    /// </summary>
    [JsonPropertyName("co_auth_list")]
    public List<WedriveSpaceAclMember>? CoAuthList { get; set; }

    /// <summary>获取或设置水印设置（详见 <see cref="WedriveFileWatermark"/>；除 show_visitor_name 外仅文档可设置）。</summary>
    [JsonPropertyName("watermark")]
    public WedriveFileWatermark? Watermark { get; set; }
}
