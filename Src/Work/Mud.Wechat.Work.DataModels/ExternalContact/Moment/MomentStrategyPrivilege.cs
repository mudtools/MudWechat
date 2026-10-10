// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Moment;

/// <summary>
/// 客户朋友圈规则组的权限配置（<c>privilege</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class MomentStrategyPrivilege
{
    /// <summary>
    /// 获取或设置是否允许查看成员的全部朋友圈发表（默认为 true）。
    /// </summary>
    [JsonPropertyName("view_moment_list")]
    public bool? ViewMomentList { get; set; }

    /// <summary>
    /// 获取或设置是否允许发表朋友圈（默认为 true）。
    /// </summary>
    [JsonPropertyName("send_moment")]
    public bool? SendMoment { get; set; }

    /// <summary>
    /// 获取或设置是否允许管理朋友圈封面与签名（默认为 true）。
    /// </summary>
    [JsonPropertyName("manage_moment_cover_and_sign")]
    public bool? ManageMomentCoverAndSign { get; set; }
}
