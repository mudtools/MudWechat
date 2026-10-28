// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contacts.Tags;

/// <summary>
/// 增加标签成员请求体（<c>/cgi-bin/tag/addtagusers</c>；调用的应用必须是指定标签的创建者，成员属于应用的可见范围）。
/// </summary>
/// <remarks><see cref="UserList"/> 与 <see cref="PartyList"/> 不能同时为空；每个标签下部门数和人员数总和不能超过 3 万个。</remarks>
[HttpJsonSerializable(SerializerClassName = "Tags")]
public class AddTagMembersRequest
{
    /// <summary>
    /// 获取或设置标签 ID。
    /// </summary>
    [JsonPropertyName("tagid")]
    public int TagId { get; set; }

    /// <summary>
    /// 获取或设置企业成员 ID 列表（单次请求个数不超过 1000；与 PartyList 不能同时为空）。
    /// </summary>
    [JsonPropertyName("userlist")]
    public List<string>? UserList { get; set; }

    /// <summary>
    /// 获取或设置企业部门 ID 列表（单次请求个数不超过 100；与 UserList 不能同时为空）。
    /// </summary>
    [JsonPropertyName("partylist")]
    public List<int>? PartyList { get; set; }
}
