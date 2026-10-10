// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 第三方「获取访问用户身份」响应中的家长条目（<c>parents</c> 数组元素，
/// <c>/cgi-bin/service/getuserinfo3rd</c> 与 <c>/cgi-bin/service/school/getuserinfo3rd</c> 共用）。
/// <para>
/// <see cref="ExternalUserid"/> 仅获取访问用户身份（getuserinfo3rd）的家长分支返回，
/// 兼容旧版字段（建议使用 parents 列表）。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolAuthThirdPartyParentItem
{
    /// <summary>
    /// 获取或设置家长所在学校的 corpid。
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? CorpId { get; set; }

    /// <summary>
    /// 获取或设置家长在家校通讯录里的 parent_userid。
    /// </summary>
    [JsonPropertyName("parent_userid")]
    public string? ParentUserid { get; set; }

    /// <summary>
    /// 获取或设置家长的外部联系人 id（仅 getuserinfo3rd 家长分支返回的兼容旧版字段）。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }
}
