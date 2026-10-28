// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.FollowUser;

/// <summary>
/// 检查用户是否配置了客户联系功能使用权限请求体（<c>/cgi-bin/externalcontact/check_follow_user</c>）。
/// <para><see cref="Userid"/> 与 <see cref="Partyid"/> 均可空，按传入的身份集合逐项检查。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "FollowUser")]
public class CheckFollowUserRequest
{
    /// <summary>
    /// 获取或设置待检查的用户 userid 列表。
    /// </summary>
    [JsonPropertyName("userid")]
    public List<string>? Userid { get; set; }

    /// <summary>
    /// 获取或设置待检查的部门 id 列表。
    /// </summary>
    [JsonPropertyName("partyid")]
    public List<long>? Partyid { get; set; }
}
